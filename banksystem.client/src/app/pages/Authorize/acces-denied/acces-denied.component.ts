import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-acces-denied',
  templateUrl: './acces-denied.component.html',
  styleUrl: './acces-denied.component.css'
})
export class AccesDeniedComponent {
  constructor(
    private router: Router
  ) { }

  goToHome(): void {
    this.router.navigate(['/home'])
  }
}
