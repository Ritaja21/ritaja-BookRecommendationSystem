import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AuthService } from '../../../core/services/auth.services';
import { UserService } from '../../../core/services/user.services';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterLink, FormsModule, NgIf],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css',
  encapsulation: ViewEncapsulation.None
})
export class AdminDashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private userService = inject(UserService);

  user: any = null;
  isEditing = false;
  editedName = '';

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