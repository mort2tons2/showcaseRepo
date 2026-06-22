import { Component, OnInit, signal } from '@angular/core';
import { Dashboard } from './dashboard/dashboard';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Dashboard],
})
export class App implements OnInit {
  ngOnInit() {}

  protected readonly title = signal('StockPriceClient');
}
