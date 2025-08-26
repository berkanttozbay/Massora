import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DatatableActionModel } from '../../../../shared/models/massora-datatable/datatable-action.model';
import { PaginationModel } from '../../../../shared/models/massora-datatable/pagination.model';
import { PartnerCompanyService } from '../../../../shared/services/entities/partnerCompany/partnerCompany.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../../auth.service';
import { PartnerCompanyAddComponent } from '../partnerCompanies-add/partnerCompanies-add.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';

interface PartnerCompany {
  id: number;
  companyId: number;
  companyName: string;
  name: string;
  contactPhone: string;
  contactEmail: string;
  address: string;
  createdDate: Date;
  updatedDate: Date;
}

@Component({
  selector: 'app-company-getall',
  templateUrl: './partnerCompanies-getAll.component.html',
  styleUrls: ['./partnerCompanies-getAll.component.css'],
  standalone: false
})
export class PartnerCompanyGetAllComponent implements OnInit {
  // Datatable'a gönderilecek ve filtrelenecek veri
    public datatableData: DatatableActionModel[] = [];
    public error: string | null = null;
    public partnerCompanies: PartnerCompany[] = [];
    // --- YENİ EKLENEN SAYFALAMA DEĞİŞKENLERİ ---
    public pagination: PaginationModel = new PaginationModel();
    public totalCount: number = 0;
    private searchTerm: string = '';
    public successMessage: string | null = null;
    public errorMessage: string | null = null;
    // -----------------------------------------

  constructor(private authService : AuthService,private modalService: NgbModal,private partnerCompanyService : PartnerCompanyService) {}
  
    ngOnInit(): void {
        
    this.getAllPartnerCompanies();
  }

  getAllPartnerCompanies(): void {
    // API isteği için parametreleri hazırlıyoruz
    this.partnerCompanyService.getPartnerCompaniesPaginated(
      this.pagination.pageNumber,
      this.pagination.pageSize,
      this.searchTerm
    ).subscribe({
      next: (response: { items: PartnerCompany[]; totalCount: number; }) => {
        this.partnerCompanies = response.items;
        this.totalCount = response.totalCount;

        // Veriyi datatable formatına dönüştür
        this.datatableData = this.partnerCompanies.map(partnerCompany => this.transformVehicleToDatatableModel(partnerCompany));
        this.error = null;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Veriler alınırken bir hata oluştu.';
      }
    });
  }

    private transformVehicleToDatatableModel(partnerCompany: PartnerCompany): DatatableActionModel {
      const actionModel = new DatatableActionModel();
      actionModel.id = partnerCompany.id.toString();
      actionModel.name = partnerCompany.name;
      actionModel.status = true


      actionModel.columns = [
      { columnName: 'PartnerCompany', columnValue: partnerCompany.name },
      { columnName: 'Company', columnValue: partnerCompany.companyName },
      { columnName: 'Address', columnValue: `${partnerCompany.address} ` },
      { columnName: 'Contact Email', columnValue: `${partnerCompany.contactEmail} ` },
      { columnName: 'Phone', columnValue: `${partnerCompany.contactPhone} ` },
      
    ];
    actionModel.boxes = [
      { boxName: 'Oluşturulma Tarihi', boxDate: partnerCompany.createdDate.toString(), boxHour: new Date(partnerCompany.createdDate).toLocaleTimeString() }
    ];
      return actionModel;

    }

     public handleSearch(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.pagination.pageNumber = 1; // Arama yapıldığında ilk sayfaya dön
    this.getAllPartnerCompanies();
  }

  public onPageChange(newPage: number): void {
    if (this.pagination.pageNumber !== newPage) {
      this.pagination.pageNumber = newPage;
      this.getAllPartnerCompanies();
    }
}
  public handleAdd(): void {
    console.log('Parent component: Yeni araç ekleme işlemi tetiklendi!');
    // Örnek: this.router.navigate(['/vehicles/new']);
  }
  openAddPartnerCompanyModal(): void {
      const modalRef = this.modalService.open(PartnerCompanyAddComponent, { centered: true });
      // Artık modal'a companyId göndermiyoruz.
  
      modalRef.result.then((result) => {
          // Gelen 'result' içinde companyId yok, ama sorun değil!
          // vehicleService.add metodu bu şekilde çağrılacak.
          this.partnerCompanyService.add(result).subscribe({
            next: () => {
              console.log('Araç başarıyla eklendi.');
              this.getAllPartnerCompanies(); // Listeyi yenile!
            }
          });
        });
    }
    openEditPartnerCompanyModal(partnerCompanySummary: any): void {
      this.partnerCompanyService.getById(partnerCompanySummary.id).subscribe(fullPartnerCompanyData => {
        
        const modalRef = this.modalService.open(PartnerCompanyAddComponent, { centered: true });
        
        // Bu satır, veriyi modal'daki 'vehicleToEdit' @Input'una gönderir.
        modalRef.componentInstance.partnerCompanyToEdit = fullPartnerCompanyData;
    
        modalRef.result.then(result => {
        // 'result' burada modal'dan dönen {id: 8, licensePlate: '...', ...} gibi bir nesnedir.
        
        if (result && result.id) {
            
            // DÜZELTME: İlk parametre olarak sadece result.id'yi gönderiyoruz.
            // İkinci parametre olarak nesnenin tamamını gönderiyoruz.
            this.partnerCompanyService.update(result.id, result).subscribe({
                next: () => {
                    console.log('Araç başarıyla güncellendi.');
                    this.getAllPartnerCompanies(); // Listeyi yenile
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
        this.partnerCompanyService.deletePermanently(id).subscribe({
           next: () => {
            // ▼▼▼ YENİ EKLENEN KISIM ▼▼▼
            this.successMessage = `${id} ID'li kayıt başarıyla silindi.`;
            // ▲▲▲ YENİ EKLENEN KISIM ▲▲▲
    
            this.getAllPartnerCompanies(); // Listeyi yenile
    
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
