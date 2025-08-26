import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompanyDetailComponent } from './company/company-detail/company-detail.component';
import { CompanyGetAllComponent } from './company/company-getAll/company-getAll.component';
import { CompanyAddComponent } from './company/company-add/company-add.component';

const routes: Routes = [
  { path: 'company-details/:id', component: CompanyDetailComponent },
  { path: 'company-getall', component: CompanyGetAllComponent },
  { path: 'company-add', component: CompanyAddComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompanyRoutingModule { }
 