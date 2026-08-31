import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { TourismInteractionService } from '../../proxy/experiences/tourism-interaction.service';
import { DestinationDto } from '../../proxy/destinations/models';
import { RouterModule } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-list-destinations',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './list-destinations.component.html',
  styleUrl: './list-destinations.component.scss'
})
export class ListDestinationsComponent implements OnInit {
  searchQuery = '';
  countryFilter = '';
  regionFilter = '';
  minPopulationFilter: number | null = null;
  searchResults: DestinationDto[] = [];
  popularDestinations: DestinationDto[] = [];
  isSearching = false;
  hasSearched = false;

  // Map de métricas por destination ID
  destinationRatings: { [id: string]: { avg: number; total: number } } = {};

  // Imagen escénica de respaldo confiable
  readonly defaultFallbackImage = 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?auto=format&fit=crop&q=80&w=800';

  constructor(
    private destinationService: DestinationService,
    private interactionService: TourismInteractionService,
    private toaster: ToasterService
  ) { }

  ngOnInit(): void {
    this.loadPopularDestinations();
  }

  loadPopularDestinations() {
    this.destinationService.getSavedDestinations().subscribe((res) => {
      this.popularDestinations = res;
      // Cargar métricas dinámicas para cada destino
      res.forEach(dest => {
        if (dest.id) {
          this.interactionService.getDestinationAverageRating(dest.id).subscribe(metrics => {
            this.destinationRatings[dest.id!] = {
              avg: metrics.averageRating,
              total: metrics.totalReviews
            };
          });
        }
      });
    });
  }

  getDestinationRating(id: string | undefined): string {
    if (!id || !this.destinationRatings[id]) return 'Sin reseñas';
    const r = this.destinationRatings[id];
    return `⭐ ${r.avg.toFixed(1)} (${r.total} reseñas)`;
  }

  search() {
    if (!this.searchQuery.trim()) return;
    this.isSearching = true;
    this.hasSearched = false;
    this.destinationService.searchExternalDestinations(
      this.searchQuery.trim(),
      this.countryFilter?.trim() || null,
      this.regionFilter?.trim() || null,
      this.minPopulationFilter || null
    ).subscribe({
      next: (res) => {
        this.searchResults = res;
        this.isSearching = false;
        this.hasSearched = true;
      },
      error: () => {
        this.isSearching = false;
        this.hasSearched = true;
        this.toaster.error('Ocurrió un error al buscar destinos externos. Intenta nuevamente.');
      }
    });
  }

  onImgError(event: any) {
    event.target.src = this.defaultFallbackImage;
  }

  saveDestination(dest: DestinationDto) {
    const finalImage = dest.imageUrl || this.defaultFallbackImage;
    this.destinationService.saveDestinationToInternalDb({
      name: dest.name,
      country: dest.country,
      population: dest.population,
      latitude: dest.latitude,
      longitude: dest.longitude,
      imageUrl: finalImage
    }).subscribe({
      next: () => {
        this.toaster.success(`${dest.name} guardado con éxito.`);
        this.loadPopularDestinations();
        this.searchResults = [];
        this.hasSearched = false;
      },
      error: () => {
        this.toaster.error(`No se pudo guardar el destino ${dest.name}.`);
      }
    });
  }
}
