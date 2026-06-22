import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
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
export class Widget implements OnInit, OnDestroy {
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
    this.initializeChart();
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

  initializeChart() {}

  updateChart(price: number) {}

  async onRemoveClick() {
    await this.stockService.groupAction(action.LEAVEGROUP, this.ticker);
    this.remove.emit(this.ticker);
  }
}
