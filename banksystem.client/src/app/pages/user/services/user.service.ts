import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.models'; 
import { BankAccount } from '../models/bank-account.models';
import { LocalStorageService } from '../../Authorize/service/storage.service';
import { UserProfileModel } from '../models/user-profile.models';
import { Response } from '../../../core/models/response.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  user!: User;
  bankAccount!: BankAccount[];
  private apiUrl = 'https://bank-system-web.azurewebsites.net/api/user/profile';
  constructor(
    private httpClient: HttpClient,
    private localStorageService: LocalStorageService
  ) { }

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
