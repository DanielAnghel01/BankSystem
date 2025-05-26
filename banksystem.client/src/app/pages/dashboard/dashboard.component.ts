import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
})
export class DashboardComponent {
  constructor(private router: Router) { }

  goToTransfer() {
    this.router.navigate(['/transfer']);
  }

  goToDeposit() {
    this.router.navigate(['/deposit']);
  }

  goToWithdraw() {
    this.router.navigate(['/withdraw']);
  }

  goToProfile() {
    this.router.navigate(['/user']);
  }

  goToExchange() {
    this.router.navigate(['/exchange-rates']);
  }

  goToTransactions() {
    this.router.navigate(['/transaction']);
  }

  goHome() {
    this.router.navigate(['/home']);
  }
}
