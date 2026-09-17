import { discardPeriodicTasks, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { apiUrl } from '../../user/api-url';
import { CurrencyService, REFRESH_INTERVAL_MS } from './currency.service';

describe('CurrencyService', () => {
    let service: CurrencyService;
    let http: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
        service = TestBed.inject(CurrencyService);
        http = TestBed.inject(HttpTestingController);
    });
    afterEach(() => http.verify());

    it('does not request a rate for the default USD currency', () => {
        // Arrange & Act
        const converted = service.convert(100);

        // Assert
        http.expectNone(apiUrl('/api/exchange-rates'));
        expect(converted).toBe(100);
    });

    it('fetches and applies the rate for the selected currency only', fakeAsync(() => {
        // Arrange
        service.setCurrency('GBP');
        tick();

        // Act
        const request = http.expectOne(r => r.url === apiUrl('/api/exchange-rates') && r.params.get('targetCurrency') === 'GBP');
        request.flush({ symbol: 'GBP', rate: 0.5, time: new Date().toISOString() });
        tick();

        // Assert
        expect(service.convert(100)).toBe(50);
        http.expectNone(() => true);
        discardPeriodicTasks();
    }));

    it('falls back to the original amount when the rate request fails', fakeAsync(() => {
        // Arrange
        service.setCurrency('EUR');
        tick();
        const request = http.expectOne(r => r.url === apiUrl('/api/exchange-rates'));

        // Act
        request.error(new ProgressEvent('error'));
        tick();

        // Assert
        expect(service.convert(100)).toBe(100);
        discardPeriodicTasks();
    }));

    it('polls again after the refresh interval elapses', fakeAsync(() => {
        // Arrange
        service.setCurrency('GBP');
        tick();
        http.expectOne(r => r.url === apiUrl('/api/exchange-rates')).flush({ symbol: 'GBP', rate: 0.5, time: new Date().toISOString() });
        tick();

        // Act
        tick(REFRESH_INTERVAL_MS);
        const secondRequest = http.expectOne(r => r.url === apiUrl('/api/exchange-rates'));
        secondRequest.flush({ symbol: 'GBP', rate: 0.6, time: new Date().toISOString() });
        tick();

        // Assert
        expect(service.convert(100)).toBe(60);
        discardPeriodicTasks();
    }));

    it('stops polling once switched back to USD', fakeAsync(() => {
        // Arrange
        service.setCurrency('GBP');
        tick();
        http.expectOne(r => r.url === apiUrl('/api/exchange-rates')).flush({ symbol: 'GBP', rate: 0.5, time: new Date().toISOString() });
        tick();

        // Act
        service.setCurrency('USD');
        tick(REFRESH_INTERVAL_MS);

        // Assert
        expect(service.convert(100)).toBe(100);
        http.expectNone(() => true);
    }));
});
