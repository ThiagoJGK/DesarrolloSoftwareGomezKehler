import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { DestinationDto } from '../../proxy/destinations/models';
import { TourismInteractionService } from '../../proxy/experiences/tourism-interaction.service';
import { ReviewDto, ExperienceDto, DestinationMetricsDto } from '../../proxy/experiences/models';
import { TourismUserService } from '../../proxy/users/tourism-user.service';
import { PublicUserProfileDto } from '../../proxy/users/models';
import { ConfigStateService } from '@abp/ng.core';
import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-destination-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './destination-detail.component.html',
  styleUrl: './destination-detail.component.scss'
})
export class DestinationDetailComponent implements OnInit {
  destinationId: string = '';
  destination: DestinationDto | undefined;
  metrics: DestinationMetricsDto | undefined;
  reviews: ReviewDto[] = [];
  experiences: ExperienceDto[] = [];

  newReview = { rating: 5, comment: '' };
  newExperience = { title: '', content: '', keywords: '' };

  currentUserId: string | undefined = '';
  allReviews: ReviewDto[] = [];
  selectedRatingFilter: 'all' | 'positive' | 'neutral' | 'negative' = 'all';
  experienceKeywordFilter = '';

  isFavorite = false;
  togglingFavorite = false;

  // Estado de edición de reseña
  editingReviewId: string | null = null;
  editReviewData = { rating: 5, comment: '' };

  selectedUserProfile: PublicUserProfileDto | null = null;
  showProfileModal = false;

  constructor(
    private route: ActivatedRoute,
    private destinationService: DestinationService,
    private interactionService: TourismInteractionService,
    private tourismUserService: TourismUserService,
    private configState: ConfigStateService,
    private toaster: ToasterService,
    private confirmation: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.configState.getOne('currentUser')?.id;
    this.route.paramMap.subscribe(params => {
      this.destinationId = params.get('id') || '';
      if (this.destinationId) {
        this.loadDestinationDetails();
        this.loadMetricsAndReviews();
        this.loadExperiences();
        this.checkIfFavorite();
      }
    });
  }

  loadDestinationDetails() {
    this.destinationService.getSavedDestinations().subscribe(res => {
      this.destination = res.find(d => d.id === this.destinationId);
    });
  }

  loadMetricsAndReviews() {
    this.interactionService.getDestinationAverageRating(this.destinationId).subscribe(m => this.metrics = m);
    this.interactionService.getDestinationReviews(this.destinationId).subscribe(r => {
      this.allReviews = r;
      this.applyReviewsFilter();
    });
  }

  applyReviewsFilter() {
    if (this.selectedRatingFilter === 'all') {
      this.reviews = this.allReviews;
    } else if (this.selectedRatingFilter === 'positive') {
      this.reviews = this.allReviews.filter(r => r.rating >= 4);
    } else if (this.selectedRatingFilter === 'neutral') {
      this.reviews = this.allReviews.filter(r => r.rating === 3);
    } else if (this.selectedRatingFilter === 'negative') {
      this.reviews = this.allReviews.filter(r => r.rating <= 2);
    }
  }

  setRatingFilter(filter: 'all' | 'positive' | 'neutral' | 'negative') {
    this.selectedRatingFilter = filter;
    this.applyReviewsFilter();
  }

  loadExperiences() {
    this.interactionService.getExperiencesByDestination(this.destinationId, this.experienceKeywordFilter || undefined).subscribe(e => this.experiences = e);
  }

  checkIfFavorite() {
    this.interactionService.getMyFavoriteDestinations().subscribe(favIds => {
      this.isFavorite = favIds.includes(this.destinationId);
    });
  }

  toggleFavorite() {
    this.togglingFavorite = true;
    if (this.isFavorite) {
      this.interactionService.removeFromFavorites(this.destinationId).subscribe({
        next: () => {
          this.isFavorite = false;
          this.togglingFavorite = false;
          this.toaster.info('Destino removido de tus favoritos.');
        },
        error: () => {
          this.togglingFavorite = false;
          this.toaster.error('Error al remover de favoritos.');
        }
      });
    } else {
      this.interactionService.addToFavorites(this.destinationId).subscribe({
        next: () => {
          this.isFavorite = true;
          this.togglingFavorite = false;
          this.toaster.success('Destino agregado a tus favoritos.');
        },
        error: () => {
          this.togglingFavorite = false;
          this.toaster.error('Error al agregar a favoritos.');
        }
      });
    }
  }

  submitActionReview() {
    if (!this.newReview.comment.trim()) {
      this.toaster.warn('Por favor ingresa un comentario para tu reseña.');
      return;
    }
    this.interactionService.addReview(this.destinationId, this.newReview.rating, this.newReview.comment.trim()).subscribe({
      next: () => {
        this.newReview.comment = '';
        this.toaster.success('¡Reseña publicada con éxito!');
        this.loadMetricsAndReviews();
      },
      error: () => {
        this.toaster.error('No se pudo publicar la reseña.');
      }
    });
  }

  startEditReview(rev: ReviewDto) {
    this.editingReviewId = rev.id || null;
    this.editReviewData = { rating: rev.rating, comment: rev.comment || '' };
  }

  cancelEditReview() {
    this.editingReviewId = null;
  }

  saveEditReview() {
    if (!this.editingReviewId) return;
    this.interactionService.editReview(this.editingReviewId, this.editReviewData.rating, this.editReviewData.comment.trim()).subscribe({
      next: () => {
        this.editingReviewId = null;
        this.toaster.success('Reseña actualizada.');
        this.loadMetricsAndReviews();
      },
      error: () => {
        this.toaster.error('Error al actualizar la reseña.');
      }
    });
  }

  deleteReview(reviewId: string) {
    this.confirmation.warn('¿Estás seguro de que deseas eliminar esta reseña?', 'Confirmar Eliminación').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.interactionService.deleteReview(reviewId).subscribe({
          next: () => {
            this.toaster.success('Reseña eliminada.');
            this.loadMetricsAndReviews();
          },
          error: () => {
            this.toaster.error('No se pudo eliminar la reseña.');
          }
        });
      }
    });
  }

  submitActionExperience() {
    if (!this.newExperience.title.trim() || !this.newExperience.content.trim()) {
      this.toaster.warn('Completa el título y la historia de tu diario de viaje.');
      return;
    }
    this.interactionService.createExperience(
      this.destinationId,
      this.newExperience.title.trim(),
      this.newExperience.content.trim(),
      this.newExperience.keywords.trim()
    ).subscribe({
      next: () => {
        this.newExperience = { title: '', content: '', keywords: '' };
        this.toaster.success('Diario de viaje publicado con éxito.');
        this.loadExperiences();
      },
      error: () => {
        this.toaster.error('Error al publicar el diario de viaje.');
      }
    });
  }

  viewPublicProfile(userId: string) {
    if (!userId) return;
    this.tourismUserService.getPublicProfile(userId).subscribe({
      next: (profile) => {
        this.selectedUserProfile = profile;
        this.showProfileModal = true;
      },
      error: (err) => {
        this.toaster.error('No se pudo cargar el perfil del usuario.');
        console.error(err);
      }
    });
  }

  closeProfileModal() {
    this.showProfileModal = false;
    this.selectedUserProfile = null;
  }
}
