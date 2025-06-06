import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BankAccountService } from '../services/bank-account.service';

//Material modules
import { FormsModule } from '@angular/forms'; // For ngModel, ngForm
/*import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; // Required for Angular Material*/
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card'; 
import { BankAccountModel } from '../models/bank-account.model';
import { AuthorizeService } from '../../Authorize/service/authorize.service';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-bank-accounts',
  templateUrl: './bank-accounts.component.html',
  styleUrl: './bank-accounts.component.css',
  imports: [
    CommonModule,
    FormsModule,
   /* BrowserAnimationsModule,*/
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatSelectModule
  ]
})
export class BankAccountComponent{
  accounts: BankAccountModel[] = [];

  constructor(
    private bankAccountService: BankAccountService,
    private authorizeService: AuthorizeService,
    private router: Router 
  ) { }



  newAccount = {
    userId: this.authorizeService.getIdFromToken(),
    accountType: '',
    currency: '',
    balance: 0,
    accountNumber: '',
    createdAt: new Date().toISOString() // Set to current date
  };

  createAccount() {
    this.newAccount.accountNumber = this.generateRandomAccountNumber();
    console.log(this.newAccount.userId);
    if (this.newAccount.userId !== null) {
      this.bankAccountService.createAccount(this.newAccount).subscribe({
        next: (res) => {
          console.log('Account created:', res);
          this.accounts.push(res); // Optionally update list
        },
        error: (err) => console.error('Error creating account', err)
      });
    }
  }

  generateRandomAccountNumber(): string {
    return Math.floor(100000000 + Math.random() * 900000000).toString();
  }

  goToProfile(): void {
    this.router.navigate(['/user'])
  }

  goToTransferFunds(): void {
    this.router.navigate(['/transfer'])
  }
}
