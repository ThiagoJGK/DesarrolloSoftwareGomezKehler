import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { DestinationService } from '../../proxy/destinations/destination.service';
import { TourismInteractionService } from '../../proxy/experiences/tourism-interaction.service';
import { DestinationDto } from '../../proxy/destinations/models';

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
  isAdvancedFiltersOpen = false;
  searchResults: DestinationDto[] = [];
  popularDestinations: DestinationDto[] = [];
  isSearching = false;
  hasSearched = false;

  favoriteIds: Set<string> = new Set<string>();

  toggleAdvancedFilters(): void {
    this.isAdvancedFiltersOpen = !this.isAdvancedFiltersOpen;
  }

  clearFilters(): void {
    this.countryFilter = '';
    this.regionFilter = '';
    this.minPopulationFilter = null;
  }

  get hasActiveFilters(): boolean {
    return !!(this.countryFilter?.trim() || this.regionFilter?.trim() || this.minPopulationFilter);
  }

  // Map de métricas por destination ID
  destinationRatings: { [id: string]: { avg: number; total: number } } = {};

  // Imagen escénica de respaldo confiable
  readonly defaultFallbackImage = 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?auto=format&fit=crop&q=80&w=800';

  constructor(
    private destinationService: DestinationService,
    private interactionService: TourismInteractionService,
    private toaster: ToasterService,
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadPopularDestinations();
    this.loadFavorites();
  }

  loadFavorites() {
    if (this.authService.isAuthenticated) {
      this.interactionService.getMyFavoriteDestinations().subscribe({
        next: (favIds) => {
          this.favoriteIds = new Set(favIds || []);
        },
        error: () => {}
      });
    }
  }

  isFavorite(dest: DestinationDto): boolean {
    return !!dest.id && this.favoriteIds.has(dest.id);
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

  exploreDestination(dest: DestinationDto) {
    if (dest.id) {
      this.router.navigate(['/tourism/destination', dest.id]);
      return;
    }
    const finalImage = dest.imageUrl || this.defaultFallbackImage;
    this.destinationService.saveDestinationToInternalDb({
      name: dest.name!,
      country: dest.country!,
      population: dest.population,
      latitude: dest.latitude,
      longitude: dest.longitude,
      imageUrl: finalImage
    }).subscribe({
      next: (saved) => {
        dest.id = saved.id;
        this.loadPopularDestinations();
        this.router.navigate(['/tourism/destination', saved.id]);
      },
      error: () => {
        this.toaster.error('No se pudo acceder a la ficha del destino.');
      }
    });
  }

  toggleFavorite(dest: DestinationDto, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    if (!this.authService.isAuthenticated) {
      this.toaster.info('Inicia sesión para gestionar tus destinos favoritos.');
      return;
    }

    if (dest.id) {
      const destId = dest.id;
      if (this.favoriteIds.has(destId)) {
        // Instant visual feedback
        this.favoriteIds.delete(destId);
        this.interactionService.removeFromFavorites(destId).subscribe({
          next: () => {
            this.toaster.info(`${dest.name} removido de tus favoritos.`);
          },
          error: () => {
            this.favoriteIds.add(destId);
            this.toaster.error('Error al remover de favoritos.');
          }
        });
      } else {
        // Instant visual feedback
        this.favoriteIds.add(destId);
        this.interactionService.addToFavorites(destId).subscribe({
          next: () => {
            this.toaster.success(`${dest.name} agregado a tus favoritos.`);
          },
          error: () => {
            this.favoriteIds.delete(destId);
            this.toaster.error('Error al agregar a favoritos.');
          }
        });
      }
    } else {
      // External item: auto-persist first, then toggle favorite
      const finalImage = dest.imageUrl || this.defaultFallbackImage;
      this.destinationService.saveDestinationToInternalDb({
        name: dest.name!,
        country: dest.country!,
        population: dest.population,
        latitude: dest.latitude,
        longitude: dest.longitude,
        imageUrl: finalImage
      }).subscribe({
        next: (saved) => {
          dest.id = saved.id;
          const newId = saved.id!;
          this.favoriteIds.add(newId);
          this.loadPopularDestinations();
          this.interactionService.addToFavorites(newId).subscribe({
            next: () => {
              this.toaster.success(`${dest.name} agregado a tus favoritos.`);
            },
            error: () => {
              this.favoriteIds.delete(newId);
              this.toaster.error('Error al agregar a favoritos.');
            }
          });
        },
        error: () => {
          this.toaster.error(`No se pudo procesar el destino ${dest.name}.`);
        }
      });
    }
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
      next: (saved) => {
        dest.id = saved.id;
        this.toaster.success(`${dest.name} guardado con éxito.`);
        this.loadPopularDestinations();
      },
      error: () => {
        this.toaster.error(`No se pudo guardar el destino ${dest.name}.`);
      }
    });
  }
}

