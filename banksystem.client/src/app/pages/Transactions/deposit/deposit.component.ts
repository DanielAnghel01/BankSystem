import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { TransactionService } from '../services/transaction.service';
import { BankAccountService } from '../services/bank-account.service';
import { BankAccountModel } from '../models/bank-account.model';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-deposit',
  templateUrl: './deposit.component.html',
  styleUrl: './deposit.component.css',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    HttpClientModule,
  ]
})
export class DepositComponent implements OnInit {
  userAccounts: BankAccountModel[] = [];
  selectedAccountNumber: string = '';
  depositAmount: number = 0;

  constructor(
    private transactionService: TransactionService,
    private bankAccountService: BankAccountService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.bankAccountService.getAccounts().subscribe({
      next: (data) => this.userAccounts = data,
      error: (err) => console.error('Failed to fetch accounts', err)
    });
  }

  submitDeposit() {
    if (!this.selectedAccountNumber || this.depositAmount <= 0) {
      alert('Please select an account and enter a valid amount.');
      return;
    }

    this.transactionService.depositFunds(this.selectedAccountNumber, this.depositAmount)
      .subscribe({
        next: (res) => {
          alert('Deposit successful!');
          this.depositAmount = 0;
          this.router.navigate(['/user'])
        },
        error: (err) => {
          console.error('Deposit failed:', err);
          alert('Deposit failed.');
        }
      });
  }

  resetForm() {
    this.selectedAccountNumber = '';
    this.depositAmount = 0;
  }
}
