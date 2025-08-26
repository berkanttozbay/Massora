import { Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { BehaviorSubject, firstValueFrom, Observable } from 'rxjs';
import { map } from 'rxjs/operators'; // map operatörünü import edin

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private userDataSubject = new BehaviorSubject<any>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  private roles: string[] = [];

  userData$ = this.userDataSubject.asObservable();
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private oidcSecurityService: OidcSecurityService) {}

  initializeAuth(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.oidcSecurityService.checkAuth().subscribe(
        ({ isAuthenticated, userData }) => {
          this.isAuthenticatedSubject.next(isAuthenticated);
          this.userDataSubject.next(userData);

          if (isAuthenticated) {
            this.roles = this.extractRoles(userData);
          }
          // 'else' bloğu kaldırıldı. Giriş yapılmamışsa, uygulama hiçbir şey yapmamalı.
          // Kullanıcı, butona tıklayarak login olmalı.

          resolve();
        },
        (error) => reject(error)
      );
    });
  }

  login(): void {
    this.oidcSecurityService.authorize();
  }

  getToken(): Promise<string> {
    return firstValueFrom(this.oidcSecurityService.getAccessToken());
  }

  logout(): void {
    this.oidcSecurityService.logoff().subscribe((result) => {
      this.isAuthenticatedSubject.next(false);
      this.userDataSubject.next(null);
      this.roles = []; // Rolleri de temizle
    });
  }

  private extractRoles(userData: any): string[] {
    if (!userData) return [];
    // 'role' claim'i tek bir değer veya bir dizi olabilir, ikisini de kontrol edelim.
    const roles = userData.role;
    if (Array.isArray(roles)) {
        return roles;
    }
    if (typeof roles === 'string') {
        return [roles];
    }
    return [];
  }

  hasRole(role: string): boolean {
    return this.roles.includes(role);
  }

  getRoles(): string[] {
    return this.roles;
  }
}