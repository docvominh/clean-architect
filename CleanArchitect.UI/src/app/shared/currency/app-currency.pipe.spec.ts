import { discardPeriodicTasks, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { apiUrl } from '../../user/api-url';
import { AppCurrencyPipe } from './app-currency.pipe';
import { CurrencyService } from './currency.service';

describe('AppCurrencyPipe', () => {
    let pipe: AppCurrencyPipe;
    let currencyService: CurrencyService;
    let http: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
        currencyService = TestBed.inject(CurrencyService);
        http = TestBed.inject(HttpTestingController);
        pipe = TestBed.runInInjectionContext(() => new AppCurrencyPipe());
    });
    afterEach(() => http.verify());

    it('formats the USD amount when no currency conversion is active', () => {
        // Arrange & Act
        const result = pipe.transform(100);

        // Assert
        expect(result).toBe('$100.00');
    });

    it('converts and formats the amount using the currently selected currency', fakeAsync(() => {
        // Arrange
        currencyService.setCurrency('GBP');
        tick();
        http.expectOne(r => r.url === apiUrl('/api/exchange-rates')).flush({ symbol: 'GBP', rate: 0.5, time: new Date().toISOString() });
        tick();

        // Act
        const result = pipe.transform(100);

        // Assert
        expect(result).toBe('£50.00');
        discardPeriodicTasks();
    }));

    it('omits the currency symbol when the selected currency is VND', fakeAsync(() => {
        // Arrange
        currencyService.setCurrency('VND');
        tick();
        http.expectOne(r => r.url === apiUrl('/api/exchange-rates')).flush({ symbol: 'VND', rate: 24000, time: new Date().toISOString() });
        tick();

        // Act
        const result = pipe.transform(100);

        // Assert
        expect(result).toBe('2,400,000');
        discardPeriodicTasks();
    }));

    it('returns null when given a null or undefined amount', () => {
        // Arrange & Act
        const nullResult = pipe.transform(null);
        const undefinedResult = pipe.transform(undefined);

        // Assert
        expect(nullResult).toBeNull();
        expect(undefinedResult).toBeNull();
    });
});
