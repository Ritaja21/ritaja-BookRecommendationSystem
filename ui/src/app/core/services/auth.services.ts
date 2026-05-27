import { inject, Inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { Observable, tap } from "rxjs";
import { environment } from "../../../environments/environment";

import { LoginRequest } from "../models/auth/login-request.model";
import { RegisterRequest } from "../models/auth/register-request.model";
import { LoginResponse } from "../models/auth/login-response.model";
import { User } from "../models/auth/user.model";
import { ApiResponse } from "../models/auth/api-response.model";


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);

  private apiUrl = environment.apiUrl;

  register(data: RegisterRequest): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(
      `${this.apiUrl}/auth/register`, data
    );
  }

  login(data: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiUrl}/auth/login`, data).pipe(tap((response) => {

        localStorage.setItem('token', response.data.token);
        localStorage.setItem('user', JSON.stringify(response.data.userDTO));
      })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getUser(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  getRole(): string | null {
    const user = this.getUser();
    return user?.role || null;
  }

}

