import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { RecommendationRequest } from "../models/recommendation/recommendation-request.model";
import { ApiResponse } from "../models/auth/api-response.model";
import { RecommendationResponse } from "../models/recommendation/recommendation-response.model";

@Injectable({
    providedIn: "root"
})

export class RecommendationService {
    private http = inject(HttpClient);
    private apiUrl = environment.apiUrl;

    getRecommendation(data: RecommendationRequest) {
        return this.http.post<ApiResponse<RecommendationResponse>>(
            `${this.apiUrl}/api/recommedation`, data
        );
    }
}
