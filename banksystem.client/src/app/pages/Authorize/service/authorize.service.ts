import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../enviroments/enviroment';
import { LoginModel } from '../models/login.model';
import { TokenModel } from '../models/token.model';
import { RegisterModel } from '../models/register.model';
import { Response } from '../../../core/models/response.model';
import { LocalStorageService } from '../service/storage.service';
import { jwtDecode } from 'jwt-decode';
import { MatDialog } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  private apiUrlLogin = '';
  private apiUrlRegister = '';

  constructor(
    private httpClient: HttpClient,
    private localStorageService: LocalStorageService,
    private dialog: MatDialog
  ){
    this.apiUrlLogin = environment.apiUrlProd ;
    this.apiUrlRegister = environment.apiUrlProd + 'api/auth/register';
  }

  register(registerModel: RegisterModel): Promise<Response<TokenModel>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    console.log('Register payload:', registerModel);
    return new Promise((resolve, reject) => {
      this.httpClient.post(this.apiUrlRegister, registerModel, { headers })
        .toPromise()
        .then((result: any) => {
          resolve(result);
          console.log(result);
        },
          (error) => {
            reject(error);
          }
        );
    });
  }

  login(loginModel: LoginModel): Promise<Response<TokenModel>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return new Promise((resolve, reject) => {
      this.httpClient.post<Response<TokenModel>>(this.apiUrlLogin + 'api/auth/login', loginModel, { headers })
        .toPromise()
        .then((result) => {
          if (result) {
            console.log("Second one: " + result.content.token);
            if (result.content.twoFAEnabled) {

            }
            this.localStorageService.set('SESSION_TOKEN', result.content.token);
            resolve(result);
            console.log('Login successful:', result);
          }
        })
        .catch((error) => {
          console.error('Login failed:', error);
          reject(error);
        });
    });
  }

  getUsernameFromToken(): string | null {
    const token = this.getToken();
    console.log('Token:', token); 
    if (!token) return null;

    try {
      const decodedToken = jwtDecode<{ unique_name: string }>(token);
      console.log(decodedToken.unique_name);
      return decodedToken?.unique_name || null;
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  getRoleFromToken(): string | null {
    const token = this.getToken();
    console.log('Token:', token);
    if (!token) return null;

    try {
      const decodedToken = jwtDecode<{ role: string }>(token);
      console.log(decodedToken.role);
      return decodedToken?.role || null;
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  getIdFromToken(): number {
    const token = this.getToken();
    console.log('Token:', token);
    if (!token) return 0;

    try {
      const decodedToken = jwtDecode<{ nameid : number }>(token);
      console.log(decodedToken.nameid);
      return decodedToken?.nameid || 0;
    } catch (error) {
      console.error('Error decoding token:', error);
      return 0;
    }
  }

  getToken(): string | null {
    return this.localStorageService.get('SESSION_TOKEN');
  }

  signOut() {
    this.localStorageService.remove('SESSION_TOKEN');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
  verify2FA(data: { username: string; code: string }): Promise<Response<TokenModel>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return new Promise((resolve, reject) => {
      this.httpClient.post<Response<TokenModel>>(this.apiUrlLogin + 'api/auth/verify-2fa', data, { headers })
        .toPromise()
        .then((result) => {
          if (result) {
            console.log("2FA verification token: " + result.content.token);
            this.localStorageService.set('SESSION_TOKEN', result.content.token);
            resolve(result);
            console.log('2FA verification successful:', result);
          }
        })
        .catch((error) => {
          console.error('2FA verification failed:', error);
          reject(error);
        });
    });
  }


}
