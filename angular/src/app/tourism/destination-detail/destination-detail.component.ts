import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { DestinationDto } from '../../proxy/destinations/models';
import { TourismInteractionService } from '../../proxy/experiences/tourism-interaction.service';
import { ReviewDto, ExperienceDto, DestinationMetricsDto } from '../../proxy/experiences/models';

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

  constructor(
    private route: ActivatedRoute,
    private destinationService: DestinationService,
    private interactionService: TourismInteractionService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.destinationId = params.get('id') || '';
      if (this.destinationId) {
        this.loadDestinationDetails();
        this.loadMetricsAndReviews();
        this.loadExperiences();
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
    this.interactionService.getDestinationReviews(this.destinationId).subscribe(r => this.reviews = r);
  }

  loadExperiences() {
    this.interactionService.getExperiencesByDestination(this.destinationId).subscribe(e => this.experiences = e);
  }

  submitActionReview() {
    if (!this.newReview.comment) return;
    this.interactionService.addReview(this.destinationId, this.newReview.rating, this.newReview.comment).subscribe(() => {
      this.newReview.comment = '';
      this.loadMetricsAndReviews();
    });
  }

  submitActionExperience() {
    if (!this.newExperience.title || !this.newExperience.content) return;
    this.interactionService.createExperience(this.destinationId, this.newExperience.title, this.newExperience.content, this.newExperience.keywords).subscribe(() => {
      this.newExperience = { title: '', content: '', keywords: '' };
      this.loadExperiences();
    });
  }
}
