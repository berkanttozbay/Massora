import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DriverGetAllComponent } from './driver/driver-getAll/driver-getAll.component';
import { DriverAddComponent } from './driver/driver-add/driver-add.component';

const routes: Routes = [
  { path: 'driver-getall', component: DriverGetAllComponent },
  { path: 'driver-add', component: DriverAddComponent },
 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DriverRoutingModule { }
 