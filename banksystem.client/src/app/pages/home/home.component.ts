import { Component, OnInit } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { AuthorizeService } from '../Authorize/service/authorize.service';
import { CommonModule } from '@angular/common';



@Component({
  standalone: true,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatButtonModule,
    RouterModule,
    MatCardModule,
    CommonModule,
    RouterModule
  ]
})
export class HomeComponent {
  username: string | null = null;
  showTermsPopup: boolean = false;
  role: string | null = null;

  constructor(
    private authService: AuthorizeService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.username = this.authService.getUsernameFromToken();
    const accepted = localStorage.getItem('termsAccepted');
    if (!accepted) {
      this.showTermsPopup = true;
    }
    this.role = this.authService.getRoleFromToken();
  }

  isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

  acceptTerms(): void {
    localStorage.setItem('termsAccepted', 'true');
    this.showTermsPopup = false;
  }

  rejectTerms(): void {
    alert('You must accept the Terms and Conditions to use the platform.');
    this.router.navigate(['/']); // Optional: adjust to a landing or exit route
  }
  isAdminIn(): boolean {
    if (this.role == 'admin') {
      return true
    }
    return false; 
  }
}
