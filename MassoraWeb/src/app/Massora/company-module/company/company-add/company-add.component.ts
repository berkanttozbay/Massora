import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

interface Company {
  id: number;
  name: string;
  contactEmail: string;
  contactPhone: string;
  address: string;
  responsibleUserId: number;
  createdAt: string;
  updatedAt: string;
}
@Component({
  selector: 'app-company-add',
  templateUrl: './company-add.component.html',
  styleUrls: ['./company-add.component.css'],
  standalone: false,
})
export class CompanyAddComponent {
  company: Company = {
    name: '',
    contactEmail: '',
    contactPhone: '',
    address: '',
    responsibleUserId: 0,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    id: 0
  };

  successMessage: string = '';
  errorMessage: string = '';

  constructor(
    private http: HttpClient,
  ) {}

  onSubmit(form:any): void {
    if (form.valid) {
      
      this.http.post('http://localhost:5260/api/company', this.company)
        .subscribe({
          next: () => {
            this.successMessage = 'Şirket başarıyla eklendi!';
            this.errorMessage = '';
            form.reset();
            // this.router.navigate(['/company-details']); // istersen yönlendir
          },
          error: (err) => {
            console.error(err);
            this.errorMessage = 'Şirket eklenirken bir hata oluştu.';
          }
        });
    }
  }
}
