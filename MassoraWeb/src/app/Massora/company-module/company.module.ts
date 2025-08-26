import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompanyDetailComponent } from './company/company-detail/company-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CompanyAddComponent } from './company/company-add/company-add.component';
import { CompanyGetAllComponent } from './company/company-getAll/company-getAll.component';
import { CompanyRoutingModule } from './company-routing.module';

@NgModule({
  declarations: [CompanyDetailComponent, CompanyAddComponent,CompanyGetAllComponent],

  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    CompanyRoutingModule
    // 
    
  ],
  exports: [CompanyDetailComponent, CompanyAddComponent, CompanyGetAllComponent]
})
export class CompanyModule { }
