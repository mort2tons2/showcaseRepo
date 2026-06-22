import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatButtonModule, MatIconButton } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { action, StockService } from '../services/stock.service';
import { Chart, registerables } from 'chart.js';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-widget',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
  ],
  templateUrl: './widget.html',
  styleUrl: './widget.css',
})
export class Widget implements OnInit, OnDestroy, AfterViewInit {
  @Input() ticker!: string;
  @Output() remove = new EventEmitter<string>();
  @ViewChild('stockChart', { static: true }) stockChart!: ElementRef<HTMLCanvasElement>;

  price: number | null = null;
  lastPrice: number | null = null;
  currentChangePercent: number = 0;
  currentChange: number = 0;
  changeText = '';
  changeClass = '';
  isLoading = true;
  errorMessage = '';

  priceHistory: number[] = [];
  chart!: Chart;
  updateSubscription!: Subscription;

  constructor(private stockService: StockService) {}

  ngOnInit(): void {
    this.fetchData();
    //this.initChart();
    this.updateSubscription = this.stockService.stockUpdates$.subscribe((update) => {
      if (update.ticker === this.ticker) {
        this.updatePrice(update.price);
      }
    });
  }
  ngOnDestroy(): void {
    if (this.chart) this.chart.destroy();
    if (this.updateSubscription) {
      this.updateSubscription.unsubscribe();
    }
  }

  ngAfterViewInit(): void {
    this.createChart();
  }

  fetchData() {
    this.stockService.fetchInitialPrice(this.ticker).subscribe({
      next: async (data) => {
        if (data && data.price) {
          this.isLoading = false;
          this.updatePrice(data.price);
          await this.stockService.groupAction(action.JOINGROUP, this.ticker);
        } else {
          this.errorMessage = 'Failed to fetch stock price.';
        }
      },
      error: (err) => {
        this.errorMessage = 'Ërror when fetching price';
      },
    });
  }

  updatePrice(newPrice: number) {
    if (this.lastPrice === null || this.lastPrice === 0) {
      this.price = newPrice;
      this.updateChart(newPrice);
      this.lastPrice = newPrice;
      return;
    }

    this.currentChange = newPrice - this.lastPrice;
    this.currentChangePercent = (this.currentChange / this.lastPrice) * 100;

    this.price = newPrice;
    this.updateChart(newPrice);
    this.lastPrice = newPrice;
  }

  //initializeChart() {}

  createChart() {
    const ctx = this.stockChart.nativeElement.getContext('2d');

    if (ctx) {
      this.chart = new Chart(ctx, {
        type: 'bar', // Change to 'line', 'pie', 'doughnut', etc.
        data: {
          labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
          datasets: [
            {
              label: '# of Votes',
              data: [12, 19, 3, 5, 2, 3],
              backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)',
              ],
              borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)',
              ],
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            y: {
              beginAtZero: true,
            },
          },
        },
      });
    }
  }

  //updateChart(price: number) {}

  /* private initChart() {
    this.chart = new Chart(this.stockChart.nativeElement, {
      type: 'line',
      data: {
        labels: [],
        datasets: [{ data: [], borderColor: 'rgb(75, 192, 192)', tension: 0.1, pointRadius: 0 }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false }, tooltip: { enabled: false } },
        scales: {
          x: { display: false },
          y: {
            display: true,
            position: 'right',
            grid: { display: false },
            ticks: {
              color: 'rgb(156, 163, 175)',
              font: { size: 10 },
              callback: (value) => '$' + Number(value).toFixed(2),
              count: 2,
            },
          },
        },
        animation: false,
      },
    });
  } */

  private updateChart(price: number) {
    this.priceHistory.push(price);
    if (this.priceHistory.length > 30) this.priceHistory.shift();

    const min = Math.min(...this.priceHistory);
    const max = Math.max(...this.priceHistory);
    const padding = (max - min) * 0.1;

    this.chart.data.labels = this.priceHistory.map((_, i) => i + 1);
    this.chart.data.datasets[0].data = this.priceHistory;

    if (this.chart.options.scales?.['y']) {
      this.chart.options.scales['y'].min = min - padding;
      this.chart.options.scales['y'].max = max + padding;
    }
    this.chart.update();
  }

  async onRemoveClick() {
    await this.stockService.groupAction(action.LEAVEGROUP, this.ticker);
    this.remove.emit(this.ticker);
  }
}
