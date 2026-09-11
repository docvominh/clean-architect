export type CurrencyCode = 'USD' | 'GBP' | 'EUR';

export interface ExchangeRatesResponse {
    rates: Record<string, number>;
}
