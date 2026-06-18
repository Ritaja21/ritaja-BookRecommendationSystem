import { NgFor, NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule, TouchedChangeEvent } from '@angular/forms';
import { RecommendationService } from '../../../core/services/recommendation.services';
import { RecommendationResponse } from '../../../core/models/recommendation/recommendation-response.model';
import { RecommendationRequest } from '../../../core/models/recommendation/recommendation-request.model';

@Component({
  selector: 'app-recommendation',
  imports: [FormsModule, NgIf, NgFor],
  templateUrl: './recommendation.component.html',
  styleUrl: './recommendation.component.css'
})
export class RecommendationComponent {
  private recService = inject(RecommendationService);

  prompt = '';
  genre = ''; 
  minimumRating: number | undefined = undefined;
  response: RecommendationResponse | null = null;
  isLoading = false;
  errorMessage = '';

  getRecommendation() {
    if (!this.prompt && !this.genre) {
      this.errorMessage = 'Please enter a prompt or select a genre';
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';
    this.response = null;

    const request: RecommendationRequest = {
      prompt: this.prompt,
      genre: this.genre,
      minimumRating: this.minimumRating
    };

    this.recService.getRecommendation(request).subscribe({
      next: (res) => {
        this.response = res.data;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message ?? 'Failed to get recommendations.';
        this.isLoading = false;
      }
    });
  }

  getStars(rating: number): string {
    return '★'.repeat(Math.round(rating));
  }

  getEmptyStars(rating: number): string {
    return '★'.repeat(5 - Math.round(rating));
  }
}

