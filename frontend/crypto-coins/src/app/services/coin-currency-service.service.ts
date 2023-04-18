import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CoinModel } from 'src/app/models/coin-model';


@Injectable({
  providedIn: 'root'
})
export class CoinCurrencyService{
  private readonly apiUrl = 'http://localhost:5209/api'; // URL de la API
  apiKey: string = "";
  constructor(private http: HttpClient) { }

  saveApiKey(apiKey: string): void {
    localStorage.setItem('apiKey', apiKey);
  }

  getApiKey(): string {
    return localStorage.getItem('apiKey') || '';
  }

  // Método que consume la API para obtener la lista de monedas
  public getCoins(): Observable<CoinModel[]> {const headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*'});
    this.apiKey = this.getApiKey();
    const url = `${this.apiUrl}/getcoins/${this.apiKey}`;
    return this.http.get<CoinModel[]>(url,{headers});
  }

  // Método que consume la API para convertir una criptomoneda en varias monedas
  public convertCryptoCurrency(fromSymbol: string, amount: number): Observable<CoinModel[]> {
    this.apiKey = this.getApiKey();
    const url = `${this.apiUrl}/getconversion/${this.apiKey}/${fromSymbol}/${amount}`; // URL completa para llamar al endpoint
    return this.http.get<CoinModel[]>(url); // Llamada GET a la API y retorno de la lista de monedas convertidas
  }
}
