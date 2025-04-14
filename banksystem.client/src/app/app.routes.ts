import { Routes } from '@angular/router';
import { LoginComponent } from './pages/Authorize/login/login.component';
import { HomeComponent } from './pages/home/home.component';

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
    path: '**',
    redirectTo: 'login'
  }
];
