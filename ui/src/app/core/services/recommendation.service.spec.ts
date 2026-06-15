import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { RecommendationService } from './recommendation.services';
import { environment } from '../../../environments/environment';

describe('RecommendationService', () => {
    let service: RecommendationService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        });

        service = TestBed.inject(RecommendationService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should call recommendation API using POST', () => {
        const mockRequest = {
            prompt: 'Mystery with Historical Reference',
            genre: 'Mystery',
            minimumRating: 4
        };

        service.getRecommendation(mockRequest).subscribe();

        const req = httpMock.expectOne(
            `${environment.apiUrl}/recommendation`
        );

        expect(req.request.method).toBe('POST');
    });

});