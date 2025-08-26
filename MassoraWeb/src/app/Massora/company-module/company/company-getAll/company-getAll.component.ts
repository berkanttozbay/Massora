import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, ParamMap } from '@angular/router';

interface Company {
  id: number;
  name: string;
  contactEmail: string;
  contactPhone: string;
  address: string;
  responsibleId: number;
  createdDate: Date;
  updatedDate: Date;
}

@Component({
  selector: 'app-company-getall',
  templateUrl: './company-getAll.component.html',
  styleUrls: ['./company-getAll.component.css'],
  standalone: false
})
export class CompanyGetAllComponent implements OnInit {
  companies: Company[] = [];
  error: string | null = null;

  constructor(private http: HttpClient,private route: ActivatedRoute) {}
    ngOnInit(): void {
        
    this.getAllCompanies();
  }

  getAllCompanies(): void {
    this.http.get<Company[]>(`http://localhost:5260/api/company`)
      .subscribe({
        next: (data) => {
          this.companies = data;
          this.error = null;
          console.log(this.companies);
        },
        error: (err) => {
          console.error(err);
          this.error = 'Şirket verileri alınamadı.';
          this.companies = [];
        }
      });
  }
}
