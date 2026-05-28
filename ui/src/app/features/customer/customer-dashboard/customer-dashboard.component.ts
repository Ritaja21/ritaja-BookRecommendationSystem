import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../../core/services/auth.services';

@Component({
  selector: 'app-customer-dashboard',
  imports: [RouterLink, NgIf],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.css'
})
export class CustomerDashboardComponent {
  private authService = inject(AuthService);

  user = this.authService.getUser();

  getInitials(): string {
    if (!this.user?.name) return '?';
    return this.user.name.split(' ')
      .map((n: string) => n[0]).join('').toUpperCase().slice(0, 2);
  }

  getFirstName(): string {
    return this.user?.name?.split(' ')[0] ?? 'there';
  }
}