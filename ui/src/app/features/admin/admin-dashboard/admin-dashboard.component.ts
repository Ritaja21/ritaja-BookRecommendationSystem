import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.services';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css'
})
export class AdminDashboardComponent {
  private authService = inject(AuthService);

  user = this.authService.getUser();

  getInitials(): string {
    if (!this.user?.name) return '?';
    return this.user.name.split(' ')
      .map((n: string) => n[0]).join('').toUpperCase().slice(0, 2);
  }
}