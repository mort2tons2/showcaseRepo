import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  AfterViewInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { action, StockService } from '../services/stock.service';
import { Chart } from 'chart.js/auto';
import { BehaviorSubject, filter, Subscription } from 'rxjs';

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

  priceStream = new BehaviorSubject<number>(0);
  price$ = this.priceStream.asObservable();

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

  @ViewChild('stockChart', { static: false }) stockChart!: ElementRef<HTMLCanvasElement>;
  chartInstance: any = null;

  ngOnInit() {
    this.fetchData();

    this.updateSubscription = this.stockService.stockUpdates$
      .pipe(filter((data) => data.ticker === this.ticker))
      .subscribe((matchedData) => {
        this.updatePrice(matchedData.price);
      });

    this.priceStream.subscribe((newPrice) => {
      if (newPrice !== 0 && this.chartInstance) {
        this.updateChart(newPrice);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.updateSubscription) {
      this.updateSubscription.unsubscribe();
    }

    if (this.chartInstance) {
      this.chartInstance.destroy();
      this.chartInstance = null;
      console.log(`[${this.ticker}] Chart instance destroyed safely.`);
    }
  }

  ngAfterViewInit() {
    if (!this.stockChart || !this.stockChart.nativeElement) {
      return false;
    }

    const ctx = this.stockChart.nativeElement.getContext('2d');
    if (ctx) {
      this.chartInstance = new Chart(ctx, {
        type: 'line',
        data: {
          labels: [],
          datasets: [
            {
              label: `${this.ticker} Stock price`,
              data: [],
              borderColor: '#2563eb',
              backgroundColor: 'rgba(37, 99, 235, 0.1)',
              fill: true,
              tension: 0.2,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            y: {
              beginAtZero: false,
            },
          },
        },
      });

      return true;
    }

    return false;
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
      this.priceStream.next(newPrice);

      this.lastPrice = newPrice;
      return;
    }

    this.currentChange = newPrice - this.lastPrice;
    this.currentChangePercent = (this.currentChange / this.lastPrice) * 100;

    this.priceStream.next(newPrice);

    this.lastPrice = newPrice;
  }

  updateChart(price: number) {
    if (!this.chartInstance) return;

    const timestamp = new Date().toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    });

    this.chartInstance.data.labels.push(timestamp);
    this.chartInstance.data.datasets[0].data.push(price);

    if (this.chartInstance.data.labels.length > 20) {
      this.chartInstance.data.labels.shift();
      this.chartInstance.data.datasets[0].data.shift();
    }

    this.chartInstance.update('none');
  }

  async onRemoveClick() {
    await this.stockService.groupAction(action.LEAVEGROUP, this.ticker);
    this.remove.emit(this.ticker);
  }
}
