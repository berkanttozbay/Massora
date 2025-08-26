import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PartnerCompanyGetAllComponent } from './partnerCompanies/partnerCompanies-getAll/partnerCompanies-getAll.component';
import { PartnerCompanyRoutingModule } from './partnerCompanies-routing.module';
import { PartnerCompanyAddComponent } from './partnerCompanies/partnerCompanies-add/partnerCompanies-add.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [PartnerCompanyGetAllComponent,PartnerCompanyAddComponent],

  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PartnerCompanyRoutingModule,
    SharedModule
  ],
  exports: [PartnerCompanyGetAllComponent,PartnerCompanyAddComponent]
})
export class PartnerCompanyModule { }
