import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { DatatableActionModel, ColumnListModel, ColumnBoxModel } from '../../../../shared/models/massora-datatable/datatable-action.model';
import { PaginationModel } from '../../../../shared/models/massora-datatable/pagination.model';
import { PaginationResultModel } from '../../../../shared/models/massora-datatable/pagination-result.model';
import { VehicleModel } from '../../../../shared/models/entities/vehicle.model';
import { VehicleService } from '../../../../shared/services/entities/vehicle/vehicle.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { VehicleAddComponent } from '../vehicle-add/vehicle-add.component';
import { AuthService } from '../../../../auth.service';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';

// API'den gelen orijinal veri yapısı
interface Vehicle {
  id: number;
  companyId: number;
  companyName: string;
  vehicleType: string;
  hourlyWageDriver: number;
  hourlyWagePartner: number;
  licensePlate: string;
  createdDate: Date;
}

@Component({
  selector: 'app-vehicle-getall',
  templateUrl: './vehicle-getAll.component.html',
  styleUrls: ['./vehicle-getAll.component.css'],
  standalone: false,
})
export class VehicleGetAllComponent implements OnInit {

  public datatableData: DatatableActionModel[] = [];
  public error: string | null = null;
  public vehicles: VehicleModel[] = []; // Tipi VehicleModel olarak güncellendi
  public pagination: PaginationModel = new PaginationModel();
  public totalCount: number = 0;
  private searchTerm: string = '';
  public successMessage: string | null = null;
  public errorMessage: string | null = null;
  // HttpClient silindi, yerine VehicleService enjekte edildi.
  constructor(private authService : AuthService,private modalService: NgbModal,private vehicleService: VehicleService) {}

  ngOnInit(): void {
    this.getAllVehicles();
  }

  getAllVehicles(): void {
    // Tüm HttpParams ve veri kontrol mantığı artık serviste.
    // Component sadece servisi çağırır.
    this.vehicleService.getVehiclesPaginated(
      this.pagination.pageNumber,
      this.pagination.pageSize,
      this.searchTerm
    ).subscribe({
      next: (response: { items: VehicleModel[]; totalCount: number; }) => {
        // Güçlü tipler sayesinde (any yerine) gelen verinin formatından eminiz.
        this.vehicles = response.items;
        this.totalCount = response.totalCount;

        // Veriyi datatable formatına dönüştür
        this.datatableData = this.vehicles.map(vehicle => this.transformVehicleToDatatableModel(vehicle));
        this.error = null;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Araç verileri alınamadı.';
        this.datatableData = [];
        this.totalCount = 0;
      }
    });
  }

  /**
   * Tek bir Vehicle nesnesini DatatableActionModel formatına dönüştürür.
   */
  private transformVehicleToDatatableModel(vehicle: Vehicle): DatatableActionModel {
    const actionModel = new DatatableActionModel();
    
    actionModel.id = vehicle.id.toString();
    actionModel.name = vehicle.licensePlate;
    actionModel.status = true; // Örnek olarak tüm araçları 'Aktif' varsayalım
    actionModel.icon = '<i class="fas fa-truck"></i>'; // FontAwesome ikonu
    
    
    actionModel.columns = [
      { columnName: 'LicensePlate', columnValue: vehicle.licensePlate },
      { columnName: 'Company', columnValue: vehicle.companyName },
      { columnName: 'Vehicle Type', columnValue: vehicle.vehicleType },
      { columnName: 'Driver Fee', columnValue: `${vehicle.hourlyWageDriver} TRY/saat` },
      { columnName: 'Partner Fee', columnValue: `${vehicle.hourlyWagePartner} TRY/saat` },
      
    ];

    actionModel.boxes = [
      { boxName: 'Oluşturulma Tarihi', boxDate: vehicle.createdDate.toString(), boxHour: new Date(vehicle.createdDate).toLocaleTimeString() }
    ];

    return actionModel;
  }

  /**
   * Datatable'dan gelen arama olayını dinler ve verileri yeniden çeker.
   */
  public handleSearch(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.pagination.pageNumber = 1; // Arama yapıldığında ilk sayfaya dön
    this.getAllVehicles();
  }

  /**
   * Datatable'dan gelen sayfa değiştirme olayını dinler.
   */
  public onPageChange(newPage: number): void {
    if (this.pagination.pageNumber !== newPage) {
      this.pagination.pageNumber = newPage;
      this.getAllVehicles();
    }
  }

  /**
   * Datatable'dan gelen "Yeni Ekle" olayını dinler.
   */
  public handleAdd(): void {
    console.log('Parent component: Yeni araç ekleme işlemi tetiklendi!');
    // Örnek: this.router.navigate(['/vehicles/new']);
  }

  public totalPages(): number {
    return Math.ceil(this.totalCount / this.pagination.pageSize);
  }
  openAddVehicleModal(): void {
    const modalRef = this.modalService.open(VehicleAddComponent, { centered: true });
    // Artık modal'a companyId göndermiyoruz.

    modalRef.result.then((result) => {
        // Gelen 'result' içinde companyId yok, ama sorun değil!
        // vehicleService.add metodu bu şekilde çağrılacak.
        this.vehicleService.add(result).subscribe({
          next: () => {
            console.log('Araç başarıyla eklendi.');
            this.getAllVehicles(); // Listeyi yenile!
          }
        });
      });
  }
  openEditVehicleModal(vehicleSummary: any): void {
  this.vehicleService.getById(vehicleSummary.id).subscribe(fullVehicleData => {
    
    const modalRef = this.modalService.open(VehicleAddComponent, { centered: true });
    
    // Bu satır, veriyi modal'daki 'vehicleToEdit' @Input'una gönderir.
    modalRef.componentInstance.vehicleToEdit = fullVehicleData;

    modalRef.result.then(result => {
    // 'result' burada modal'dan dönen {id: 8, licensePlate: '...', ...} gibi bir nesnedir.
    
    if (result && result.id) {
        
        // DÜZELTME: İlk parametre olarak sadece result.id'yi gönderiyoruz.
        // İkinci parametre olarak nesnenin tamamını gönderiyoruz.
        this.vehicleService.update(result.id, result).subscribe({
            next: () => {
                console.log('Araç başarıyla güncellendi.');
                this.getAllVehicles(); // Listeyi yenile
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
    this.vehicleService.deletePermanently(id).subscribe({
       next: () => {
        // ▼▼▼ YENİ EKLENEN KISIM ▼▼▼
        this.successMessage = `${id} ID'li kayıt başarıyla silindi.`;
        // ▲▲▲ YENİ EKLENEN KISIM ▲▲▲

        this.getAllVehicles(); // Listeyi yenile

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