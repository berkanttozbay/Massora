import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FuelHistoriesComponent } from './fuelhistories/fuelhistories-getAll/fuelhistories.component';
import { FuelHistoriesRoutingModule } from './fuelhistories-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { AddFuelHistoryModalComponent } from './fuelhistories/fuelhistories-add/fuelhistories.component';

@NgModule({
  declarations: [FuelHistoriesComponent,AddFuelHistoryModalComponent],

  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FuelHistoriesRoutingModule,
    SharedModule
    
  ],
  exports: [FuelHistoriesComponent,AddFuelHistoryModalComponent]
})
export class FuelHistoriesModule { }
