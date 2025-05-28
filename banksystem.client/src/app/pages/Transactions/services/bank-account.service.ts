import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BankAccountModel } from '../models/bank-account.model';
import { environment } from '../../../../enviroments/enviroment';
import { LocalStorageService } from '../../Authorize/service/storage.service';

@Injectable({
  providedIn: 'root'
})
export class BankAccountService {
  private apiUrl = ''; 

  constructor(
    private http: HttpClient,
    private localStorageService: LocalStorageService
  ) {
    this.apiUrl = environment.apiUrlLocal + 'api/bank-account';
  }

  getAccounts(): Observable<BankAccountModel[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.localStorageService.get('SESSION_TOKEN')}`
    });
    return this.http.get<BankAccountModel[]>(`${this.apiUrl}/by-user`, {headers});
  }

  createAccount(account: BankAccountModel): Observable<BankAccountModel> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.localStorageService.get('SESSION_TOKEN')}`
    });
    return this.http.post<BankAccountModel>(`${this.apiUrl}/create`, account, {headers});
  }
}
