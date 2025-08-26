import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { DatatableActionModel } from '../../models/massora-datatable/datatable-action.model'; // Modellerin yolunu kendi projenize göre güncelleyin
@Component({
  selector: 'app-massora-datatable',
  templateUrl: './massora-datatable.component.html',
  styleUrls: ['./massora-datatable.component.css'],
  standalone: false,
})
export class MassoraDatatableComponent implements OnChanges {

  // --- MEVCUT INPUT/OUTPUT'LAR ---
  @Input() data: DatatableActionModel[] = []; // <-- YENİ EKLENEN @Input
  @Input() title: string = 'Veri Listesi';
  @Output() onAdd = new EventEmitter<void>();
  @Output() onSearch = new EventEmitter<string>();
  @Output() edit = new EventEmitter<any>();

  // ===================================================================
  // YENİ EKLENEN SAYFALAMA INPUT VE OUTPUT'LARI
  // ===================================================================
  @Input() totalCount: number = 0;
  @Input() pageSize: number = 10;
  @Input() pageNumber: number = 1;
  @Input() totalPages: number = 1; 
  @Output() pageChange = new EventEmitter<number>();
  @Output() addClicked = new EventEmitter<void>();
  @Output() deleteClicked = new EventEmitter<void>();
  // ===================================================================

  public filteredData: DatatableActionModel[] = [];
  public searchTerm: string = '';

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.filteredData = this.data || [];
    }
    // Toplam kayıt veya sayfa boyutu değiştiğinde, toplam sayfa sayısını yeniden hesapla
    if (changes['totalCount'] || changes['pageSize']) {
      this.calculateTotalPages();
    }
  }

  private calculateTotalPages(): void {
    if (this.totalCount > 0 && this.pageSize > 0) {
      this.totalPages = Math.ceil(this.totalCount / this.pageSize);
    } else {
      this.totalPages = 0;
    }
  }

  search(): void {
    this.onSearch.emit(this.searchTerm);
  }

  addNewItem(): void {
    this.onAdd.emit();
  }
  onEditClick(item: any): void {
  this.edit.emit(item);
}
onAddClick() {
  this.addClicked.emit();
}
onDeleteClick(item: any): void {
  this.deleteClicked.emit(item);
}

  /**
   * @param page Gidilecek yeni sayfa numarası
   */
  public goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.pageNumber) {
      this.pageChange.emit(page);
    }
  }
}
