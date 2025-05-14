import { Component } from '@angular/core';
import { TransactionService } from '../services/transaction.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-transaction',
  templateUrl: './transaction.component.html',
  styleUrl: './transaction.component.css',
  imports: [FormsModule],
})
export class TransactionComponent {
  senderAccountNumber!: number;
  reciverAccountNumber!: number;
  amount!: number;
  details: string = '';
  message: string = '';
  error: string = '';

  constructor(private transactionService: TransactionService) { }

  submit(): void {
    const transactionPayload = {
      senderAccountNumber: this.senderAccountNumber,
      reciverAccountNumber: this.reciverAccountNumber,
      amount: this.amount,
      details: this.details,
      transactionType: 'transfer' // Optional, if required by backend
    };

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
}
