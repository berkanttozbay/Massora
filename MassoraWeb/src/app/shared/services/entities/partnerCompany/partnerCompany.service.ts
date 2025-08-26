import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseApiRequestService } from '../../base/base-api-request.service'; // Base servisinizin yolu
import { VehicleModel } from '../../../models/entities/vehicle.model'; // Modelinizin yolu
import { PaginationResultModel } from '../../../models/massora-datatable/pagination-result.model'; // Modelinizin yolu
import { PartnerCompanyModel } from '../../../models/entities/partnercompany.model';
import { DropdownModel } from '../../../models/entities/dropdown';

@Injectable({
  providedIn: 'root',
})
export class PartnerCompanyService extends BaseApiRequestService<PartnerCompanyModel> {

  constructor(protected override httpClient: HttpClient) {
    super(httpClient);
    // Bu servise özel API endpoint'ini belirtiyoruz.
    this.apiUri = 'partnerCompany';
  }

  /**
   * Araçları sayfalama ve arama kriterlerine göre API'den çeker.
   * @param pageNumber İstenen sayfa numarası.
   * @param pageSize Sayfa başına kayıt sayısı.
   * @param searchTerm Arama metni.
   * @returns Gelen veriyi PaginationResultModel formatında bir Observable olarak döndürür.
   */
  public getPartnerCompaniesPaginated(
    pageNumber: number,
    pageSize: number,
    searchTerm: string
  ): Observable<PaginationResultModel> {

    // API isteği için parametreleri hazırlıyoruz.
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    // Eğer bir arama terimi varsa, onu da parametrelere ekliyoruz.
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    // Base servisteki getApiUri metodunu kullanarak tam URL'i oluşturuyoruz.
    const url = this.getApiUri(this.apiUri);

    // HttpClient ile GET isteği atıyoruz ve beklenen cevap tipini belirtiyoruz.
    // Bu sayede 'any' kullanmaktan kurtulup tip güvenliği sağlıyoruz.
    return this.httpClient.get<PaginationResultModel>(url, { params });
  }

  public getForDropdown(): Observable<DropdownModel[]> {
        // getApiUri helper'ı ile tam URL'i oluşturuyoruz: http://.../api/drivers/for-dropdown
        const url = this.getApiUri(`${this.apiUri}/for-dropdown`);
    
        // HttpClient ile GET isteği atıyoruz ve beklediğimiz cevap tipini belirtiyoruz.
        return this.httpClient.get<DropdownModel[]>(url);
      }

      public getById(id: number): Observable<PartnerCompanyModel> {
    // Base servisteki 'get(id)' metodunu çağırıyoruz.
    // Eğer base servisteki metodun adı farklıysa, burayı ona göre güncelleyin.
    return this.get(id);
  }
}