import { Component, OnInit } from '@angular/core';
import { TransactionService } from '../services/transaction.service';
import { TransactionHistoryModel } from '../models/transaction-history.model';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-transaction-history',
  templateUrl: './transaction-history.component.html',
  styleUrl: './transaction-history.component.css',
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule
  ]
})
export class TransactionHistoryComponent implements OnInit {
  transactions: TransactionHistoryModel[] = [];

  constructor(
    private transactionService: TransactionService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.transactionService.getUserTransactions().subscribe({
      next: (data) => this.transactions = data,
      error: (err) => console.error('Failed to load transactions', err)
    });
  }

  goToHome(): void {
    this.router.navigate(['/home'])
  }
  goToProfile(): void {
    this.router.navigate(['/user'])
  }
  goToDashboard(): void {
    this.router.navigate(['/dashboard'])
  }
}
