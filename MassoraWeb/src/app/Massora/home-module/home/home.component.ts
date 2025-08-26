import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../auth.service';
import { DashboardService } from '../../../shared/services/entities/dashboard/dashboard.service';
import { DashboardStatsModel } from '../../../shared/models/entities/dashboard-stats.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  standalone: false,

})
export class HomeComponent implements OnInit{
  stats$!: Observable<DashboardStatsModel>;
  error: string | null = null;
  constructor(private dashboardService: DashboardService,private authService: AuthService) {
    
  }
  ngOnInit(): void {
    this.stats$ = this.dashboardService.getStats();
  }
  logout() {
    this.authService.logout();
  }
}
