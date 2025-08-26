import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { AuthService } from './auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: false,
})
export class AppComponent {
  constructor(private router: Router, public authService: AuthService) { }

   ngOnInit() {
    this.authService.initializeAuth().catch(error => {
      console.error('Kimlik doğrulama hatası:', error);
    });
  }
  // Butonun çağıracağı login metodu
  login(): void {
    this.authService.login();
  }

  // Butonun çağıracağı logout metodu
  logout(): void {
    this.authService.logout();
  }

  //#region Router
  goToCompanyDetail() {
    this.router.navigate(['/company-details', 1]);  // '/company-details/1'
  }

  goToCompanyAdd() {
    this.router.navigate(['/company-add']);
  }
  goToGetAll() {
    this.router.navigate(['/company-getall']);
  }
  goToVehicleGetAll() {
    this.router.navigate(['/vehicle-getall']);
  }
  goToVehicleAdd() {
    this.router.navigate(['/vehicle-add']);
  }
  goToDriverGetAll() {
    this.router.navigate(['/driver-getall']);
  }
  goToDriverAdd() {
    this.router.navigate(['/partnerCompanies-add']);
  }
  goToPartnerCompanyGetAll() {
    this.router.navigate(['/partnerCompanies-getall']);
  }
  goToPartnerCompanyAdd() {
    this.router.navigate(['/partnerCompanies-add']);
  }
  goToWorkHistories() {
    this.router.navigate(['/workhistories']);
  }
  goToFuelHistories() {
    this.router.navigate(['/fuelhistories']);
  }
  //#endregion
}
