import { Pipe, PipeTransform, inject, LOCALE_ID } from '@angular/core';
import { CurrencyService } from './currency.service';

@Pipe({
    name: 'appCurrency',
    standalone: true,
    pure: false,
})
export class AppCurrencyPipe implements PipeTransform {
    private readonly currencyService = inject(CurrencyService);
    private readonly locale = inject(LOCALE_ID);

    transform(usdAmount: number | null | undefined): string | null {
        if (usdAmount == null) return null;

        const code = this.currencyService.currency();
        const convertedAmount = this.currencyService.convert(usdAmount);
        const parts = new Intl.NumberFormat(this.locale, { style: 'currency', currency: code }).formatToParts(convertedAmount);

        return parts
            .filter(part => code !== 'VND' || part.type !== 'currency')
            .map(part => part.value)
            .join('')
            .trim();
    }
}
