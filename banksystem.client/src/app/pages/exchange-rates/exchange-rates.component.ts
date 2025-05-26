import { Component } from '@angular/core';
import { ExchangeRatesService } from '../exchange-rates/service/exchange-rates.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-exchange-rates',
  templateUrl: './exchange-rates.component.html',
  styleUrl: './exchange-rates.component.css',
  imports: [CommonModule]
})
export class ExchangeRatesComponent {
  rates: any;
  baseCurrency = 'RON'; // Display RON as base even though API is USD-based

  constructor(private exchangeService: ExchangeRatesService) { }

  commonCurrencies = ['USD', 'EUR', 'GBP', 'RON', 'CAD', 'AUD', 'CHF', 'JPY'];

  ngOnInit(): void {
    this.exchangeService.getExchangeRates().subscribe(data => {
      const usdToRon = parseFloat(data.rates['RON']);
      const rebasedRates: any = {};

      for (const [currency, rate] of Object.entries(data.rates)) {
        if (this.commonCurrencies.includes(currency)) {
          rebasedRates[currency] = parseFloat(rate as string) / usdToRon;
        }
      }

      this.rates = rebasedRates;
    });
  }
  toNumber(value: unknown): number {
    return typeof value === 'number' ? value : parseFloat(value as string);
  }
}
