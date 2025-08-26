import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'; 
import { VehicleModel } from '../../../../shared/models/entities/vehicle.model';


// Interface'i ayrı bir model dosyasına taşımanız daha iyi olur, ama şimdilik burada kalabilir.
interface Vehicle {
    companyId: number;
    vehicleType: string;
    licensePlate: string;
    hourlyWageDriver: number;
    hourlyWagePartner: number;
    createdDate?: string;
    updatedDate?: string; 
}

@Component({
  selector: 'app-vehicle-add',
  templateUrl: './vehicle-add.component.html',
  styleUrls: ['./vehicle-add.component.css'],
  standalone: false,
})
export class VehicleAddComponent implements OnInit {
  @Input() vehicleToEdit: VehicleModel | null = null;
  newVehicle: Partial<VehicleModel> = {}; 
  vehicleForm: Partial<VehicleModel> = {};
  modalTitle: string = 'Add Vehicle';

  constructor(public activeModal: NgbActiveModal) {}
  ngOnInit(): void {
    // Eğer dışarıdan düzenlenecek bir araç verisi geldiyse...
    if (this.vehicleToEdit) {
      this.modalTitle = 'Edit Vehicle';
      // ...formu o aracın verileriyle doldur.
      this.vehicleForm = { ...this.vehicleToEdit }; 
    }
  }
  onSubmit(form: any): void {
    if (form.valid) {
      // Formdaki son veriyi geri döndür
      this.activeModal.close(this.vehicleForm);
    }
  }
}
