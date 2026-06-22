import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { Dashboard } from './dashboard/dashboard';

/* type StockPrices = {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
} */

@Component({
  selector: 'app-root',
  standalone: true,
  //imports: [DashboardComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [Dashboard],
})
export class App implements OnInit {
  //public forecasts: WeatherForecast[] = [];
  //public stockTickers: string[] = ['AMZN', 'MSFT', 'APPLE', 'META', 'IBM'];

  //constructor(private http: HttpClient) {}

  ngOnInit() {
    //this.getForecasts();
  }

  /* getForecasts() {
    this.http.get<WeatherForecast[]>('/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  } */

  protected readonly title = signal('StockPriceClient');
}
