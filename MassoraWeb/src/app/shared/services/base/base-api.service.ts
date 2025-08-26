import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { throwError } from 'rxjs';

@Injectable()
export abstract class BaseApiService {
  constructor(protected httpClient: HttpClient) { }

  protected httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  protected getAngularUri(uri: string): string {
    return `${environment.angularUrl}/${uri}`;
  }

  protected getApiUri(uri: string): string {
    return `${environment.apiUrl}/${uri}`;
  }
  protected getAuthUri(uri: string): string {
    return `${environment.authUrl}/${uri}`;
  }

  protected handleError(error: HttpErrorResponse) {
    console.error('Error occurred:', error);
    return throwError(() => new Error('An error occurred, please try again later.'));
  }
}
