import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, Observable, of, Subject, switchMap, timer } from 'rxjs';
import { apiUrl } from '../../user/api-url';
import { CurrencyCode, CurrencyRate } from './currency.models';

export const REFRESH_INTERVAL_MS = 30_000;

@Injectable({ providedIn: 'root' })
export class CurrencyService {
    private readonly http = inject(HttpClient);
    readonly currency = signal<CurrencyCode>('USD');
    private readonly rate = signal<number | null>(null);
    private readonly currencyChanges = new Subject<CurrencyCode>();

    constructor() {
        this.currencyChanges
            .pipe(
                switchMap(code => (code === 'USD' ? of(null) : timer(0, REFRESH_INTERVAL_MS).pipe(switchMap(() => this.fetchRate(code))))),
                takeUntilDestroyed(),
            )
            .subscribe(currencyRate => this.rate.set(currencyRate?.rate ?? null));
    }

    setCurrency(code: CurrencyCode): void {
        this.currency.set(code);
        this.currencyChanges.next(code);
    }

    convert(usdAmount: number): number {
        if (this.currency() === 'USD') return usdAmount;
        const rate = this.rate();
        return rate ? usdAmount * rate : usdAmount;
    }

    private fetchRate(targetCurrency: CurrencyCode): Observable<CurrencyRate | null> {
        return this.http.get<CurrencyRate>(apiUrl('/api/exchange-rates'), { params: { targetCurrency } }).pipe(catchError(() => of(null)));
    }
}
