import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthorizeService } from './service/authorize.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authorizeService: AuthorizeService, private router: Router) { }

  //canActivate(): boolean {
  //  if (this.authorizeService.isAuthenticated()) return true;

  //  this.router.navigate(['/login']);
  //  return false;
  //}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!this.authorizeService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return false;
    }

    const requiredRoles = route.data['roles'] as string[] | undefined;
    const userRole = this.authorizeService.getRoleFromToken();

    if (requiredRoles && (!userRole || !requiredRoles.includes(userRole))) {
      this.router.navigate(['/acces-denied']); 
      return false;
    }

    return true;
  }
}


