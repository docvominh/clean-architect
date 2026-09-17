export type CurrencyCode = 'USD' | 'GBP' | 'EUR';

export interface CurrencyRate {
    symbol: string;
    rate: number;
    time: string;
}
