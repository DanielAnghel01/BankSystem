import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BankAccountModel } from '../models/bank-account.model';

@Injectable({
  providedIn: 'root'
})
export class BankAccountService {
  private apiUrl = 'https://localhost:7022/api/bank-account'; 

  constructor(private http: HttpClient) { }

  getAccounts(): Observable<BankAccountModel[]> {
    const userId = Number(localStorage.getItem('userId')); // or get from AuthService
    return this.http.get<BankAccountModel[]>(`${this.apiUrl}/by-user?userId=${userId}`);
  }

  createAccount(account: BankAccountModel): Observable<BankAccountModel> {
    return this.http.post<BankAccountModel>(`${this.apiUrl}/create`, account);
  }
}
