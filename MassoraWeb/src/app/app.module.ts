import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule,HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { AppComponent } from './app.component';
import { CompanyModule } from './Massora/company-module/company.module';
import { CompanyRoutingModule } from './Massora/company-module/company-routing.module';
import { RouterModule } from '@angular/router';
import { CompanyDetailComponent } from './Massora/company-module/company/company-detail/company-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from './shared/shared.module';
import { HomeModule } from './Massora/home-module/home.module';
import { VehicleModule } from './Massora/vehicle-module/vehicle.module';
import { DriverModule } from './Massora/driver-module/driver.module';
import { Routes } from '@angular/router';
import { AppRoutingModule } from './app.routes';
import { PartnerCompanyModule } from './Massora/partnerCompanies-module/partnerCompanies.module';
import { WorkHistoriesModule } from './Massora/workhistories-module/workhistories.module';
import { FuelHistoriesModule } from './Massora/fuelhistories-module/fuelhistories.module';
import { AuthInterceptor, AuthModule, LogLevel } from 'angular-auth-oidc-client';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    CompanyModule, // Ensure appRoutes is imported here,
    SharedModule,
    HomeModule,
    VehicleModule,
    DriverModule,
    AppRoutingModule,
    PartnerCompanyModule,
    WorkHistoriesModule,
    FuelHistoriesModule,
    FormsModule,
    AuthModule.forRoot({
      config: {
        authority: 'http://localhost:5139', // Auth projen
        redirectUrl: window.location.origin,
        postLoginRoute: '/home',
        postLogoutRedirectUri: window.location.origin ,
        clientId: 'angular-client',
        scope: 'openid profile massoraapi offline_access',
        responseType: 'code',
        silentRenew: true,
        useRefreshToken: true,
        logLevel: LogLevel.Debug,
        secureRoutes: ['http://localhost:5260'], // API endpoint'in
      },
    }),
    NgbModule,
    
    
  ],
  providers: [
    // 2. ADIM: providers
    provideHttpClient(withInterceptorsFromDi()),
    { 
      provide: HTTP_INTERCEPTORS, 
      useClass: AuthInterceptor, 
      multi: true 
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
