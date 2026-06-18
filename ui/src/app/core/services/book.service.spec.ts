import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { BookService } from "./book.services"
import { TestBed } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment";

describe('BookService', () => {
    let service: BookService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        });

        service = TestBed.inject(BookService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should call GET/book', () => {
        service.getBooks().subscribe();
        const req = httpMock.expectOne(
            `${environment.apiUrl}/book`
        );

        expect(req.request.method).toBe('GET');
    });

    it('should call POST/book', () => {
        const mockBook = {
            title: 'Atomic Habits',
            author: 'James Clear',
            genre: 'Self Help',
            description: 'Good book'
        }
        service.createBook(mockBook).subscribe();
        const req = httpMock.expectOne(
            `${environment.apiUrl}/book`
        );

        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(mockBook);
    });

    it('should call PUT /book/:id', () => {

        const bookId = 1;

        const updateData = {
            id:1,
            title: 'Updated Title',
            author: 'James Clear'
        };

        service.updateBook(bookId, updateData).subscribe();

        const req = httpMock.expectOne(
            `${service['apiUrl']}/book/${bookId}`
        );

        expect(req.request.method).toBe('PUT');
        expect(req.request.body).toEqual(updateData);

    });

    it('should call DELETE /book/:id', () => {

        const bookId = 1;

        service.deleteBook(bookId).subscribe();

        const req = httpMock.expectOne(
            `${service['apiUrl']}/book/${bookId}`
        );

        expect(req.request.method).toBe('DELETE');

    });

    it('should call search endpoint with query parameters', () => {

        service.searchBooks({
            query: 'Atomic',
            author: 'James',
            genre: 'Self Help'
        }).subscribe();

        const req = httpMock.expectOne(
            `${service['apiUrl']}/book/search?query=Atomic&author=James&genre=Self%20Help`
        );

        expect(req.request.method).toBe('GET');

    });


})