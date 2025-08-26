import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { DatatableActionModel } from '../../../../shared/models/massora-datatable/datatable-action.model';
import { PaginationModel } from '../../../../shared/models/massora-datatable/pagination.model';
import { FuelHistoryService } from '../../../../shared/services/entities/fuelHistory/fuelHistory.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../../auth.service';
import { AddFuelHistoryModalComponent } from '../fuelhistories-add/fuelhistories.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';

interface FuelHistory {
  id: number;
  vehicleId: number;
  vehicleType: string;
  driverId : number ;
  driverName: string;
  fuelCompany : string;
  liter : number;
  fee : number;
  date: Date;
  time : Date;
  createdDate: Date;
}

@Component({
  selector: 'app-fuelhistories',
  templateUrl: './fuelhistories.component.html',
  styleUrls: ['./fuelhistories.component.css'],
  standalone: false
})
export class FuelHistoriesComponent implements OnInit {
  // Datatable'a gönderilecek ve filtrelenecek veri
    public datatableData: DatatableActionModel[] = [];
    public error: string | null = null;
    public fuelHistories: FuelHistory[] = [];
    // --- YENİ EKLENEN SAYFALAMA DEĞİŞKENLERİ ---
    public pagination: PaginationModel = new PaginationModel();
    public totalCount: number = 0;
    private searchTerm: string = '';
    public successMessage: string | null = null;
    public errorMessage: string | null = null;
    // -----------------------------------------
  

  constructor(private authService : AuthService,private modalService: NgbModal,private fuelHistoryService : FuelHistoryService) {}
    ngOnInit(): void {
        
    this.getAllFuelHistories();
  }

  getAllFuelHistories(): void {

    // API isteği için parametreleri hazırlıyoruz
    this.fuelHistoryService.getFuelHistoriesPaginated(
      this.pagination.pageNumber,
      this.pagination.pageSize,
      this.searchTerm
    ).subscribe({
      next: (response: { items: FuelHistory[]; totalCount: number; }) => {
        this.fuelHistories = response.items;
        this.totalCount = response.totalCount;

        // Veriyi datatable formatına dönüştür
        this.datatableData = this.fuelHistories.map(fuelHistory => this.transformVehicleToDatatableModel(fuelHistory));
        this.error = null;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Veriler alınırken bir hata oluştu.';
      }
    });
      
  }

  /**
     * Tek bir Vehicle nesnesini DatatableActionModel formatına dönüştürür.
     */
    private transformVehicleToDatatableModel(fuelHistory: FuelHistory): DatatableActionModel {
      const actionModel = new DatatableActionModel();
      
      actionModel.id = fuelHistory.id.toString();
      actionModel.name = fuelHistory.driverId.toString();
      actionModel.icon = '<i class="fas fa-truck"></i>'; // FontAwesome ikonu
      
      actionModel.columns = [
        { columnName: 'Driver', columnValue: fuelHistory.driverName },
        { columnName: 'Vehicle', columnValue: fuelHistory.vehicleType },
        { columnName: 'Fuel Company', columnValue: `${fuelHistory.fuelCompany} ` },
        { columnName: 'Liter', columnValue: `${fuelHistory.liter}` },
        { columnName: 'Fee', columnValue: `${fuelHistory.fee} TRY/saat` },
        { columnName: 'Time', columnValue: `${fuelHistory.time}` },
        
      ];
  
      actionModel.boxes = [
        { boxName: 'Oluşturulma Tarihi', boxDate: fuelHistory.createdDate.toString(), boxHour: new Date(fuelHistory.createdDate).toLocaleTimeString() }
      ];
  
      return actionModel;
    }
  
    /**
     * Datatable'dan gelen arama olayını dinler ve verileri yeniden çeker.
     */
    public handleSearch(searchTerm: string): void {
      this.searchTerm = searchTerm;
      this.pagination.pageNumber = 1; // Arama yapıldığında ilk sayfaya dön
      this.getAllFuelHistories();
    }
  
