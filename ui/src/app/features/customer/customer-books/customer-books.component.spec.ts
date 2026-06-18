import { ComponentFixture, TestBed } from "@angular/core/testing";
import { CustomerBooksComponent } from "./customer-books.component";
import { BookService } from "../../../core/services/book.services";
import { UserService } from "../../../core/services/user.services";
import { of, throwError } from 'rxjs';

describe('CustomerBooksComponent', () => {

    let component: CustomerBooksComponent;
    let fixture: ComponentFixture<CustomerBooksComponent>;

    let bookServiceSpy: jasmine.SpyObj<BookService>;
    let userServiceSpy: jasmine.SpyObj<UserService>;

   beforeEach(async () => {

  bookServiceSpy = jasmine.createSpyObj(
    'BookService',
    ['getBooks', 'searchBooks']
  );

  userServiceSpy = jasmine.createSpyObj(
    'UserService',
    ['getHistory', 'markAsRead', 'rateBook']
  );

  bookServiceSpy.getBooks.and.returnValue(
    of({ data: [] } as any)
  );

  userServiceSpy.getHistory.and.returnValue(
    of({ data: [] } as any)
  );

  await TestBed.configureTestingModule({
    imports: [CustomerBooksComponent],
    providers: [
      {
        provide: BookService,
        useValue: bookServiceSpy
      },
      {
        provide: UserService,
        useValue: userServiceSpy
      }
    ]
  }).compileComponents();

  fixture = TestBed.createComponent(CustomerBooksComponent);
  component = fixture.componentInstance;
  fixture.detectChanges();
});

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should call loadBooks and loadHistory on init', () => {

        spyOn(component, 'loadBooks');
        spyOn(component, 'loadHistory');

        component.ngOnInit();

        expect(component.loadBooks).toHaveBeenCalled();
        expect(component.loadHistory).toHaveBeenCalled();
    });

    it('should load books successfully', () => {

        const mockResponse = {
            data: [
                {
                    bookId: 1,
                    title: 'Harry Potter'
                }
            ]
        };

        bookServiceSpy.getBooks.and.returnValue(
            of(mockResponse as any)
        );

        component.loadBooks();

        expect(component.books.length).toBe(1);
        expect(component.books[0].title).toBe('Harry Potter');
    });

    it('should handle loadBooks error', () => {

        bookServiceSpy.getBooks.and.returnValue(
            throwError(() => ({
                error: {
                    message: 'Failed to load books'
                }
            }))
        );

        component.loadBooks();

        expect(component.errorMessage)
            .toBe('Failed to load books');
    });


    it('should load user history correctly', () => {

        const mockResponse = {
            data: [
                {
                    bookId: 1,
                    isRead: true,
                    rating: null
                },
                {
                    bookId: 2,
                    isRead: true,
                    rating: 5
                }
            ]
        };

        userServiceSpy.getHistory.and.returnValue(
            of(mockResponse as any)
        );

        component.loadHistory();

        expect(component.history)
            .toEqual([1, 2]);

        expect(component.ratedbooks)
            .toEqual([2]);
    });

    it('should mark book as read', () => {

        userServiceSpy.markAsRead.and.returnValue(
            of({})
        );

        component.markAsRead(10);

        expect(
            userServiceSpy.markAsRead
        ).toHaveBeenCalledWith({
            bookId: 10
        });

        expect(component.history)
            .toContain(10);
    });

    it('should search books', () => {

        const mockResponse = {
            data: []
        };

        bookServiceSpy.searchBooks.and.returnValue(
            of(mockResponse as any)
        );

        component.searchQuery = 'Harry';

        component.search();

        expect(
            bookServiceSpy.searchBooks
        ).toHaveBeenCalled();
    });

    it('should open rating modal', () => {

        component.openRatingModal(5);

        expect(component.selectedBookId)
            .toBe(5);

        expect(component.showRatingModal)
            .toBeTrue();
    });

    it('should submit rating successfully', () => {

        userServiceSpy.rateBook.and.returnValue(
            of({})
        );

        spyOn(component, 'loadBooks');
        spyOn(component, 'loadHistory');

        component.selectedBookId = 1;
        component.selectedRating = 5;

        component.submitRating();

        expect(
            userServiceSpy.rateBook
        ).toHaveBeenCalledWith({
            bookId: 1,
            rating: 5
        });

        expect(component.showRatingModal)
            .toBeFalse();

        expect(component.loadBooks)
            .toHaveBeenCalled();

        expect(component.loadHistory)
            .toHaveBeenCalled();
    });

    it('should not submit rating when rating is invalid', () => {

        component.selectedBookId = 1;
        component.selectedRating = 0;

        component.submitRating();

        expect(
            userServiceSpy.rateBook
        ).not.toHaveBeenCalled();
    });

});
