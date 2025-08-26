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
  selector: 'app-company-detail',
  templateUrl: './company-detail.component.html',
  styleUrls: ['./company-detail.component.css'],
  standalone: false
})
export class CompanyDetailComponent implements OnInit {
  companies: Company[] = [];
  error: string | null = null;

  constructor(private http: HttpClient,private route: ActivatedRoute) {}

  ngOnInit(): void {
    // Route parametresini al
    this.route.paramMap.subscribe((params: ParamMap) => {
      const idStr = params.get('id');
      if (idStr) {
        const id = +idStr;
        this.getCompaniesByResponsibleId(id);
        
      } else {
        this.error = 'ID parametresi bulunamadı.';
        this.companies = [];
      }
      
    });
  }
  

  getCompaniesByResponsibleId(id: number): void {
    this.http.get<Company[]>(`http://localhost:5260/api/company/responsible-user/${id}`)
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
