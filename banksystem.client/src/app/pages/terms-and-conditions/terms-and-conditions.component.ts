import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';

@Component({
  standalone: true,
  selector: 'app-terms-and-conditions',
  templateUrl: './terms-and-conditions.component.html',
  styleUrl: './terms-and-conditions.component.css',
  imports: [
    CommonModule,
    MatCardModule,
    MatListModule,
    MatDividerModule
  ]
})
export class TermsAndConditionsComponent {

}
