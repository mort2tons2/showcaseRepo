import { MatButtonModule, MatIconButton } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Component, OnInit } from '@angular/core';
import { Widget } from '../widget/widget';

const darkModeKey = 'darkMode';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    Widget,
  ],
})
export class Dashboard implements OnInit {
  newTicker = '';
  stockTickers: string[] = ['AMZN', 'MSFT', 'APPLE', 'META', 'IBM'];
  darkMode = false;

  ngOnInit(): void {
    this.darkMode = localStorage.getItem(darkModeKey) === 'true';
    this.changeTheme();
  }

  addStock() {
    const ticker = this.newTicker.trim().toUpperCase();
    if (!ticker) {
      alert('Please enter a ticker symbol');
      return;
    }
    if (this.stockTickers.includes(ticker)) {
      alert('This stock is already in your dashboard');
      return;
    }
    this.stockTickers.push(ticker);
    this.newTicker = '';
  }

  removeStock(tickerToRemove: string) {
    this.stockTickers = this.stockTickers.filter((t) => t !== tickerToRemove);
  }

  toggleDarkMode() {
    this.darkMode = !this.darkMode;
    localStorage.setItem(darkModeKey, this.darkMode.valueOf.toString());
    this.changeTheme();
  }

  changeTheme() {
    if (this.darkMode) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }
}
