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
    path: 'transaction',
    //component: TransactionComponent,
    loadComponent: () => import('./pages/Transactions/transaction/transaction.component').then(m => m.TransactionComponent),
    data: { title: 'Transactions' },
    canActivate: [AuthGuard]
  },
  {
    path: 'user',
    loadComponent: () => import('./pages/user/user.component').then(m => m.UserComponent),
    data: { title: 'User' },
    canActivate: [AuthGuard]
  },

  {
    path: '**',
    redirectTo: 'home'
  }
];
