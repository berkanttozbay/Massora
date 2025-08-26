import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WorkHistoriesComponent } from './workhistories/workhistories-getAll/workhistories.component';

const routes: Routes = [
  { path: '', component: WorkHistoriesComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WorkHistoriesRoutingModule { }
 