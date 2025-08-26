import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseApiRequestService } from '../../base/base-api-request.service';
import { DashboardStatsModel } from '../../../models/entities/dashboard-stats.model';

@Injectable({
  providedIn: 'root',
})
export class DashboardService extends BaseApiRequestService<any> {

  constructor(protected override httpClient: HttpClient) {
    super(httpClient);
    // Bu servisin ana endpoint'i
    this.apiUri = 'dashboard'; 
  }

  public getStats(): Observable<DashboardStatsModel> {
    const url = this.getApiUri(`${this.apiUri}/stats`);
    return this.httpClient.get<DashboardStatsModel>(url);
  }
}