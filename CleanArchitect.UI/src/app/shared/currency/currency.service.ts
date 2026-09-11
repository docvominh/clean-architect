import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { apiUrl } from '../../user/api-url';
import { CurrencyCode, ExchangeRatesResponse } from './currency.models';

@Injectable({ providedIn: 'root' })
export class CurrencyService {
    private readonly http = inject(HttpClient);
    readonly currency = signal<CurrencyCode>('USD');
    private readonly rates = signal<Record<string, number>>({});

    constructor() {
        this.loadRates();
    }

    setCurrency(code: CurrencyCode): void {
        this.currency.set(code);
    }

    convert(usdAmount: number): number {
        const code = this.currency();
        if (code === 'USD') return usdAmount;
        const rate = this.rates()[code];
        return rate ? usdAmount * rate : usdAmount;
    }

    private async loadRates(): Promise<void> {
        try {
            const response = await firstValueFrom(this.http.get<ExchangeRatesResponse>(apiUrl('/api/exchange-rates')));
            this.rates.set(response.rates);
        } catch {
            // Keep USD-only display if the rate service is unavailable.
        }
    }
}
