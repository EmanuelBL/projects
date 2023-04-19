import { Component, OnInit, inject } from '@angular/core';
import { CoinModel } from 'src/app/models/coin-model';
import { CoinCurrencyService } from 'src/app/services/coin-currency-service.service';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-coin-component',
  templateUrl: './coin-component.component.html',
  styleUrls: ['./coin-component.component.css']
})
export class CoinComponent implements OnInit {
  showApiKeyForm = true;
  form: FormGroup;
  coins: CoinModel[] = [];
  amount: number = 0;
  convertedCoins: CoinModel[] = [];
  apiKey: string = '';
  error: string = '';
  type: string = '';
  symbols: string[] = ['BTC', 'ETH', 'BNB', 'USDT', 'ADA'];
  loading = false;

  constructor(private coinCurrencyService: CoinCurrencyService, private formBuilder: FormBuilder) {
    this.apiKey = localStorage.getItem('apiKey') || '';

    this.form = formBuilder.group({
      id: [0],
      apiKey: ['', Validators.required],
      amount: [''],
      type: new FormControl(this.symbols[0])
    })
  }

  ngOnInit() {
   if(localStorage.getItem('apiKey')) this.showApiKeyForm=false;
    this.loadCoins();
    setInterval(() => {
      this.loadCoins();
    }, 5000); // Actualizar cada 5 segundos
  }

  loadCoins() {
    try {
      const apiKeyValue=localStorage.getItem('apiKey');
      if (apiKeyValue) {
        this.coinCurrencyService.getCoins().subscribe(
          (data: CoinModel[]) => {
            this.coins = data;
          },
          (error) => {
            if (error.status === 409) {
              console.log('API inválida');
              alert('API inválida');
              this.logout();
            }
          }
        );
      }
    } catch (error) {
      console.error('Error al cargar las monedas:', error);
    }
  }

  saveApiKey(): void {
    try {
      const apiKeyValue = this.form.get('apiKey')?.value;
      if (apiKeyValue) {
        this.coinCurrencyService.saveApiKey(apiKeyValue);
        this.showApiKeyForm = false;
        this.form.get('apiKey')?.setValue('');
        alert('ApiKey guardada correctamente en localStorage');
      } else {
        alert('Por favor ingrese una apiKey válida');
      }
    } catch (error) {
      console.error('Error al guardar la apiKey:', error);
    }
  }

  convertCurrency(): void {
    try {
      const typeValue = this.form.get('type')?.value;
      const amountValue = this.form.get('amount')?.value;

      if (typeValue && amountValue) {
        this.loading = true;
        this.coinCurrencyService.convertCryptoCurrency(typeValue, amountValue).subscribe((coins: CoinModel[]) => {
          this.convertedCoins = coins;
          this.loading = false;
        }, error => {
          this.error = error.message;
        });

      } else {
        alert('Los valores de cantidad y tipo no pueden quedar en blanco');
      }
    } catch (error) {
      console.error('Error al convertir la moneda:', error);
    }
  }

  logout(): void {
    try {
      localStorage.removeItem('apiKey');
      this.showApiKeyForm = true;
    } catch (error) {
      console.error('Error al hacer logout:', error);
    }
  }

  onKeyDown(event: KeyboardEvent) {
    // Obtener el código de la tecla presionadaf
    const keyCode = event.keyCode || event.which;

    // Permitir números, decimales, teclas de navegación (flechas), borrar y retroceso
    const allowedKeys = [48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 190, 110, 37, 38, 39, 40, 8, 46];

    // Verificar si la tecla presionada está permitida
    if (allowedKeys.indexOf(keyCode) === -1) {
      // Si no está permitida, evitar que se ingrese el carácter
      event.preventDefault();
    }
  }
}
