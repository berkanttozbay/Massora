import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { VehicleService } from '../../../../shared/services/entities/vehicle/vehicle.service';
import { DriverService } from '../../../../shared/services/entities/driver/driver.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { WorkHistoryModel } from '../../../../shared/models/entities/workhistory.model';
import { PartnerCompanyService } from '../../../../shared/services/entities/partnerCompany/partnerCompany.service';

// Backend'deki DropdownDto'ya karşılık gelen interface
interface DropdownModel {
  id: number;
  name: string;
}

@Component({
  selector: 'app-addworkhistories',
  templateUrl: './workhistories.component.html',
  styleUrls: ['./workhistories.component.css'],
  standalone: false,
})
export class AddWorkHistoryModalComponent implements OnInit {
  
  @Input() workHistoryToEdit: WorkHistoryModel | null = null;
  
  // SADECE BU TEK NESNEYİ KULLANACAĞIZ
  workHistoryForm: Partial<WorkHistoryModel> = {};
  
  modalTitle: string = 'Add Work History';
  vehicles$!: Observable<DropdownModel[]>;
  drivers$!: Observable<DropdownModel[]>;
  partners$!: Observable<DropdownModel[]>;

  constructor(
    public activeModal: NgbActiveModal,
    private vehicleService: VehicleService,
    private driverService: DriverService,
    private partnerCompanyService: PartnerCompanyService
  ) {}

  ngOnInit(): void {
    // Eğer bu bir DÜZENLEME işlemiyse...
    if (this.workHistoryToEdit) {
      this.modalTitle = 'Edit Work History';
      
      const formData = { ...this.workHistoryToEdit };

      // Backend'den gelen tam tarih/saat bilgisini (örn: startTime) al
      const startDate = new Date(formData.startTime);
      const endDate = new Date(formData.endTime);

      // Bu bilgiyi HTML input'larının anlayacağı formatlara böl
      const year = startDate.getFullYear();
      const month = (startDate.getMonth() + 1).toString().padStart(2, '0');
      const day = startDate.getDate().toString().padStart(2, '0');
      
      formData.date = `${year}-${month}-${day}`;
      formData.startTime = startDate.getHours().toString().padStart(2, '0') + ':' + startDate.getMinutes().toString().padStart(2, '0');
      formData.endTime = endDate.getHours().toString().padStart(2, '0') + ':' + endDate.getMinutes().toString().padStart(2, '0');
      
      // Son olarak, tamamen doğru formatlanmış bu nesneyi forma ata.
      this.workHistoryForm = formData;
    }
    
    // Dropdown'ları doldur
    this.vehicles$ = this.vehicleService.getForDropdown(); 
    this.drivers$ = this.driverService.getForDropdown();
    this.partners$ = this.partnerCompanyService.getForDropdown();
  }

  onSubmit(form: any): void {
    if (form.valid) {
      // Veriyi, forma bağlı olan DOĞRU nesneden alıyoruz.
      const datePart = this.workHistoryForm.date;
      const startPart = this.workHistoryForm.startTime;
      const endPart = this.workHistoryForm.endTime;

      // Tarih ve saatleri birleştirerek backend'in beklediği tam ISO formatını oluşturuyoruz.
      const fullIsoStartTime = `${datePart}T${startPart}:00`; 
      const fullIsoEndTime = `${datePart}T${endPart}:00`; 

      // Gönderilecek son veriyi hazırlıyoruz.
      const dataToSend = {
        ...this.workHistoryForm,
        id: this.workHistoryToEdit?.id,
        // ID'leri number formatına çeviriyoruz.
        vehicleId: parseInt(String(this.workHistoryForm.vehicleId), 10),
        driverId: parseInt(String(this.workHistoryForm.driverId), 10),
        partnerCompanyId: parseInt(String(this.workHistoryForm.partnerCompanyId), 10),
        // Tarih/saat alanlarını güncelliyoruz.
        startTime: fullIsoStartTime,
        endTime: fullIsoEndTime,
      };
      
      // Artık gereksiz olan 'date' alanını siliyoruz.
      delete dataToSend.date;

      console.log('APIye Gönderilecek Veri:', dataToSend);

      // Modal'ı SADECE BİR KEZ, doğru veriyle kapatıyoruz.
      this.activeModal.close(dataToSend);
    }
  }
}