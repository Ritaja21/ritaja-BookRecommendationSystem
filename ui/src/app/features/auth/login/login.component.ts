import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';

import { AuthService } from '../../../core/services/auth.services';


@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  email = '';
  password = '';
  errorMessage = '';

  onLogin() {
    const loginData = {
      email: this.email,
      password: this.password
    };

    this.authService.login(loginData).subscribe({
      next: (response) => {
        console.log(response);

        const user = response.data.userDTO;

        if (user.role === 'Admin') {
          this.router.navigate(['/admin/dashboard']);
        }
        else {
          this.router.navigate(['/customer/dashboard']);
        }

      },
      error: (error) => {
        console.log(error);
        this.errorMessage = error.error.message;
      }

    });
  }

}
