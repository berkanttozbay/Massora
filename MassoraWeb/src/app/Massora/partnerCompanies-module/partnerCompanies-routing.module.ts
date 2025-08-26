import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PartnerCompanyGetAllComponent } from './partnerCompanies/partnerCompanies-getAll/partnerCompanies-getAll.component';
import { PartnerCompanyAddComponent } from './partnerCompanies/partnerCompanies-add/partnerCompanies-add.component';

const routes: Routes = [
  { path: 'partnerCompanies-getAll', component: PartnerCompanyGetAllComponent },
  { path: 'partnerCompanies-add', component: PartnerCompanyAddComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartnerCompanyRoutingModule { }
 