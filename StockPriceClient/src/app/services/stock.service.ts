import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { StockUpdate } from '../types/types';

export const action = {
  JOINGROUP: 'JoinStockGroup',
  LEAVEGROUP: 'LeaveStockGroup',
};

const config = {
  apiRootUrl: 'https://localhost:7219',
};

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private connection!: HubConnection;
  private stockUpdateSubject = new Subject<StockUpdate>();

  // Expose this as an observable that components can subscribe to
  stockUpdates$ = this.stockUpdateSubject.asObservable();

  constructor(private http: HttpClient) {
    this.initializeSignalR();
  }

  fetchInitialPrice(ticker: string): Observable<{ price: number }> {
    return this.http.get<{ price: number }>(`${config.apiRootUrl}/api/stocks/${ticker}`);
  }

  private async initializeSignalR() {
    this.connection = new HubConnectionBuilder()
      .withUrl(`${config.apiRootUrl}/stocks-feed`)
      .configureLogging(LogLevel.Information)
      .build();

    this.connection.on('ReceiveStockPriceUpdate', (stockUpdate: StockUpdate) => {
      this.stockUpdateSubject.next(stockUpdate);
    });

    this.connection.onclose(async () => {
      await this.startConnection();
    });

    await this.startConnection();
  }

  private async startConnection() {
    try {
      await this.connection.start();
    } catch (err) {
      console.error('SignalR Connection Error, retrying...', err);
      setTimeout(() => this.startConnection(), 6000);
    }
  }

  async groupAction(action: string, ticker: string) {
    if (this.connection.state === 'Connected') {
      await this.connection.invoke(action, ticker);
    }
  }
}
