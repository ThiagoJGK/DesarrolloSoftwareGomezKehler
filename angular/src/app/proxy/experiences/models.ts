
export interface DestinationMetricsDto {
  averageRating: number;
  totalReviews: number;
}

export interface ExperienceDto {
  id?: string;
  destinationId?: string;
  userId?: string;
  title?: string;
  content?: string;
  keywords?: string;
  authorName?: string;
  authorUsername?: string;
  authorAvatar?: string;
}

export interface ReviewDto {
  id?: string;
  rating: number;
  comment?: string;
  userId?: string;
  destinationId?: string;
  authorName?: string;
  authorUsername?: string;
  authorAvatar?: string;
}

