import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { ApiResponse } from "../models/auth/api-response.model";
import { Book } from "../models/books/book.model";
import { CreateBook } from "../models/books/createbook.model";
import { UpdateBook } from "../models/books/updatebook.model";

@Injectable({
  providedIn: 'root'
})
export class BookService {
    private http = inject(HttpClient)
    private apiUrl = environment.apiUrl;

    getBooks() {
        return this.http.get<ApiResponse<Book[]>>(
            `${this.apiUrl}/books`
        );
    }

    createBook(data: CreateBook) {
        return this.http.post<ApiResponse<Book>>(
            `${this.apiUrl}/books`,
            data
        );
    }
    updateBook(id: number, data: UpdateBook) {
        return this.http.put<ApiResponse<Book>>(
            `${this.apiUrl}/books/${id}`,
            data
        );
    }

    deleteBook(id: number) {
        return this.http.delete(`${this.apiUrl}/books/${id}`);
    }

}