import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { VehicleService } from '../../../../shared/services/entities/vehicle/vehicle.service';
import { DriverModel } from '../../../../shared/models/entities/driver.model';
import { DropdownModel } from '../../../../shared/models/entities/dropdown';

@Component({
  selector: 'app-driver-add',
  templateUrl: './driver-add.component.html',
  styleUrls: ['./driver-add.component.css'],
  standalone: false,
})
export class DriverAddComponent implements OnInit {
  
  @Input() driverToEdit: DriverModel | null = null;
  driverForm: Partial<DriverModel> = {};
  
  modalTitle: string = 'Add Driver';
  vehicles$!: Observable<DropdownModel[]>;

  constructor(
    public activeModal: NgbActiveModal,
    private vehicleService: VehicleService
  ) {}

  ngOnInit(): void {
  if (this.driverToEdit) {
    this.modalTitle = 'Edit Driver';

    const formData = { ...this.driverToEdit };

    if (formData.vehicleId) {
      formData.vehicleId = Number(formData.vehicleId);
    }
    
    if (formData.birthDate) {
      formData.birthDate = new Date(formData.birthDate).toISOString().split('T')[0];
    }
    this.driverForm = formData;
  }
  
  this.vehicles$ = this.vehicleService.getForDropdown();
}

  onSubmit(form: any): void {
    if (form.valid) {
      const dataToSend = {
        id: this.driverToEdit?.id,
        vehicleId: parseInt(String(this.driverForm.vehicleId), 10)
      };
      console.log('APIye Gönderilecek Veri:', dataToSend);
      this.activeModal.close(dataToSend);
    }
  }
}