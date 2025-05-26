import { Component } from '@angular/core';
import { TransactionService } from '../services/transaction.service';
import { FormsModule } from '@angular/forms';
import { AuthorizeService } from '../../Authorize/service/authorize.service';
import { BankAccountService } from '../services/bank-account.service';
import { BankAccountModel } from '../models/bank-account.model';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';


@Component({
  standalone: true,
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrl: './transaction.component.css',
  imports: [
    FormsModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    CommonModule
  ],
})
export class TransactionComponent {
  senderAccountNumber!: string;
  reciverAccountNumber!: string;
  amount!: number;
  details: string = '';
  message: string = '';
  error: string = '';
  userAccounts: BankAccountModel[] = [];

  constructor(
    private transactionService: TransactionService,
    private authorizeService: AuthorizeService,
    private bankAccountService: BankAccountService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.bankAccountService.getAccounts().subscribe({
      next: (accounts: BankAccountModel[]) => {
        this.userAccounts = accounts;
        console.log(this.userAccounts);
        },
        error: (err) => console.error('Failed to load user accounts:', err)
      });

  }

  submit(): void {
    const transactionPayload = {
      senderAccountNumber: this.senderAccountNumber,
      reciverAccountNumber: this.reciverAccountNumber,
      amount: this.amount,
      details: this.details
    };
    console.log(transactionPayload);
    this.transactionService.makeTransfer(transactionPayload).subscribe({
      next: (res) => {
        this.message = res.message || 'Transfer successful';
        this.error = '';
      },
      error: (err) => {
        this.error = err.error?.message || 'Transfer failed';
        this.message = '';
      }
    });
  }

  resetForm() {
    this.senderAccountNumber = '';
    this.reciverAccountNumber = '';
    this.amount = 0;
    this.details = '';
  }

  goToProfile(): void {
    this.router.navigate(['/user'])
  }
}
