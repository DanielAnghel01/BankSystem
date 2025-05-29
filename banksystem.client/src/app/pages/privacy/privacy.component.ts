import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';

@Component({
  standalone: true,
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  styleUrl: './privacy.component.css',
  imports: [
    MatCardModule,
    MatDividerModule,
    MatListModule,
  ]
})
export class PrivacyComponent {

}
