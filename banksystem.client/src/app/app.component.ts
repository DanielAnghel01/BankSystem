import { RouterOutlet } from '@angular/router';
import { Component } from '@angular/core';


@Component({
  standalone: true,
  selector: 'app-root',
  imports:[RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'banksystem.client';
}
