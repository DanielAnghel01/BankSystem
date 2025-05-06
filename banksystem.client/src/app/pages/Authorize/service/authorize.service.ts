import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../enviroments/enviroment';
import { LoginModel } from '../models/login.model';
import { TokenModel } from '../models/token.model';
import { Response } from '../../../core/models/response.model';
import { LocalStorageService } from '../service/storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  private apiUrl = 'https://bank-system-web.azurewebsites.net/api/auth/login';
  private apiUrlRegister = 'https://bank-system-web.azurewebsites.net/api/auth/register';

  constructor(
    private httpClient: HttpClient,
    private localStorageService: LocalStorageService
  ){
    this.apiUrl = environment.apiUrl;
  }

  register(data: any): Promise<any> {
    return this.httpClient.post('https://localhost:7022/api/auth/register', data).toPromise(); // Adjust endpoint as needed
  }


  login(loginModel: LoginModel): Promise<Response<TokenModel>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    return new Promise((resolve, reject) => {
      this.httpClient.post(this.apiUrl, loginModel, {headers})
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

  signOut() {
    this.localStorageService.remove('SESION_TOKEN');
  }
}
