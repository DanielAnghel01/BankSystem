import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.models'; 
import { BankAccount } from '../models/bank-account.models';
import { LocalStorageService } from '../../Authorize/service/storage.service';
import { UserProfileModel } from '../models/user-profile.models';
import { Response } from '../../../core/models/response.model';
import { environment } from '../../../../enviroments/enviroment';

@Injectable({ providedIn: 'root' })
export class UserService {
  user!: User;
  bankAccount!: BankAccount[];
  private apiUrl = '';
  constructor(
    private httpClient: HttpClient,
    private localStorageService: LocalStorageService
  ) {
    this.apiUrl = environment.apiUrlProd + 'api/user/profile';
  }

  getUserProfile(): Promise<UserProfileModel> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.localStorageService.get('SESSION_TOKEN')}`
    });
    console.log(headers);
    return new Promise((resolve, reject) => {
      this.httpClient.get<UserProfileModel>(
        this.apiUrl,
        { headers }
      )
        .toPromise()
        .then((result) => {
          if (result) {
            resolve(result);
            console.log(result);
          }
        },
          (error) => {
            reject(error);
          }
        );
    });
  }

}
