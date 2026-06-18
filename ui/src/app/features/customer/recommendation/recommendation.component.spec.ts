import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';

import { RecommendationComponent } from './recommendation.component';
import { RecommendationService } from '../../../core/services/recommendation.services';

describe('RecommendationComponent', () => {

    let component: RecommendationComponent;
    let fixture: ComponentFixture<RecommendationComponent>;

    let recommendationServiceSpy: jasmine.SpyObj<RecommendationService>;

    beforeEach(async () => {

        recommendationServiceSpy = jasmine.createSpyObj(
            'RecommendationService',
            ['getRecommendation']
        );

        await TestBed.configureTestingModule({
            imports: [RecommendationComponent],
            providers: [
                {
                    provide: RecommendationService,
                    useValue: recommendationServiceSpy
                }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(
            RecommendationComponent
        );

        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should show validation error when prompt and genre are empty', () => {

        component.prompt = '';
        component.genre = '';

        component.getRecommendation();

        expect(component.errorMessage)
            .toBe('Please enter a prompt or select a genre');
    });

    it('should call recommendation service successfully', () => {

        const mockData = {
            internalRecommendations: [],
            externalRecommendations: []
        };

        const mockResponse = {
            data: mockData
        };

        recommendationServiceSpy.getRecommendation
            .and.returnValue(of(mockResponse as any));

        component.prompt = 'Thriller';

        component.getRecommendation();

        expect(
            recommendationServiceSpy.getRecommendation
        ).toHaveBeenCalled();

        expect(component.response)
            .toEqual(mockData as any);

        expect(component.isLoading)
            .toBeFalse();

        expect(component.errorMessage)
            .toBe('');
    });

    it('should handle recommendation service error', () => {

        recommendationServiceSpy.getRecommendation
            .and.returnValue(
                throwError(() => ({
                    error: {
                        message: 'API Failed'
                    }
                }))
            );

        component.prompt = 'Thriller';

        component.getRecommendation();

        expect(component.errorMessage)
            .toBe('API Failed');

        expect(component.isLoading)
            .toBeFalse();
    });

    it('should return filled stars correctly', () => {

        const result = component.getStars(4);

        expect(result).toBe('★★★★');
    });

    it('should return empty stars correctly', () => {

        const result = component.getEmptyStars(4);

        expect(result).toBe('★');
    });

    it('should send correct request object to service', () => {

        const mockResponse = {
            data: {
                internalRecommendations: [],
                externalRecommendations: []
            }
        };

        recommendationServiceSpy.getRecommendation
            .and.returnValue(of(mockResponse as any));

        component.prompt = 'Fantasy';
        component.genre = 'Fiction';
        component.minimumRating = 4;

        component.getRecommendation();

        expect(
            recommendationServiceSpy.getRecommendation
        ).toHaveBeenCalledWith({
            prompt: 'Fantasy',
            genre: 'Fiction',
            minimumRating: 4
        });
    });

});