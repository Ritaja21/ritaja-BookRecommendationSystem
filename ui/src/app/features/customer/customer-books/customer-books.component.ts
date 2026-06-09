import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf, NgFor, DecimalPipe } from '@angular/common';
import { BookService } from '../../../core/services/book.services';
import { Book } from '../../../core/models/books/book.model';
import { UserService } from '../../../core/services/user.services';


@Component({
  selector: 'app-customer-books',
  imports: [FormsModule, NgIf, NgFor, DecimalPipe],
  templateUrl: './customer-books.component.html',
  styleUrl: './customer-books.component.css'
})
export class CustomerBooksComponent implements OnInit {
  private bookService = inject(BookService);
  private userService = inject(UserService);

  books: Book[] = [];
  history: number[] = [];
  searchQuery = '';
  genreFilter = '';
  errorMessage = '';

  ngOnInit(): void {
    this.loadBooks();
    this.loadHistory();
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

  loadHistory() {
    this.userService.getHistory().subscribe({
      next: (response) => {

        this.history = response.data
          .filter(x => x.isRead)
          .map(x => x.bookId);

      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  markAsRead(bookId: number) {

    this.userService.markAsRead({ bookId }).subscribe({

      next: () => {

        this.history.push(bookId);

      },

      error: (error) => {
        console.log(error);
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
