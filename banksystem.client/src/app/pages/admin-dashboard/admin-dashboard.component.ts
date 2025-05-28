import { Component, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { User } from '../user/models/user.models';
import { AdminService } from './services/admin.service';
import { UserProfilesModel } from './models/user-profiles.model';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css',
  imports: [
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTooltipModule,
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    MatProgressSpinnerModule

  ]
})
export class AdminDashboardComponent implements OnInit {
  registerForm!: FormGroup;
  users: User[] = [];
  errorMessage: string | null = null;
  isLoading = false;

  constructor(
    private adminService: AdminService,
    private fb: FormBuilder,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadUsers();
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      dateOfBirth: ['', Validators.required],
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      role: ['', Validators.required],
    });
  }

  loadUsers(): void {
    this.adminService.loadUsers()
      .then((response: User[]) => {
        console.log('ITS HERE: ' + response);
        this.users = response;
        console.log(this.users);
      })
      .catch(error => {
        console.error('Error loading user profile:', error);
      });
    
  }


  createUser(): void {
    if (this.registerForm.valid) {
      const formValues = this.registerForm.value;

      if (formValues.password !== formValues.confirmPassword) {
        this.errorMessage = 'Passwords do not match';
        return;
      }

      this.isLoading = true;
      this.errorMessage = null;

      this.adminService.createAdvancedUser(formValues)
        .then(() => {
        })
        .catch(error => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Registration failed. Please try again.';
        });
    }
  }

  deactivateUser(userId: number): void {
    this.adminService.deactivateUser(userId)
      .then(() => {
        this.loadUsers()
      })
      .catch(error => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Deactivation failed. Please try again.';
      });
  }
  activateUser(userId: number): void {
    this.adminService.activateUser(userId)
      .then(() => {
        this.loadUsers()
      })
      .catch(error => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Deactivation failed. Please try again.';
      });
  }
  twoFactor(userId: number): void {
    this.adminService.twoFactor(userId)
      .then(() => {
        this.loadUsers()
      })
      .catch(error => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Deactivation failed. Please try again.';
      });
  }

}
