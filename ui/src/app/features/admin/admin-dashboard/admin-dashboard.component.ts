import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.services';
import { UserService } from '../../../core/services/user.services';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css'
})
export class AdminDashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private userService = inject(UserService);

  user: any = null;

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile() {
    this.userService.getProfile().subscribe({
      next: (response) => {
        this.user = response.data;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  getInitials(): string {
    if (!this.user?.name) return '?';

    return this.user.name
      .split(' ')
      .map((n: string) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }
}