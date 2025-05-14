import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthorizeService } from './service/authorize.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authorizeService: AuthorizeService, private router: Router) { }

  canActivate(): boolean {
    if (this.authorizeService.isAuthenticated()) return true;

    this.router.navigate(['/login']);
    return false;
  }
}


