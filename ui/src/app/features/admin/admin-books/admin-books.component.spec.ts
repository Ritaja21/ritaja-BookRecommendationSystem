import { ComponentFixture, TestBed } from "@angular/core/testing";
import { AdminBooksComponent } from "./admin-books.component";
import { BookService } from "../../../core/services/book.services";
import { of, throwError } from 'rxjs';

describe('AdminBooksComponent', () => {

    let component: AdminBooksComponent;
    let fixture: ComponentFixture<AdminBooksComponent>;

    let bookServiceSpy: jasmine.SpyObj<BookService>;

    beforeEach(async () => {

        bookServiceSpy = jasmine.createSpyObj(
            'BookService',
            ['getBooks', 'createBook', 'updateBook', 'deleteBook']
        );

        bookServiceSpy.getBooks.and.returnValue(
            of({
                data: []
            } as any)
        );

        await TestBed.configureTestingModule({
            imports: [AdminBooksComponent],
            providers: [
                {
                    provide: BookService,
                    useValue: bookServiceSpy
                }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(AdminBooksComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should call loadBooks on init', () => {
        spyOn(component, 'loadBooks');

        component.ngOnInit();

        expect(component.loadBooks).toHaveBeenCalled();
    });

    it('should load books successfully', () => {

        const mockResponse = {
            data: [
                {
                    bookId: 1,
                    title: 'Harry Potter',
                    author: 'J.K Rowling',
                    genre: 'Fantasy'
                }
            ]
        };

        bookServiceSpy.getBooks.and.returnValue(
            of(mockResponse as any)
        );

        component.loadBooks();

        expect(component.books.length).toBe(1);
        expect(component.filteredBooks.length).toBe(1);
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

    it('should filter books by search query', () => {

        component.books = [
            {
                bookId: 1,
                title: 'Harry Potter',
                author: 'JK Rowling'
            } as any,
            {
                bookId: 2,
                title: 'Atomic Habits',
                author: 'James Clear'
            } as any
        ];

        component.searchQuery = 'Harry';

        component.filterBooks();

        expect(component.filteredBooks.length)
            .toBe(1);
    });

    it('should open add form', () => {

        component.openAddForm();

        expect(component.showForm)
            .toBeTrue();

        expect(component.isEditing)
            .toBeFalse();

        expect(component.editingId)
            .toBeNull();
    });

    it('should open edit form', () => {

        const book = {
            bookId: 5,
            title: 'Harry Potter',
            author: 'JK Rowling',
            genre: 'Fantasy',
            description: 'Magic'
        } as any;

        component.openEditForm(book);

        expect(component.isEditing)
            .toBeTrue();

        expect(component.editingId)
            .toBe(5);

        expect(component.formData.title)
            .toBe('Harry Potter');
    });

    it('should create book successfully', () => {

        component.isEditing = false;

        component.formData = {
            title: 'New Book',
            author: 'Author',
            genre: '',
            description: ''
        };

        bookServiceSpy.createBook.and.returnValue(
            of({} as any)
        );

        spyOn(component, 'loadBooks');

        component.saveBook();

        expect(bookServiceSpy.createBook)
            .toHaveBeenCalled();

        expect(component.loadBooks)
            .toHaveBeenCalled();
    });

    it('should update book successfully', () => {

        component.isEditing = true;
        component.editingId = 1;

        component.formData = {
            title: 'Updated Book',
            author: 'Author',
            genre: '',
            description: ''
        };

        bookServiceSpy.updateBook.and.returnValue(
            of({} as any)
        );

        spyOn(component, 'loadBooks');

        component.saveBook();

        expect(bookServiceSpy.updateBook)
            .toHaveBeenCalled();

        expect(component.loadBooks)
            .toHaveBeenCalled();
    });

    it('should show validation error when title is missing', () => {

        component.formData = {
            title: '',
            author: '',
            genre: '',
            description: ''
        };

        component.saveBook();

        expect(component.errorMessage)
            .toBe('Title and Author are required.');
    });

    it('should delete book successfully', () => {

        spyOn(window, 'confirm')
            .and.returnValue(true);

        bookServiceSpy.deleteBook.and.returnValue(
            of({} as any)
        );

        spyOn(component, 'loadBooks');

        component.deleteBook(1);

        expect(bookServiceSpy.deleteBook)
            .toHaveBeenCalledWith(1);

        expect(component.loadBooks)
            .toHaveBeenCalled();
    });

    it('should cancel form', () => {

        component.showForm = true;

        component.cancelForm();

        expect(component.showForm)
            .toBeFalse();
    });

});