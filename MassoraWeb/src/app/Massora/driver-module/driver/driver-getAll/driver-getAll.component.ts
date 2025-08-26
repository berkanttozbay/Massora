import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { DatatableActionModel } from '../../../../shared/models/massora-datatable/datatable-action.model';
import { PaginationModel } from '../../../../shared/models/massora-datatable/pagination.model';
import { DriverService } from '../../../../shared/services/entities/driver/driver.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../../auth.service';
import { DriverAddComponent } from '../driver-add/driver-add.component';

interface Driver {
  id: number;
  responsibleUserId: number;
  name: string;
  companyId: number;
  companyName: string;
  email: string;
  phone: string;
  birthDate: Date;
  gender: string;
  createdDate: Date;
  updatedDate: Date;
  vehicleId: number;
  vehicleType : string;
  
}

@Component({
  selector: 'app-vehicle-getall',
  templateUrl: './driver-getAll.component.html',
  styleUrls: ['./driver-getAll.component.css'],
  standalone: false
})
export class DriverGetAllComponent implements OnInit {
  
    // Datatable'a gönderilecek ve filtrelenecek veri
    public datatableData: DatatableActionModel[] = [];
    public error: string | null = null;
    public drivers: Driver[] = [];
    // --- YENİ EKLENEN SAYFALAMA DEĞİŞKENLERİ ---
    public pagination: PaginationModel = new PaginationModel();
    public totalCount: number = 0;
    private searchTerm: string = '';
    // -----------------------------------------

  constructor(private authService : AuthService,private modalService: NgbModal,private driverService : DriverService) {}
    ngOnInit(): void {
        
    this.getAllDrivers();
  }

  getAllDrivers(): void {
   this.driverService.getDriversPaginated(
      this.pagination.pageNumber,
      this.pagination.pageSize,
      this.searchTerm
    ).subscribe({
      next: (response: { items: Driver[]; totalCount: number; }) => {
        this.drivers = response.items;
        this.totalCount = response.totalCount;

        // Veriyi datatable formatına dönüştür
        this.datatableData = this.drivers.map(driver => this.transformVehicleToDatatableModel(driver));
        this.error = null;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Veriler alınırken bir hata oluştu.';
      }
    });
   
}

  private transformVehicleToDatatableModel(driver: Driver): DatatableActionModel {
      const actionModel = new DatatableActionModel();
      
      actionModel.id = driver.id.toString();
      actionModel.name = driver.name;
      actionModel.status = true; // Örnek olarak tüm araçları 'Aktif' varsayalım
      actionModel.icon = '<i class="fas fa-truck"></i>'; // FontAwesome ikonu
      actionModel.avatar = ''; 
      
      actionModel.columns = [
        { columnName: 'Driver', columnValue: driver.name },
        { columnName: 'Company', columnValue: driver.companyName },
        { columnName: 'Email', columnValue: driver.email },
        { columnName: 'Contact Phone', columnValue: `${driver.phone} ` },
        { columnName: 'Gender', columnValue: `${driver.gender} ` },
        { columnName: 'Birth Date', columnValue: driver.birthDate.toString() },
        
      
        
        
      ];

      actionModel.boxes = [
        { boxName: 'Oluşturulma Tarihi', boxDate: driver.createdDate.toString(), boxHour: new Date(driver.createdDate).toLocaleTimeString() }
      ];

      return actionModel;
    }

  public handleSearch(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.pagination.pageNumber = 1; // Arama yapıldığında ilk sayfaya dön
    this.getAllDrivers();
  }
  public onPageChange(newPage: number): void {
    if (this.pagination.pageNumber !== newPage) {
      this.pagination.pageNumber = newPage;
      this.getAllDrivers();
    }
  }
    public handleAdd(): void {
    console.log('Parent component: Yeni araç ekleme işlemi tetiklendi!');
    // Örnek: this.router.navigate(['/vehicles/new']);
  }

  openAddDriverModal(): void {
      const modalRef = this.modalService.open(DriverAddComponent, { centered: true });
      // Artık modal'a companyId göndermiyoruz.
  
      modalRef.result.then((result) => {
          // Gelen 'result' içinde companyId yok, ama sorun değil!
          // vehicleService.add metodu bu şekilde çağrılacak.
          this.driverService.add(result).subscribe({
            next: () => {
              console.log('Araç başarıyla eklendi.');
              this.getAllDrivers(); // Listeyi yenile!
            }
          });
        });
    }
    openEditDriverModal(driverSummary: any): void {
      this.driverService.getById(driverSummary.id).subscribe(fullDriverData => {
         console.log('API\'den gelen tam sürücü verisi:', fullDriverData);
        const modalRef = this.modalService.open(DriverAddComponent, { centered: true });
        
        // Bu satır, veriyi modal'daki 'vehicleToEdit' @Input'una gönderir.
        modalRef.componentInstance.driverToEdit = fullDriverData;
    
        modalRef.result.then(result => {
        // 'result' burada modal'dan dönen {id: 8, licensePlate: '...', ...} gibi bir nesnedir.
        
        if (result && result.id) {
            
            // DÜZELTME: İlk parametre olarak sadece result.id'yi gönderiyoruz.
            // İkinci parametre olarak nesnenin tamamını gönderiyoruz.
            this.driverService.update(result.id, result).subscribe({
                next: () => {
                    console.log('Driver başarıyla güncellendi.');
                    this.getAllDrivers(); // Listeyi yenile
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
}
