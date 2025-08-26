import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AuthService } from './auth.service'; // Kendi AuthService'inizin yolunu kontrol edin

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    // API'nizin adresiyle başlamayan istekleri (örneğin, oidc-client'ın kendi istekleri) es geçebiliriz.
    // Bu adımı kendi API adresinize göre düzenleyebilirsiniz veya şimdilik kaldırabilirsiniz.
    // if (!request.url.startsWith('http://localhost:5260')) {
    //   return next.handle(request);
    // }

    // AuthService'deki getToken() metodu bir Promise döndürdüğü için onu
    // RxJS 'from' operatörü ile bir Observable'a çeviriyoruz.
    return from(this.authService.getToken()).pipe(
      // switchMap ile token değerini alıp yeni bir Observable zinciri başlatıyoruz.
      switchMap(token => {
        // Eğer token mevcutsa
        if (token) {
          // İsteği klonlayıp header'ına 'Authorization' başlığını ekliyoruz.
          const clonedRequest = request.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });
          // Header eklenmiş yeni isteği yola devam ettiriyoruz.
          return next.handle(clonedRequest);
        }

        // Eğer token yoksa, orijinal isteği olduğu gibi devam ettiriyoruz.
        return next.handle(request);
      })
    );
  }
}