import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { DestinationDto } from '../../proxy/destinations/models';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-list-destinations',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './list-destinations.component.html',
  styleUrl: './list-destinations.component.scss'
})
export class ListDestinationsComponent implements OnInit {
  searchQuery = '';
  searchResults: DestinationDto[] = [];
  popularDestinations: DestinationDto[] = [];
  isSearching = false;

  constructor(private destinationService: DestinationService) { }

  ngOnInit(): void {
    this.loadPopularDestinations();
  }

  loadPopularDestinations() {
    this.destinationService.getSavedDestinations().subscribe((res) => {
      this.popularDestinations = res;
    });
  }

  search() {
    if (!this.searchQuery) return;
    this.isSearching = true;
    this.destinationService.searchExternalDestinations(this.searchQuery, null).subscribe((res) => {
      this.searchResults = res;
      this.isSearching = false;
    });
  }

  saveDestination(dest: any) {
    this.destinationService.saveDestinationToInternalDb({
      name: dest.name,
      country: dest.country,
      population: dest.population,
      latitude: dest.latitude,
      longitude: dest.longitude,
      imageUrl: dest.imageUrl || 'https://images.unsplash.com/photo-1488085061387-422e29b40080?auto=format&fit=crop&q=80&w=800'
    }).subscribe(() => {
      alert(`${dest.name} guardado como destino popular.`);
      this.loadPopularDestinations();
      this.searchResults = [];
    });
  }
}
