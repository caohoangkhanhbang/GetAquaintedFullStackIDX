import { Injectable } from '@angular/core';
import { map, catchError, tap, switchMap } from 'rxjs/operators';
import { HttpParams, HttpHeaders } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from 'src/app/modules/auth';
const KEY_SSO_TOKEN = 'sso_token';
const KEY_SSO_TOKEN_2 = 'sso_token_2';

@Injectable({
  providedIn: 'root',
})
export class HttpUtilsService {
  constructor(private auth: AuthService, private cookieService: CookieService) { }
  getFindHTTPParams(queryParams: any): HttpParams {
    let params = new HttpParams()
      .set('sortOrder', queryParams.sortOrder)
      .set('sortField', queryParams.sortField)
      .set('page', (queryParams.pageNumber + 1).toString())
      .set('record', queryParams.pageSize.toString());
    let keys: any[] = [],
      values: any[] = [];
    if (queryParams.more) {
      params = params.append('more', 'true');
    }
    Object.keys(queryParams.filter).forEach(function (key) {
      if (typeof queryParams.filter[key] !== 'string' || queryParams.filter[key] !== '') {
        keys.push(key);
        values.push(queryParams.filter[key]);
      }
    });
    if (keys.length > 0) {
      params = params.append('filter.keys', keys.join('|')).append('filter.vals', values.join('|'));
    }
    return params;
  }

  getFindHTTPParamsNew(dataTablesParameters: any): HttpParams {
    let params = new HttpParams()
      .set('sortOrder', dataTablesParameters.order.length > 0 ? dataTablesParameters.order[0].dir : 'id')
      .set('sortField', dataTablesParameters.order.length > 0 ? dataTablesParameters.columns[dataTablesParameters.order[0].column].data : 'asc')
      .set('page', ((dataTablesParameters.start / 10) + 1).toString())
      .set('record', dataTablesParameters.length.toString());
    if (dataTablesParameters.more) {
      params = params.append('more', 'true');
    }
    //Xử lý tạm thời cho trường hợp search 1 field
    if (dataTablesParameters.search.value != "") {
      let keys: any[] = [],
        values: any[] = [];
      keys.push("keyword");
      values.push(dataTablesParameters.search.value);
      if (keys.length > 0) {
        params = params.append('filter.keys', keys.join('|')).append('filter.vals', values.join('|'));
      }
    }
    return params;
  }

  getHTTPHeaders(version: any = "1.0"): HttpHeaders {
    const auth = this.auth.getAuthFromLocalStorage();
    let result = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth.access_token}`,
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Headers': 'Content-Type',
      'x-api-version': `${version}`,
      'TimeZone': (new Date()).getTimezoneOffset().toString()
    });
    return result;
  }
}
