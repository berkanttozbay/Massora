import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { VehicleService } from '../../../../shared/services/entities/vehicle/vehicle.service';
import { DriverService } from '../../../../shared/services/entities/driver/driver.service';
import { FuelHistoryModel } from '../../../../shared/models/entities/fuelhistory.model';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

interface DropdownModel {
  id: number;
  name: string;
}

@Component({
  selector: 'app-addfuelhistories',
  templateUrl: './fuelhistories.component.html',
  styleUrls: ['./fuelhistories.component.css'],
  standalone: false,
})
export class AddFuelHistoryModalComponent implements OnInit {
  
  @Input() fuelHistoryToEdit: FuelHistoryModel | null = null;
  
  // SADECE BU NESNEYİ KULLANACAĞIZ
  fuelHistoryForm: Partial<FuelHistoryModel> = {};
  
  modalTitle: string = 'Add Fuel History';
  vehicles$!: Observable<DropdownModel[]>;
  drivers$!: Observable<DropdownModel[]>;

  constructor(
    public activeModal: NgbActiveModal,
    private vehicleService: VehicleService,
    private driverService: DriverService
  ) {}

  ngOnInit(): void {
    if (this.fuelHistoryToEdit) {
      this.modalTitle = 'Edit Fuel History';
      const formData = { ...this.fuelHistoryToEdit };
      const eventDate = new Date(formData.date);
      
      const year = eventDate.getFullYear();
      const month = (eventDate.getMonth() + 1).toString().padStart(2, '0');
      const day = eventDate.getDate().toString().padStart(2, '0');
      const datePart = `${year}-${month}-${day}`;
      
      const hours = eventDate.getHours().toString().padStart(2, '0');
      const minutes = eventDate.getMinutes().toString().padStart(2, '0');
      const timePart = `${hours}:${minutes}`;

      formData.date = datePart;
      formData.time = timePart;
      
      this.fuelHistoryForm = formData;
    }
    
    this.vehicles$ = this.vehicleService.getForDropdown();
    this.drivers$ = this.driverService.getForDropdown();
  }

  onSubmit(form: any): void {
    if (form.valid) {
      // ▼▼▼ DÜZELTME BURADA ▼▼▼
      // Veriyi 'newFuelHistory' yerine, forma bağlı olan 'fuelHistoryForm'dan alıyoruz.
      const datePart = this.fuelHistoryForm.date;
      const timePart = this.fuelHistoryForm.time;
      // ▲▲▲ DÜZELTME BİTTİ ▲▲▲

      const fullIsoDateTime = `${datePart}T${timePart}:00`;

      const dataToSend = {
        ...this.fuelHistoryForm,
        id: this.fuelHistoryToEdit?.id,
        date: fullIsoDateTime,
        time: fullIsoDateTime,
        // ID'lerin number olduğundan emin olalım
        vehicleId: parseInt(String(this.fuelHistoryForm.vehicleId), 10),
        driverId: parseInt(String(this.fuelHistoryForm.driverId), 10),
      };

      console.log('APIye Gönderilecek Veri:', dataToSend);
      this.activeModal.close(dataToSend);
    }
  }
}