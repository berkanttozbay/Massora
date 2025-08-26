import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { DatatableActionModel } from '../../../../shared/models/massora-datatable/datatable-action.model';
import { PaginationModel } from '../../../../shared/models/massora-datatable/pagination.model';
import { WorkkHistoryService } from '../../../../shared/services/entities/workHistory/workHistory.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../../auth.service';
import { AddWorkHistoryModalComponent } from '../workhistories-add/workhistories.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';

interface WorkHistory {
  id: number;
  driverId : number;
  driverName: string;
  vehicleId: number;
  vehicleType: string;
  companyId: number;
  companyName: string;
  partnerCompanyId: number;
  date : Date;
  startTime: Date;
  endTime: Date;
  calculatedDriverFee: number;
  calculatedPartnerFee: number;
  address: string;
  createdDate: Date;
}

@Component({
  selector: 'app-workhistories',
  templateUrl: './workhistories.component.html',
  styleUrls: ['./workhistories.component.css'],
  standalone: false
})
export class WorkHistoriesComponent implements OnInit {
  // Datatable'a gönderilecek ve filtrelenecek veri
    public datatableData: DatatableActionModel[] = [];
    public error: string | null = null;
    public workHistories: WorkHistory[] = [];
    // --- YENİ EKLENEN SAYFALAMA DEĞİŞKENLERİ ---
    public pagination: PaginationModel = new PaginationModel();
    public totalCount: number = 0;
    private searchTerm: string = '';
    public successMessage: string | null = null;
    public errorMessage: string | null = null;
    // -----------------------------------------

  constructor(private authService : AuthService,private modalService: NgbModal,private workHistoryService : WorkkHistoryService) {}
    ngOnInit(): void {
        
    this.getAllWorkHistories();
  }

  getAllWorkHistories(): void {

    this.workHistoryService.getWorkHistoriesPaginated(
      this.pagination.pageNumber,
      this.pagination.pageSize,
      this.searchTerm
    ).subscribe({
      next: (response: { items: WorkHistory[]; totalCount: number; }) => {
        this.workHistories = response.items;
        this.totalCount = response.totalCount;

        // Veriyi datatable formatına dönüştür
        this.datatableData = this.workHistories.map(workHistory => this.transformVehicleToDatatableModel(workHistory));
        this.error = null;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Veriler alınırken bir hata oluştu.';
      }
    });
    
  }

  private transformVehicleToDatatableModel(workHistory: WorkHistory): DatatableActionModel {
      const actionModel = new DatatableActionModel();
      
      actionModel.id = workHistory.id.toString();
      actionModel.name = workHistory.driverId.toString();
      // actionModel.status = true; // Örnek olarak tüm araçları 'Aktif' varsayalım
      actionModel.icon = '<i class="fas fa-truck"></i>'; // FontAwesome ikonu
      
      actionModel.columns = [
        { columnName: 'Vehicle', columnValue: workHistory.vehicleType },
        { columnName: 'Company', columnValue: workHistory.companyName },
        { columnName: 'Driver', columnValue: workHistory.driverName },
        { columnName: 'Start Time', columnValue: `${workHistory.startTime.toString()}` },
        { columnName: 'End Time', columnValue: `${workHistory.endTime.toString()}` },
        { columnName: 'Driver Fee', columnValue: `${workHistory.calculatedDriverFee} TRY/saat` },
        { columnName: 'Partner Fee', columnValue: `${workHistory.calculatedPartnerFee} TRY/saat` },
        { columnName: 'Address', columnValue: `${workHistory.address}` },
        
      ];
  
      actionModel.boxes = [
        { boxName: 'Oluşturulma Tarihi', boxDate: workHistory.createdDate.toString(), boxHour: new Date(workHistory.createdDate).toLocaleTimeString() }
      ];
  
      return actionModel;
    }
  
    /**
     * Datatable'dan gelen arama olayını dinler ve verileri yeniden çeker.
     */
    public handleSearch(searchTerm: string): void {
      this.searchTerm = searchTerm;
      this.pagination.pageNumber = 1; // Arama yapıldığında ilk sayfaya dön
      this.getAllWorkHistories();
    }
  
    /**
     * Datatable'dan gelen sayfa değiştirme olayını dinler.
     */
    public onPageChange(newPage: number): void {
      if (this.pagination.pageNumber !== newPage) {
        this.pagination.pageNumber = newPage;
        this.getAllWorkHistories();
      }
    }
  
    /**
     * Datatable'dan gelen "Yeni Ekle" olayını dinler.
     */
    public handleAdd(): void {
      console.log('Parent component: Yeni araç ekleme işlemi tetiklendi!');
      // Örnek: this.router.navigate(['/vehicles/new']);
    }

    openAddWorkHistoryModal(): void {
            const modalRef = this.modalService.open(AddWorkHistoryModalComponent, { centered: true });
            // Artık modal'a companyId göndermiyoruz.
        
            modalRef.result.then((result) => {
                // Gelen 'result' içinde companyId yok, ama sorun değil!
                // vehicleService.add metodu bu şekilde çağrılacak.
                this.workHistoryService.add(result).subscribe({
                  next: () => {
                    console.log('WorkHistory başarıyla eklendi.');
                    this.getAllWorkHistories(); // Listeyi yenile!
                  }
                });
              });
          }

  openEditWorkHistoryModal(workHistorySummary: any): void {
    this.workHistoryService.getById(workHistorySummary.id).subscribe(fullWorkHistoryData => {
      
      const modalRef = this.modalService.open(AddWorkHistoryModalComponent, { centered: true });
      
      // Bu satır, veriyi modal'daki 'vehicleToEdit' @Input'una gönderir.
      modalRef.componentInstance.workHistoryToEdit = fullWorkHistoryData;

      modalRef.result.then(result => {
      // 'result' burada modal'dan dönen {id: 8, licensePlate: '...', ...} gibi bir nesnedir.
      
      if (result && result.id) {
          
          // DÜZELTME: İlk parametre olarak sadece result.id'yi gönderiyoruz.
          // İkinci parametre olarak nesnenin tamamını gönderiyoruz.
          this.workHistoryService.update(result.id, result).subscribe({
              next: () => { 
                  console.log('WorkHistory başarıyla güncellendi.');
                  this.getAllWorkHistories(); // Listeyi yenile
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
      this.workHistoryService.deletePermanently(id).subscribe({
        next: () => {
          // ▼▼▼ YENİ EKLENEN KISIM ▼▼▼
          this.successMessage = `${id} ID'li kayıt başarıyla silindi.`;
          // ▲▲▲ YENİ EKLENEN KISIM ▲▲▲

          this.getAllWorkHistories(); // Listeyi yenile

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
