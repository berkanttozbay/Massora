import { Injectable } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseApiService } from './base-api.service';    
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class BaseApiRequestService<T> extends BaseApiService {
  public odataUri: string = '';
  public apiUri: string = '';
  public pathcHeaders = new HttpHeaders({ 'Content-Type': 'application/json-patch+json' });

get(id: any): Observable<T> {
    return this.httpClient.get<T>(this.getApiUri(`${this.apiUri}/${id}`));
  }

getList(): Observable<T> {
    return this.httpClient.get<T>(this.getApiUri(this.apiUri));
  }

add(entity: T): Observable<T> {
return this.httpClient.post<T>(this.getApiUri(this.apiUri), entity);
}

async addAsync(entity: T): Promise<T> {
return firstValueFrom(this.httpClient.post<T>(this.getApiUri(this.apiUri), entity));
}

update( id: any,entity: T): Observable<T> {
return this.httpClient.put<T>(this.getApiUri(`${this.apiUri}/${id}`), entity, { headers: this.pathcHeaders });
}

updateWithPut(entity: T, id: any): Observable<T> {
return this.httpClient.put<T>(this.getApiUri(`${this.apiUri}/${id}`), entity);
}

delete(entity: any, id: any): Observable<T> {
return this.httpClient.patch<T>(this.getApiUri(`${this.apiUri}(${id})`), entity);
}


deletePermanently(id: any): Observable<T> {
return this.httpClient.delete<T>(this.getApiUri(`${this.apiUri}/${id}`));
}


}
