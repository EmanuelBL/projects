import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CoinModel } from 'src/app/models/coin-model';

@Injectable({
  providedIn: 'root'
})
export class CoinCurrencyService {
  private readonly apiUrl = 'http://localhost:5209/api'; // Url de la api
  apiKey: string = "";

  constructor(private http: HttpClient) { }

  saveApiKey(apiKey: string): void {
    localStorage.setItem('apiKey', apiKey);
  }

  getApiKey(): string {
    return localStorage.getItem('apiKey') || '';
  }

  // Consumir la API para obtener la lista de monedas
  public getCoins(): Observable<CoinModel[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*'
    });
    this.apiKey = this.getApiKey();
    const url = `${this.apiUrl}/coins/${this.apiKey}`;
    return this.http.get<CoinModel[]>(url, { headers }).pipe(
      catchError(err => {
        return throwError('Error al obtener las monedas', err.message);
      })
    );
  }

  // Consumir la API para convertir una moneda
  public convertCryptoCurrency(fromSymbol: string, amount: number): Observable<CoinModel[]> {
    this.apiKey = this.getApiKey();
    const url = `${this.apiUrl}/conversion/${this.apiKey}/${fromSymbol}/${amount}`;
    return this.http.get<CoinModel[]>(url).pipe(
      catchError(err => {
        return throwError(`Error al convertir la moneda ${fromSymbol}`, err.message);
      })
    );
  }
}
