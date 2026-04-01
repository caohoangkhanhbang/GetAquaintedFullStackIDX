import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, finalize, map, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpUtilsService } from 'src/app/_metronic/core/utils/http-utils.service';
import { QueryParamsModel } from 'src/app/_metronic/core/models/query-models/query-params.model';
import { QueryResultsModel } from 'src/app/_metronic/core/models/query-models/query-results.model';
import { KhoaHocModel } from '../model/khoa-hoc.model';


const API_ROOT_URL = environment.HOST_TUTORIAL_API + '/api/khoahoc';
const API_URL = environment.HOST_JEEHR_API + '/' + environment.apiUrl;
const API_SHARE = environment.HOST_TUTORIAL_API + '/api/share';


@Injectable({
    providedIn: 'root'
})

export class KhoaHocService {
    constructor(private http: HttpClient,
        private httpUtils: HttpUtilsService,
    ) { }

    //=======================================================

    findData(queryParams: QueryParamsModel): Observable<QueryResultsModel> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        const httpParams = this.httpUtils.getFindHTTPParams(queryParams);
        const url = API_ROOT_URL + '/list';
        return this.http.get<QueryResultsModel>(url, {
            headers: httpHeaders,
            params: httpParams
        });
    }

    getData(dataTablesParameters: any): Observable<any> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        const httpParams = this.httpUtils.getFindHTTPParamsNew(dataTablesParameters);
        return this.http.get<any>(API_ROOT_URL + '/list', {
            headers: httpHeaders,
            params: httpParams
        }).pipe(map((res: any) => {
            if (res && res.status == 1 && res.data.length > 0) {
                return {
                    data: res.data,
                    recordsFiltered: res.page.Total,
                    recordsTotal: res.page.Total,
                };
            } else {
                return {
                    data: [],
                    recordsFiltered: 0,
                    recordsTotal: 0,
                };
            }
        }));
    }

    getDetail(id: number): Observable<any> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        const url = API_ROOT_URL + `/detail/${id}`;
        return this.http.get<any>(url, { headers: httpHeaders });
    }

    create(item: KhoaHocModel): Observable<any> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        return this.http.post<any>(API_ROOT_URL + '/insert', item, { headers: httpHeaders });
    }

    update(item: KhoaHocModel): Observable<any> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        return this.http.post<any>(API_ROOT_URL + '/update', item, { headers: httpHeaders });
    }

    delete(id: number): Observable<any> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        const url = API_ROOT_URL + `/delete/${id}`;
        console.log('API_ROOT_URL', API_ROOT_URL);
        return this.http.delete<any>(url, { headers: httpHeaders });
    }

    getListNamHoc(): Observable<any> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        const url = API_SHARE + `/list-nam-hoc`;
        return this.http.get<any>(url, { headers: httpHeaders });
    }

}
