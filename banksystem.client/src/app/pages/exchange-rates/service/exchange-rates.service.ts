import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ExchangeRatesService {
  private apiKey = '9fd43f420651472fa8e1e4f8d01be311'; // ⬅️ Replace this with your actual key
  private apiUrl = 'https://api.currencyfreaks.com/latest';

  constructor(private http: HttpClient) { }

  getExchangeRates() {
    return this.http.get<any>(`${this.apiUrl}?apikey=${this.apiKey}`);
  }
}
