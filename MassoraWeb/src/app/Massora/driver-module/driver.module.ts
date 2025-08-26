import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DriverGetAllComponent } from './driver/driver-getAll/driver-getAll.component';
import { DriverRoutingModule } from './driver-routing.module';
import { DriverAddComponent } from './driver/driver-add/driver-add.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [DriverGetAllComponent,DriverAddComponent],

  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DriverRoutingModule,
    SharedModule
  ],
  exports: [DriverGetAllComponent,DriverAddComponent]
})
export class DriverModule { }
