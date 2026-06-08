import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf, NgFor, DecimalPipe } from '@angular/common';
import { BookService } from '../../../core/services/book.services';
import { Book } from '../../../core/models/books/book.model';


@Component({
  selector: 'app-customer-books',
  imports: [FormsModule, NgIf, NgFor, DecimalPipe],
  templateUrl: './customer-books.component.html',
  styleUrl: './customer-books.component.css'
})
export class CustomerBooksComponent implements OnInit {
  private bookService = inject(BookService);

  books: Book[] = [];
  searchQuery = '';
  genreFilter = '';
  errorMessage = '';

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks() {
    this.bookService.getBooks().subscribe({
      next: (response) => {
        this.books = response.data;
      },
      error: (error) => {
        this.errorMessage = error.error?.message ?? 'Failed to load books';
      }
    });
  }

  search() {
    if (!this.searchQuery && !this.genreFilter) {
      this.loadBooks();
      return;
    }
    this.bookService.searchBooks({
      query: this.searchQuery,
      genre: this.genreFilter
    }).subscribe({
      next: (response) => {
        this.books = response.data;
      },
      error: (error) => {
        this.errorMessage = error.error?.message ?? 'Searched Failed';
      }
    });
  }

  clearSearch() {
    this.searchQuery = '';
    this.genreFilter = '';
    this.loadBooks();
  }

  getStars(rating: number): string {
    return '★'.repeat(Math.round(rating));
  }

  getEmptyStars(rating: number): string {
    return '★'.repeat(5 - Math.round(rating));
  }
}
