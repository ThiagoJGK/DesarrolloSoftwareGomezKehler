
export interface DestinationDto {
  id?: string;
  name?: string;
  country?: string;
  population: number;
  latitude: number;
  longitude: number;
  imageUrl?: string;
  lastExternalUpdate?: string;
}

export interface SaveDestinationInput {
  name: string;
  country: string;
  population: number;
  latitude: number;
  longitude: number;
  imageUrl?: string;
}
