import { Routes } from '@angular/router';
import { LoginComponent } from './pages/Authorize/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/Authorize/register/register.component';
import { BankAccountComponent } from './pages/Transactions/bank-accounts/bank-accounts.component'
import { TransactionComponent } from './pages/Transactions/transaction/transaction.component'
import { AuthGuard } from './pages/Authorize/login.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'login',
    //component: LoginComponent,
    loadComponent: () => import('./pages/Authorize/login/login.component').then(m => m.LoginComponent),
    data: { title: 'Login' }
  },
  {
    path: 'home',
    //component: HomeComponent,
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent),
    data: { title: 'Home' }
  },
  {
    path: 'register',
    //component: RegisterComponent,
    loadComponent: () => import('./pages/Authorize/register/register.component').then(m => m.RegisterComponent),
    data: { title: 'Register' }
  },
  {
    path: 'bank-accounts',
    //component: BankAccountsComponent,
    loadComponent: () => import('./pages/Transactions/bank-accounts/bank-accounts.component').then(m => m.BankAccountComponent),
    data: { title: 'BankAccount' },
    canActivate: [AuthGuard]
  },
  {
    path: 'transfer',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/Transactions/transaction/transaction.component').then(m => m.TransactionComponent),
    data: { title: 'transfer' },
    canActivate: [AuthGuard]
  },
  {
    path: 'user',
    loadComponent: () => import('./pages/user/user.component').then(m => m.UserComponent),
    data: { title: 'User' },
    canActivate: [AuthGuard]
  },
  {
    path: 'exchange-rates',
    //component: ExchangeRatesComponent,
    loadComponent: () => import('./pages/exchange-rates/exchange-rates.component').then(m => m.ExchangeRatesComponent),
    data: { title: 'Exchange Rates' },
    canActivate: [AuthGuard]
  },
  {
    path: 'transaction',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/Transactions/transaction-history/transaction-history.component').then(m => m.TransactionHistoryComponent),
    data: { title: 'transaction' },
    canActivate: [AuthGuard]
  },
  {
    path: 'deposit',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/Transactions/deposit/deposit.component').then(m => m.DepositComponent),
    data: { title: 'deposit' },
    canActivate: [AuthGuard]
  },
  {
    path: 'withdraw',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/Transactions/withdraw/withdraw.component').then(m => m.WithdrawComponent),
    data: { title: 'withdraw' },
    canActivate: [AuthGuard]
  },
  {
    path: 'dashboard',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    data: { title: 'dashboard' },
    canActivate: [AuthGuard]
  },
  {
    path: 'acces-denied',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/Authorize/acces-denied/acces-denied.component').then(m => m.AccesDeniedComponent),
    data: { title: 'acces-denied' }
  },
  {
    path: 'admin',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
    data: { title: 'admin', roles: ['admin'] },
    canActivate: [AuthGuard]
  },

  {
    path: '**',
    redirectTo: 'home'
  }
];
