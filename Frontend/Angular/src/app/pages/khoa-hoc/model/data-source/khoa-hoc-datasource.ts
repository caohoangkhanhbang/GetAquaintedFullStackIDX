import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { BaseDataSource } from 'src/app/_metronic/core/models/data-sources/_base.datasource';
import { KhoaHocService } from '../../services/khoa-hoc-service';
import { inject } from '@angular/core';
import { QueryParamsModel } from 'src/app/_metronic/core/models/query-models/query-params.model';
import { QueryResultsModel } from 'src/app/_metronic/core/models/query-models/query-results.model';

export class KhoaHocDataSource extends BaseDataSource {
    constructor(private apiService: KhoaHocService) {
        super();
    }

    loadList(queryParams: QueryParamsModel) {
        this.loadingSubject.next(true);
        this.apiService.findData(queryParams)
            .pipe(
                tap(resultFromServer => {
                    if (resultFromServer && resultFromServer.status == 1 && (resultFromServer.data != null)) {
                        this.entitySubject.next(resultFromServer.data);
                        var totalCount = resultFromServer.page.Total || (resultFromServer.page.AllPage * resultFromServer.page.Size);
                        this.paginatorTotalSubject.next(totalCount);
                    } else {
                        this.entitySubject.next(resultFromServer.data);
                        this.paginatorTotalSubject.next(0);
                    }
                }),
                catchError(err => of(new QueryResultsModel([], err))),
                finalize(() => this.loadingSubject.next(false))
            ).subscribe(res => {
            });
    }
}
