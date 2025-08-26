import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from './components/massora-sidebar/massora-sidebar.component';
import { RouterModule } from '@angular/router';
import { MassoraDatatableComponent } from './components/massora-datatable/massora-datatable.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [SidebarComponent,MassoraDatatableComponent],
  imports: [CommonModule, RouterModule,FormsModule],
  exports: [SidebarComponent,MassoraDatatableComponent]
})
export class SharedModule {}
