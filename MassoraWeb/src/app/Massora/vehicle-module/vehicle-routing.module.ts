import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VehicleGetAllComponent } from './vehicle/vehicle-getAll/vehicle-getAll.component';
import { VehicleAddComponent } from './vehicle/vehicle-add/vehicle-add.component';
import { AuthGuard } from '../../guardService/auth.guard';

const routes: Routes = [
  { path: 'vehicle-getall', component: VehicleGetAllComponent},
  { path: 'vehicle-add', component: VehicleAddComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VehicleRoutingModule { }
 