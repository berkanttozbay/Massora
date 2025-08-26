import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from '../Massora/home-module/home/home.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  
  {
    path: 'home',
    loadChildren: () => import('../Massora/home-module/home.module').then(m => m.HomeModule)
  },
  {
    path: 'company',
    loadChildren: () => import('../Massora/company-module/company.module').then(m => m.CompanyModule)
  },
  {
    path: 'driver',
    loadChildren: () => import('../Massora/driver-module/driver.module').then(m => m.DriverModule)
  },
  {
    path: 'partnerCompanies',
    loadChildren: () => import('../Massora/partnerCompanies-module/partnerCompanies.module').then(m => m.PartnerCompanyModule)
  },
  {
    path: 'workhistories',
    loadChildren: () => import('../Massora/workhistories-module/workhistories.module').then(m => m.WorkHistoriesModule)
  },
  {
    path: 'fuelhistories',
    loadChildren: () => import('../Massora/fuelhistories-module/fuelhistories.module').then(m => m.FuelHistoriesModule)
  },

]
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SharedRoutingModule { }
 