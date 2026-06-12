import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';

import { ApiResponse } from '../models/auth/api-response.model';
import { User } from '../models/auth/user.model';
import { UserUpdate } from '../models/auth/userupdate.model';
import { UserHistory } from "../models/books/user-history.model";
import { UserRead } from '../models/books/user-read.model';
import { RateBook } from '../models/books/user-rate.model';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    private http = inject(HttpClient);

    private apiUrl = environment.apiUrl;

    getProfile() {
        return this.http.get<ApiResponse<User>>(
            `${this.apiUrl}/user/profile`
        );
    }

    updateProfile(data: UserUpdate) {
        return this.http.patch<ApiResponse<User>>(
            `${this.apiUrl}/user/profile`,
            data
        );
    }

    getHistory() {
        return this.http.get<ApiResponse<UserHistory[]>>(
            `${this.apiUrl}/user/history`
        )
    }

    markAsRead(data: UserRead) {
        return this.http.post(
            `${this.apiUrl}/user/read`,
            data
        );
    }

    rateBook(data: RateBook) {
        return this.http.post(
            `${this.apiUrl}/user/rate`,
            data
        );
    }


}