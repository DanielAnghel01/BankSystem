import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TransactionModel } from '../models/transactions.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl = 'https://bank-system-web.azurewebsites.net/api/transaction'; // Adjust if needed

  constructor(private http: HttpClient) { }

  makeTransfer(transaction: Partial<TransactionModel>): Observable<any> {
    return this.http.post(`${this.apiUrl}/transfer`, transaction);
  }
}
