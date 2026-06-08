import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';

import { ApiResponse } from '../models/auth/api-response.model';
import { User } from '../models/auth/user.model';
import { UserUpdate } from '../models/auth/userupdate.model';

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

}