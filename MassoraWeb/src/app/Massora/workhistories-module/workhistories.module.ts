import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { WorkHistoriesComponent } from './workhistories/workhistories-getAll/workhistories.component';
import { VehicleRoutingModule } from '../vehicle-module/vehicle-routing.module';
import { WorkHistoriesRoutingModule } from './workhistories-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { AddWorkHistoryModalComponent } from './workhistories/workhistories-add/workhistories.component';

@NgModule({
  declarations: [WorkHistoriesComponent,AddWorkHistoryModalComponent],

  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    WorkHistoriesRoutingModule,
    SharedModule
  ],
  exports: [WorkHistoriesComponent,AddWorkHistoryModalComponent]
})
export class WorkHistoriesModule { }
