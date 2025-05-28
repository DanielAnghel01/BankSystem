import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LocalStorageService } from '../../Authorize/service/storage.service';
import { Response } from '../../../core/models/response.model';
import { environment } from '../../../../enviroments/enviroment';
import { User } from '../../user/models/user.models';
import { UserProfilesModel } from '../models/user-profiles.model';
import { RegisterModel } from '../../Authorize/models/register.model';
import { TokenModel } from '../../Authorize/models/token.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
  user!: User[];
  private apiUrl = '';
  private apiUrlRegister = '';
  constructor(
    private httpClient: HttpClient,
    private localStorageService: LocalStorageService
  ) {
    this.apiUrl = environment.apiUrlLocal + 'api/user';
    this.apiUrlRegister = environment.apiUrlLocal + 'api/auth';
  }

  loadUsers(): Promise<User[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.localStorageService.get('SESSION_TOKEN')}`
    });
    console.log(headers);
    return new Promise((resolve, reject) => {
      this.httpClient.get<User[]>(
        this.apiUrl + '/get-users',
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
  createAdvancedUser(registerModel: RegisterModel): Promise<Response<TokenModel>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    console.log('Register payload:', registerModel);
    return new Promise((resolve, reject) => {
      this.httpClient.post(this.apiUrlRegister + '/admin/create', registerModel, { headers })
        .toPromise()
        .then((result: any) => {
          resolve(result);
          alert('User created successfully!');
          console.log(result);
        },
          (error) => {
            reject(error);
          }
        );
    });
  }

  deactivateUser(userId: number): Promise<void> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.localStorageService.get('SESSION_TOKEN')}`
    });

    return new Promise((resolve, reject) => {
      this.httpClient.post<void>(
        `${this.apiUrl}/deactivate/${userId}`,
        { headers }
      )
        .toPromise()
        .then(() => {
          alert('User deactivated successfully!');
          resolve();
        })
        .catch(error => {
          reject(error);
        });
    });
  }
  activateUser(userId: number): Promise<void> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.localStorageService.get('SESSION_TOKEN')}`
    });

    return new Promise((resolve, reject) => {
      this.httpClient.post<void>(
        `${this.apiUrl}/activate/${userId}`,
        { headers }
      )
        .toPromise()
        .then(() => {
          alert('User activated successfully!');
          resolve();
        })
        .catch(error => {
          reject(error);
        });
    });
  }
  twoFactor(userId: number): Promise<void> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.localStorageService.get('SESSION_TOKEN')}`
    });

    return new Promise((resolve, reject) => {
      this.httpClient.post<void>(
        `${this.apiUrl}/tfa/${userId}`,
        { headers }
      )
        .toPromise()
        .then(() => {
          resolve();
        })
        .catch(error => {
          reject(error);
        });
    });
  }

}
