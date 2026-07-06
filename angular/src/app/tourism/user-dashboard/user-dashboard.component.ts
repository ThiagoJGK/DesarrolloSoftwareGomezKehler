import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourismInteractionService } from '../../proxy/experiences/tourism-interaction.service';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { ExperienceDto } from '../../proxy/experiences/models';
import { DestinationDto } from '../../proxy/destinations/models';
import { RouterModule } from '@angular/router';
import { ProfileService } from '@abp/ng.account.core/proxy';
import { TourismUserService } from '../../proxy/users/tourism-user.service';
import { AuthService, ConfigStateService } from '@abp/ng.core';
import { NotificationService } from '../../proxy/notifications/notification.service';
import { NotificationDto } from '../../proxy/notifications/models';
import { AdminMetricsService } from '../../proxy/metrics/admin-metrics.service';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.scss'
})
export class UserDashboardComponent implements OnInit {
  activeTab: 'experiences' | 'favorites' | 'admin' | 'notifications' = 'experiences';
  
  myExperiences: ExperienceDto[] = [];
  myFavoritesIds: string[] = [];
  savedDestinations: DestinationDto[] = [];
  favoriteDestinations: DestinationDto[] = [];
  notifications: NotificationDto[] = [];
  unreadNotificationsCount = 0;

  // Métricas del Admin
  apiUsageSummary: any[] = [];
  systemStats: any = null;
  recentApiCalls: any[] = [];
  exportingMetrics = false;
  
  isAdmin = false;

  // Estado de edición de experiencia
  editingExperienceId: string | null = null;
  editExperienceData = { title: '', content: '', keywords: '' };

  // Campos de perfil de usuario
  profileName = '';
  profileSurname = '';
  profileEmail = '';
  profileUsername = '';
  profilePreferences = '';
  photoBase64 = '';

  constructor(
    private interactionService: TourismInteractionService,
    private destinationService: DestinationService,
    private profileService: ProfileService,
    private tourismUserService: TourismUserService,
    private authService: AuthService,
    private configState: ConfigStateService,
    private notificationService: NotificationService,
    private adminMetricsService: AdminMetricsService
  ) {}

  ngOnInit(): void {
    const currentUser = this.configState.getOne('currentUser');
    this.isAdmin = currentUser?.roles?.includes('admin') || false;

    this.loadMyExperiences();
    this.loadMyFavorites();
    this.loadProfile();
    this.loadNotifications();
    
    if (this.isAdmin) {
      this.loadAdminMetrics();
    }
  }

  setTab(tab: 'experiences' | 'favorites' | 'admin' | 'notifications') {
    this.activeTab = tab;
  }

  loadNotifications() {
    this.notificationService.getMyNotifications().subscribe(notifs => {
      this.notifications = notifs;
      this.unreadNotificationsCount = notifs.filter(n => !n.isRead).length;
    });
  }

  markAsRead(id: string) {
    this.notificationService.markAsRead(id).subscribe(() => {
      this.loadNotifications();
    });
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.loadNotifications();
    });
  }

  loadAdminMetrics() {
    this.adminMetricsService.getApiUsageSummary().subscribe(sum => this.apiUsageSummary = sum);
    this.adminMetricsService.getSystemStatistics().subscribe(stats => this.systemStats = stats);
    this.adminMetricsService.getRecentApiCalls().subscribe(calls => this.recentApiCalls = calls);
  }

  exportReport() {
    this.exportingMetrics = true;
    this.adminMetricsService.exportMetricsReport('CSV').subscribe(base64 => {
      const csvContent = atob(base64);
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.setAttribute('href', url);
      link.setAttribute('download', `reporte_metricas_${new Date().toISOString().substring(0,10)}.csv`);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      this.exportingMetrics = false;
    });
  }

  loadProfile() {
    this.profileService.get().subscribe(profile => {
      this.profileName = profile.name || '';
      this.profileSurname = profile.surname || '';
      this.profileEmail = profile.email || '';
      this.profileUsername = profile.userName || '';
      this.photoBase64 = profile.extraProperties?.['Photo'] || '';
      this.profilePreferences = profile.extraProperties?.['Preferences'] || '';
    });
  }

  onPhotoSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      if (file.type !== 'image/jpeg' && file.type !== 'image/png') {
        alert('Solo se permiten archivos JPG o PNG.');
        return;
      }
      if (file.size > 1024 * 1024) {
        alert('La imagen de perfil supera el límite de 1 MB permitido por razones de rendimiento.');
        return;
      }
      const reader = new FileReader();
      reader.onload = () => {
        this.photoBase64 = reader.result as string;
        this.saveProfile();
      };
      reader.readAsDataURL(file);
    }
  }

  saveProfile() {
    this.profileService.get().subscribe(profile => {
      const input = {
        userName: profile.userName,
        email: profile.email,
        name: this.profileName,
        surname: this.profileSurname,
        phoneNumber: profile.phoneNumber,
        concurrencyStamp: profile.concurrencyStamp,
        extraProperties: {
          ...profile.extraProperties,
          Photo: this.photoBase64,
          Preferences: this.profilePreferences
        }
      };
      this.profileService.update(input).subscribe({
        next: () => {
          alert('Perfil actualizado con éxito.');
          this.loadProfile();
        },
        error: (err) => {
          alert('Error al actualizar el perfil.');
          console.error(err);
        }
      });
    });
  }

  deleteAccount() {
    if (!confirm('¿Estás COMPLETAMENTE seguro de eliminar tu cuenta? Esta acción es irreversible.')) return;
    this.tourismUserService.deleteMyAccount().subscribe({
      next: () => {
        alert('Cuenta eliminada con éxito.');
        this.authService.logout().subscribe();
      },
      error: (err) => {
        alert('Error al eliminar la cuenta.');
        console.error(err);
      }
    });
  }

  loadMyExperiences() {
    this.interactionService.getMyExperiences().subscribe(exps => {
      this.myExperiences = exps;
    });
  }

  loadMyFavorites() {
    this.interactionService.getMyFavoriteDestinations().subscribe(favIds => {
      this.myFavoritesIds = favIds;
      this.destinationService.getSavedDestinations().subscribe(dests => {
        this.favoriteDestinations = dests.filter(d => favIds.includes(d.id || ''));
      });
    });
  }

  removeFromFavorites(id: string) {
    this.interactionService.removeFromFavorites(id).subscribe(() => {
      this.loadMyFavorites();
    });
  }

  startEditExperience(exp: ExperienceDto) {
    this.editingExperienceId = exp.id || null;
    this.editExperienceData = {
      title: exp.title || '',
      content: exp.content || '',
      keywords: exp.keywords || ''
    };
  }

  cancelEditExperience() {
    this.editingExperienceId = null;
  }

  saveEditExperience() {
    if (!this.editingExperienceId) return;
    this.interactionService.editExperience(
      this.editingExperienceId,
      this.editExperienceData.title,
      this.editExperienceData.content,
      this.editExperienceData.keywords
    ).subscribe(() => {
      this.editingExperienceId = null;
      this.loadMyExperiences();
    });
  }

  deleteExperience(experienceId: string) {
    if (!confirm('¿Estás seguro de eliminar esta experiencia?')) return;
    this.interactionService.deleteExperience(experienceId).subscribe(() => {
      this.loadMyExperiences();
    });
  }
}
