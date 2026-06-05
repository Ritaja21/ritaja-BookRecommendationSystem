import { NgFor, NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../../core/services/book.services';
import { Book } from '../../../core/models/books/book.model';
import { CreateBook } from '../../../core/models/books/createbook.model';
import { UpdateBook } from '../../../core/models/books/updatebook.model';

@Component({
  selector: 'app-admin-books',
  imports: [FormsModule, NgIf, NgFor],
  templateUrl: './admin-books.component.html',
  styleUrl: './admin-books.component.css'
})
export class AdminBooksComponent implements OnInit {
  private bookService = inject(BookService);

  books: Book[] = [];
  filteredBooks: Book[] = [];
  searchQuery = '';
  selectedGenre = '';
  errorMessage = '';
  successMessage = '';

  //form state
  showForm = false;
  isEditing = false;
  editingId: number | null = null;

  formData: CreateBook = {
    title: '',
    author: '',
    genre: '',
    description: ''
  };

  ngOnInit() {
    this.loadBooks();
  }

  loadBooks() {
    this.bookService.getBooks().subscribe({
      next: (response) => {
        this.books = response.data;
        this.filteredBooks = this.books;
      },
      error: (error) => {
        this.errorMessage = error.error?.message ?? 'Failed to load books';
      }
    });
  }

  filterBooks() {
    this.filteredBooks = this.books.filter(book => {
      const matchesSearch =
        book.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        book.author.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchesGenre = this.selectedGenre
        ? book.genre?.toLowerCase().includes(this.selectedGenre.toLowerCase())
        : true;
      return matchesSearch && matchesGenre;
    });
  }

  openAddForm() {
    this.isEditing = false;
    this.editingId = null;
    this.formData = { title: '', author: '', genre: '', description: '' };
    this.showForm = true;
    this.clearMessages();
  }

  openEditForm(book: Book) {
    console.log('book object:', book);  // add this
    this.isEditing = true;
    this.editingId = book.bookId;
    this.formData = {
      title: book.title,
      author: book.author,
      genre: book.genre ?? '',
      description: book.description ?? ''
    };
    this.showForm = true;
    this.clearMessages();
  }

  saveBook() {
    if (!this.formData.title || !this.formData.author) {
      this.errorMessage = 'Title and Author are required.';
      return;
    }

    if (this.isEditing && this.editingId !== null) {
      const updateData: UpdateBook = { id: this.editingId, ...this.formData };
      this.bookService.updateBook(this.editingId, updateData).subscribe({
        next: () => {
          this.successMessage = 'Book updated successfully.';
          this.showForm = false;
          this.loadBooks();
        },
        error: (error) => {
          this.errorMessage = error.error?.message ?? 'Failed to update book.';
        }
      });
    } else {
      this.bookService.createBook(this.formData).subscribe({
        next: () => {
          this.successMessage = 'Book added successfully.';
          this.showForm = false;
          this.loadBooks();
        },
        error: (error) => {
          this.errorMessage = error.error?.message ?? 'Failed to add book.';
        }
      });
    }
  }

  deleteBook(id: number) {
    if (!confirm('Are you sure you want to delete this book?')) return;
    this.bookService.deleteBook(id).subscribe({
      next: () => {
        this.successMessage = 'Book deleted successfully.';
        this.loadBooks();
      },
      error: (error) => {
        this.errorMessage = error.error?.message ?? 'Failed to delete book.';
      }
    });
  }

  cancelForm() {
    this.showForm = false;
    this.clearMessages();
  }

  clearMessages() {
    this.errorMessage = '';
    this.successMessage = '';
  }

  getStars(rating: number): string {
    return '★'.repeat(Math.round(rating));
  }

  getEmptyStars(rating: number): string {
    return '★'.repeat(5 - Math.round(rating));
  }
}
