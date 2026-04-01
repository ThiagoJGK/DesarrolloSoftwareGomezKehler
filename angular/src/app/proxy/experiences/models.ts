
export interface DestinationMetricsDto {
  averageRating: number;
  totalReviews: number;
}

export interface ExperienceDto {
  id?: string;
  title?: string;
  content?: string;
  keywords?: string;
}

export interface ReviewDto {
  id?: string;
  rating: number;
  comment?: string;
  userId?: string;
}
