import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FuelHistoriesComponent } from './fuelhistories/fuelhistories-getAll/fuelhistories.component';

const routes: Routes = [
  { path: '', component: FuelHistoriesComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FuelHistoriesRoutingModule { }
 