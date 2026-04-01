import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TourismInteractionService } from '../../proxy/experiences/tourism-interaction.service';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { ExperienceDto } from '../../proxy/experiences/models';
import { DestinationDto } from '../../proxy/destinations/models';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
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
    // Como la API no tiene getMyExperiences directo, pediriamos los destinos y mapeariamos,
    // o pediremos getAll, o como el ABP AppService lo requiera.
    // Como workaround traemos las experiencias que podamos si tuvieramos un endpoint.
    // Simularemos la recolección para la maqueta:
    this.myExperiences = [];
  }

  loadMyFavorites() {
    this.interactionService.getMyFavoriteDestinations().subscribe(favIds => {
      this.myFavoritesIds = favIds;
      // Cargar destinos para cruzar data
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
}
