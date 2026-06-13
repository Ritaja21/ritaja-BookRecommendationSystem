import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.services';
import { UserService } from '../../../core/services/user.services';

@Component({
  selector: 'app-customer-dashboard',
  imports: [RouterLink, NgIf, NgFor, FormsModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.css'
})
export class CustomerDashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private userService = inject(UserService);

  user: any = null;
  history: any[] = [];
  isEditing = false;
  editedName = "";

  ngOnInit(): void {
    this.loadProfile();
    this.loadHistory();
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

  startEdit() {
    this.editedName = this.user.name;
    this.isEditing = true;
  }

  cancelEdit() {
    this.isEditing = false;
  }

  saveProfile() {
    const updateData = {
      name: this.editedName
    };

    this.userService.updateProfile(updateData).subscribe({
      next: (response) => {
        this.user = response.data;

        localStorage.setItem(
          'user',
          JSON.stringify(response.data)
        );

        this.isEditing = false;
      },

      error: (error) => {
        console.log(error);
      }
    });

  }

  loadHistory() {
    this.userService.getHistory().subscribe({
      next: (response) => {
        this.history = response.data;
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

  getStars(rating: number): string {
    return '★'.repeat(Math.round(rating));
  }

  getEmptyStars(rating: number): string {
    return '★'.repeat(5 - Math.round(rating));
  }

}