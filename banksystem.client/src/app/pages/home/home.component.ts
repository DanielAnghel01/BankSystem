import { Component } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { AuthorizeService } from '../Authorize/service/authorize.service';
import { CommonModule } from '@angular/common';


@Component({
  standalone: true,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatButtonModule,
    RouterModule,
    MatCardModule,
    CommonModule
  ]
})
export class HomeComponent {
  username: string | null = null;

  constructor(private authService: AuthorizeService) { }

  ngOnInit(): void {
    this.username = this.authService.getUsernameFromToken();
  }

  isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }
}
