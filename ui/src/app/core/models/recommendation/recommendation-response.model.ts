import { Book } from "../books/book.model";
import { RecommendationBook } from "./recommendation-book.model";

export interface RecommendationResponse {
    internalRecommendations: Book[],
    externalRecommendations: RecommendationBook[]
}