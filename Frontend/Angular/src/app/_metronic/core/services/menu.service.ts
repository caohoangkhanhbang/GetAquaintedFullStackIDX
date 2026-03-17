import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { QueryParamsModel } from '../models/query-models/query-params.model';
import { HttpUtilsService } from '../utils/http-utils.service';
import { QueryResultsModel } from '../models/query-models/query-results.model';

@Injectable()
export class MenuServices {
  data_share$ = new BehaviorSubject<any>([]);
  lastFilter$: BehaviorSubject<QueryParamsModel> = new BehaviorSubject(new QueryParamsModel({}, 'asc', '', 0, 10));
  ReadOnlyControl: boolean;
  constructor(private http: HttpClient, private httpUtils: HttpUtilsService) { }

  layMenuChucNang(): Observable<QueryResultsModel> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    return this.http.post<any>(environment.HOST_JEESALES_API + `/api/menus/GetMenu`, "", { headers: httpHeaders });
  }

  Get_DSNhacNho(): Observable<QueryResultsModel> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    return this.http.get<any>(environment.HOST_JEELANDINGPAGE_API + `/api/widgets/Get_DSNhacNho`, { headers: httpHeaders });
  }
  
  Get_LogoApp(AppID: number): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    return this.http.get<any>(environment.HOST_JEEACCOUNT_API + `/api/logo/Get_LogoApp?AppID=${AppID}`, { headers: httpHeaders });
  }

  Count_SoLuongNhacNho(): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    return this.http.get<any>(environment.HOST_JEELANDINGPAGE_API + `/api/widgets/Count_SoLuongNhacNho`, { headers: httpHeaders });
  }
}
