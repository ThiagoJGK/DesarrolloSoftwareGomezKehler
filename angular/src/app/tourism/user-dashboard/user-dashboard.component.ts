import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TourismInteractionService } from '../../proxy/experiences/tourism-interaction.service';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { ExperienceDto } from '../../proxy/experiences/models';
import { DestinationDto } from '../../proxy/destinations/models';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.scss'
})
export class UserDashboardComponent implements OnInit {
  activeTab: 'experiences' | 'favorites' | 'admin' = 'experiences';
  
  myExperiences: ExperienceDto[] = [];
  myFavoritesIds: string[] = [];
  savedDestinations: DestinationDto[] = [];
  favoriteDestinations: DestinationDto[] = [];
  
  isAdmin = true; // Simulación. En ABP real esto lo sacamos de ConfigStateService

  // Estado de edición de experiencia
  editingExperienceId: string | null = null;
  editExperienceData = { title: '', content: '', keywords: '' };

  constructor(
    private interactionService: TourismInteractionService,
    private destinationService: DestinationService
  ) {}

  ngOnInit(): void {
    this.loadMyExperiences();
    this.loadMyFavorites();
  }

  setTab(tab: 'experiences' | 'favorites' | 'admin') {
    this.activeTab = tab;
  }

  loadMyExperiences() {
    // Cargamos experiencias de todos los destinos guardados
    this.destinationService.getSavedDestinations().subscribe(dests => {
      this.myExperiences = [];
      dests.forEach(dest => {
        if (dest.id) {
          this.interactionService.getExperiencesByDestination(dest.id).subscribe(exps => {
            this.myExperiences = [...this.myExperiences, ...exps];
          });
        }
      });
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
