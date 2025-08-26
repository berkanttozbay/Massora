import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { VehicleGetAllComponent } from './vehicle/vehicle-getAll/vehicle-getAll.component';
import { VehicleAddComponent } from './vehicle/vehicle-add/vehicle-add.component';
import { VehicleRoutingModule } from './vehicle-routing.module';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [VehicleGetAllComponent,VehicleAddComponent],

  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    VehicleRoutingModule,
    SharedModule
  ],
  exports: [VehicleGetAllComponent,VehicleAddComponent]
})
export class VehicleModule { }
