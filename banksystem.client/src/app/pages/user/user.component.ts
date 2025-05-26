import { Component, OnInit } from '@angular/core';
import { UserService } from './services/user.service';
import { User } from './models/user.models';
import { BankAccount } from './models/bank-account.models';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { UserProfileModel } from './models/user-profile.models';
import { Response } from '../../core/models/response.model';
import { AuthorizeService } from '../Authorize/service/authorize.service';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';


@Component({
  standalone: true,
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrl: './user.component.css',
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule
  ]
})
export class UserComponent implements OnInit {
  user!: User;
  bankAccount: BankAccount[] = [];

  constructor(
    private userService: UserService,
    private authorizeService: AuthorizeService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.userService.getUserProfile()
      .then((response: UserProfileModel) => {
        console.log(response.bankAccounts);
        this.user = response.user;
        this.bankAccount = response.bankAccounts

      })
      .catch(error => {
        console.error('Error loading user profile:', error);
      });
  }

  logout(): void {
    this.authorizeService.signOut();
    this.router.navigate(['/home']);
  }
  createBankAccount(): void {
    this.router.navigate(['/bank-accounts'])
  }
  goToHome(): void {
    this.router.navigate(['/home'])
  }
  goToTransfer(): void {
    this.router.navigate(['/transfer'])
  }
}
