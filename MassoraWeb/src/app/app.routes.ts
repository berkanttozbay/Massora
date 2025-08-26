import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SharedRoutingModule } from './shared/shared-routing.module';
import { VehicleAddComponent } from './Massora/vehicle-module/vehicle/vehicle-add/vehicle-add.component';
import { VehicleGetAllComponent } from './Massora/vehicle-module/vehicle/vehicle-getAll/vehicle-getAll.component';


export const routes: Routes = [
  { path: '', redirectTo: 'vehicles', pathMatch: 'full' }, 
  
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    SharedRoutingModule

  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
