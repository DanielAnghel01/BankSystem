import { Routes } from '@angular/router';
import { LoginComponent } from './pages/Authorize/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/Authorize/register/register.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
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
    path: '**',
    redirectTo: 'login'
  }
];
