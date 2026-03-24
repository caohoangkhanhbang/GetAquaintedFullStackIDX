import { AfterViewInit, Component, ElementRef, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, fromEvent, merge, tap } from 'rxjs';
import { DotTuyenSinhService } from '../services/dot-tuyen-sinh.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { DotTuyenSinhModel } from '../model/dot-tuyen-sinh.model';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { QueryParamsModel } from 'src/app/_metronic/core/models/query-models/query-params.model';
import { DotTuyenSinhDataSource } from '../model/data-sources/dot-tuyen-sinh.datasource';
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTableModule } from '@angular/material/table';
import { SharedModule } from 'src/app/_metronic/shared/shared.module';
import Swal, { SweetAlertOptions } from 'sweetalert2';
import { TokenStorage } from 'src/app/modules/auth/services/token-storage.service';
import { MatPaginatorIntlCustom } from 'src/app/modules/auth/services/config-mat-page';
import { DotTuyenSinhEditDialogComponent } from '../dot-tuyen-sinh-edit/dot-tuyen-sinh-edit.dialog.component';
import { LayoutUtilsService } from 'src/app/_metronic/core/utils/layout-utils.service';
import { BacDaoTaoModel } from '../../danhmuc/bac-dao-tao/model/bac-dao-tao.model';

@Component({
    selector: 'app-dot-tuyen-sinh-list',
    standalone: true,
    providers: [DotTuyenSinhService, LayoutUtilsService, TokenStorage, { provide: MatPaginatorIntl, useClass: MatPaginatorIntlCustom }],
    imports: [CommonModule, ReactiveFormsModule, MatPaginatorModule, MatSortModule, MatIconModule, TranslateModule, MatTooltipModule, MatTableModule, SharedModule],
    templateUrl: './dot-tuyen-sinh-list.component.html',
})
export class DotTuyenSinhTableListComponent implements OnInit, AfterViewInit, OnDestroy {
    private DotTuyenSinhService = inject(DotTuyenSinhService);
    private translate = inject(TranslateService);
    public dialog = inject(MatDialog);
    private tokenStorage = inject(TokenStorage);
    private layoutUtilsService = inject(LayoutUtilsService);

    itemModel: any;
    dataSource: DotTuyenSinhDataSource;
    dataResult: any[] = [];
    displayedColumns = ['STT', 'NamHoc', 'Dot', 'TenDotTS', 'KhoaHoc', 'ThoiGianNhanHS', 'NgayInGBTT', 'ThoiGianLayHS', 'NgayNhapHocDK', 'GhiChu', 'CreatedBy', 'CreatedDate', 'actions']
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    @ViewChild('searchInput', { static: true }) searchInput: ElementRef;
    public pageSize: number = 10;
    constructor() {
        this.tokenStorage.getPageSize().subscribe(res => {
            this.pageSize = +res;
        });
    }

    ngAfterViewInit(): void {
    }

    ngOnInit(): void {

        merge(this.sort.sortChange, this.paginator.page)
            .pipe(
                tap(() => {
                    this.loadDataList();
                })
            )
            .subscribe();

        fromEvent(this.searchInput.nativeElement, 'keyup')
            .pipe(
                debounceTime(150),
                distinctUntilChanged(),
                tap(() => {
                    this.paginator.pageIndex = 0;
                    this.loadDataList();
                })
            )
            .subscribe();

        this.dataSource = new DotTuyenSinhDataSource(this.DotTuyenSinhService);
        this.dataSource.entitySubject.subscribe(res => this.dataResult = res);
        this.loadDataList();
    }

    loadDataList() {

        const queryParams = new QueryParamsModel(
            this.filterConfiguration(),
            this.sort.direction,
            this.sort.active,
            this.paginator.pageIndex,
            this.paginator.pageSize
        );
        this.dataSource.loadList(queryParams);

        setTimeout((x: any) => {
            this.loadPage();
        }, 500)
    }

    loadPage() {
        var arrayData: any[] = [];
        this.dataSource.entitySubject.subscribe(res => arrayData = res);
        if (arrayData !== undefined && arrayData?.length == 0) {
            var totalRecord = 0;
            this.dataSource.paginatorTotal$.subscribe(tt => totalRecord = tt)
            if (totalRecord > 0) {
                const queryParams1 = new QueryParamsModel(
                    this.filterConfiguration(),
                    this.sort.direction,
                    this.sort.active,
                    this.paginator.pageIndex = this.paginator.pageIndex - 1,
                    this.paginator.pageSize
                );
                this.dataSource.loadList(queryParams1);
            }
            else {
                const queryParams1 = new QueryParamsModel(
                    this.filterConfiguration(),
                    this.sort.direction,
                    this.sort.active,
                    this.paginator.pageIndex = 0,
                    this.paginator.pageSize
                );
                this.dataSource.loadList(queryParams1);
            }
        }
    }

    filterConfiguration(): any {

        const filter: any = {};
        const searchText: string = this.searchInput.nativeElement.value;
        filter.keyword = searchText;
        return filter;
    }

    Delete(item: any) {
        const successAlert: SweetAlertOptions = {
            icon: 'warning',
            title: 'Bạn có chắc muốn xóa dữ liệu này không?',
            text: "Dữ liệu không thể hoàn tác sau khi bị xóa",
            showCancelButton: true,
            focusCancel: true,
            cancelButtonText: "Không",
            confirmButtonText: 'Có',
            customClass: {
                confirmButton: 'btn swl-confirm-btn',
                cancelButton: 'btn btn-active-light'
            }
        };
        Swal.fire(successAlert).then((clicked) => {
            if (clicked.isConfirmed) {
                this.DotTuyenSinhService.delete(item.Id).subscribe((res) => {
                    if (res && res.status == 1) {
                        this.loadDataList();
                        this.layoutUtilsService.showSuccess(res.error.message);
                    } else {
                        this.layoutUtilsService.showError(res.error.message);
                    }
                });
            }
        });
    }

    edit(id: number) {
        const item = new DotTuyenSinhModel();
        item.clear(); // Set all defaults fields
        item.Id = id;
        this.Update(item);
    }

    ngOnDestroy(): void {
    }

    getTitle() {
        let result = this.itemModel.RowID > 0 ? this.translate.instant("COMMON.capnhat")
            : this.translate.instant("COMMON.themmoi");
        return result;
    }

    Add() {
        const item = new DotTuyenSinhModel();
        item.clear(); // Set all defaults fields
        this.Update(item);
    }

    Update(_item: DotTuyenSinhModel) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.width = '600px';
        dialogConfig.height = 'auto';
        dialogConfig.data = {
            item: _item,
            isView: false // Add this flag for edit mode
        };
        const dialogRef = this.dialog.open(DotTuyenSinhEditDialogComponent, dialogConfig);
        dialogRef.afterClosed().subscribe(result => {
            this.loadDataList();
        });
    }
    View(_item: DotTuyenSinhModel) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.width = '600px';
        dialogConfig.height = 'auto';
        dialogConfig.data = {
            item: _item,
            isView: true // Add this flag for view mode
        };
        const dialogRef = this.dialog.open(DotTuyenSinhEditDialogComponent, dialogConfig);
        dialogRef.afterClosed().subscribe(result => {
            this.loadDataList();
        });
    }
    getHeight(): any {
        let tmp_height = 0;
        tmp_height = window.innerHeight - 382;
        return tmp_height + 'px';
    }
}
