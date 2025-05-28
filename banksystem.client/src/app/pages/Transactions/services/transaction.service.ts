import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TransactionModel } from '../models/transactions.model';
import { environment } from '../../../../enviroments/enviroment';
import { LocalStorageService } from '../../Authorize/service/storage.service';
import { TransactionHistoryModel } from '../models/transaction-history.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl = '';

  constructor(
    private http: HttpClient,
    private localStorage: LocalStorageService
  ) {
    this.apiUrl = environment.apiUrlProd + 'api/transaction';
  }

  makeTransfer(transaction: Partial<TransactionModel>): Observable<any> {
    console.log("Service: "+transaction);
    return this.http.post(`${this.apiUrl}/transfer`, transaction);
  }

  getUserTransactions(): Observable<TransactionHistoryModel[]> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.localStorage.get('SESSION_TOKEN')}`
    });
    console.log(headers);
    return this.http.get<TransactionHistoryModel[]>(`${this.apiUrl}/by-user`, { headers });
  }

  depositFunds(accountNumber: string, amount: number): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.localStorage.get('SESSION_TOKEN')}`
    });
    return this.http.post(`${this.apiUrl}/deposit`, {
      accountNumber,
      amount
    }, {
      headers
    });
  }

  withdrawFunds(accountNumber: string, amount: number): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${this.localStorage.get('SESSION_TOKEN')}`
    });
    return this.http.post(`${this.apiUrl}/withdraw`, {
      accountNumber,
      amount
    }, {
      headers
    });
  }

}