    /**
     * Datatable'dan gelen sayfa değiştirme olayını dinler.
     */
    public onPageChange(newPage: number): void {
      if (this.pagination.pageNumber !== newPage) {
        this.pagination.pageNumber = newPage;
        this.getAllFuelHistories();
      }
    }
  
    /**
     * Datatable'dan gelen "Yeni Ekle" olayını dinler.
     */
    public handleAdd(): void {
      console.log('Parent component: Yeni araç ekleme işlemi tetiklendi!');
      // Örnek: this.router.navigate(['/vehicles/new']);
    }
    openAddVehicleFuelHistoryModal(): void {
        const modalRef = this.modalService.open(AddFuelHistoryModalComponent, { centered: true });
        // Artık modal'a companyId göndermiyoruz.
    
        modalRef.result.then((result) => {
            // Gelen 'result' içinde companyId yok, ama sorun değil!
            // vehicleService.add metodu bu şekilde çağrılacak.
            this.fuelHistoryService.add(result).subscribe({
              next: () => {
                console.log('Araç başarıyla eklendi.');
                this.getAllFuelHistories(); // Listeyi yenile!
              }
            });
          });
      }
      openEditFuelHistoryModal(fuelHistorySummary: any): void {
        this.fuelHistoryService.getById(fuelHistorySummary.id).subscribe(fullFuelHistoryData => {
          
          const modalRef = this.modalService.open(AddFuelHistoryModalComponent, { centered: true });
          
          // Bu satır, veriyi modal'daki 'vehicleToEdit' @Input'una gönderir.
          modalRef.componentInstance.fuelHistoryToEdit = fullFuelHistoryData;
      
          modalRef.result.then(result => {
          // 'result' burada modal'dan dönen {id: 8, licensePlate: '...', ...} gibi bir nesnedir.
          
          if (result && result.id) {
              
              // DÜZELTME: İlk parametre olarak sadece result.id'yi gönderiyoruz.
              // İkinci parametre olarak nesnenin tamamını gönderiyoruz.
              this.fuelHistoryService.update(result.id, result).subscribe({
                  next: () => {
                      console.log('FuelHistory başarıyla güncellendi.');
                      this.getAllFuelHistories(); // Listeyi yenile
                  },
                  error: (err) => {
                      console.error('Güncelleme sırasında hata:', err);
                  }
              });
      
          } else {
              console.error('Güncellenecek kaydın ID\'si bulunamadı.', result);
          }
      });
        });
      }
  
handleDeleteRequest(item: any): void {
    // Onay modal'ını aç
    const modalRef = this.modalService.open(ConfirmationModalComponent, { centered: true });
    
    // Modal'a silinecek öğenin adını gönder (isteğe bağlı)
    modalRef.componentInstance.itemName = item.name;

    // Modal'dan gelecek olan sonucu (evet/hayır) dinle
    modalRef.result.then(
      (result) => {
        // Eğer kullanıcı "Evet" butonuna tıkladıysa (modal 'true' ile kapandıysa)
        if (result === true) {
          this.deleteItem(item.id);
        }
      },
      (reason) => {
        // Modal, "Hayır" veya dışarı tıklanarak kapatıldıysa
        console.log(`Silme işlemi iptal edildi: ${reason}`);
      }
    );
  }


  // API'ye silme isteğini gönderen metot
  private deleteItem(id: number): void {
    this.fuelHistoryService.deletePermanently(id).subscribe({
       next: () => {
        // ▼▼▼ YENİ EKLENEN KISIM ▼▼▼
        this.successMessage = `${id} ID'li kayıt başarıyla silindi.`;
        // ▲▲▲ YENİ EKLENEN KISIM ▲▲▲

        this.getAllFuelHistories(); // Listeyi yenile

        // Mesajın 3 saniye sonra kaybolmasını sağlayalım
        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        // ... hata yönetimi
        this.errorMessage = "Kayıt silinirken bir hata oluştu.";
        setTimeout(() => {
          this.errorMessage = null;
        }, 3000);
      }
    });
  }
  
}
