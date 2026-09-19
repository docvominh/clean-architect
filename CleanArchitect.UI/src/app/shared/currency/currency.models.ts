export type CurrencyCode = 'USD' | 'GBP' | 'EUR' | 'VND';

export interface CurrencyRate {
    symbol: string;
    rate: number;
    time: string;
}
