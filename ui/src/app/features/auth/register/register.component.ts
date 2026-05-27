import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';

import { AuthService } from '../../../core/services/auth.services';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  name = ''
  email = '';
  password = '';
  errorMessage = '';

  onRegister() {
    const RegisterData = {
      name: this.name,
      email: this.email,
      password: this.password
    };

    this.authService.register(RegisterData).subscribe({
      next: (response) => {
        console.log(response);
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.log(error);
        this.errorMessage = error.error.message;
      }

    });
  }
}
