import { Component, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PartnerCompanyModel } from '../../../../shared/models/entities/partnercompany.model';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'app-company-add',
  templateUrl: './partnerCompanies-add.component.html',
  styleUrls: ['./partnerCompanies-add.component.css'],
  standalone: false,
})
export class PartnerCompanyAddComponent {

  newPartnerCompany: Partial<PartnerCompanyModel> = {}; 
   @Input() partnerCompanyToEdit: PartnerCompanyModel | null = null;
   partnerCompanyForm: Partial<PartnerCompanyModel> = {};
    modalTitle: string = 'Add Partner Company';

  constructor(public activeModal: NgbActiveModal) {}
  ngOnInit(): void {
    // Eğer dışarıdan düzenlenecek bir araç verisi geldiyse...
    if (this.partnerCompanyToEdit) {
      this.modalTitle = 'Edit Partner Company';
      // ...formu o aracın verileriyle doldur.
      this.partnerCompanyForm = { ...this.partnerCompanyToEdit }; 
    }
  }
  onSubmit(form: any): void {
    if (form.valid) {
      this.activeModal.close(this.partnerCompanyForm);
    }
    
  }
}
