import { AfterViewInit, Component, ElementRef, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, fromEvent, merge, tap } from 'rxjs';
import { KhoaHocService } from '../services/khoa-hoc-service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { KhoaHocModel } from '../model/khoa-hoc.model';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { QueryParamsModel } from 'src/app/_metronic/core/models/query-models/query-params.model';
import { KhoaHocDataSource } from '../model/data-source/khoa-hoc-datasource';
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
import { KhoaHocEditDialogComponent } from '../khoa-hoc-edit/khoa-hoc-edit.dialog.component';
import { LayoutUtilsService } from 'src/app/_metronic/core/utils/layout-utils.service';
import { finalize } from "rxjs";

@Component({
    selector: 'app-khoa-hoc-list',
    standalone: true,
    providers: [KhoaHocService, LayoutUtilsService, TokenStorage, { provide: MatPaginatorIntl, useClass: MatPaginatorIntlCustom }],
    imports: [CommonModule, ReactiveFormsModule, MatPaginatorModule, MatSortModule, MatIconModule, TranslateModule, MatTooltipModule, MatTableModule, SharedModule],
    templateUrl: './khoa-hoc.component.html',
})
export class KhoaHocTableListComponent implements OnInit, OnDestroy {
    private KhoaHocService = inject(KhoaHocService);
    private translate = inject(TranslateService);
    public dialog = inject(MatDialog);
    private tokenStorage = inject(TokenStorage);
    private layoutUtilsService = inject(LayoutUtilsService);

    itemModel: any;
    dataSource: KhoaHocDataSource;
    dataResult: any[] = [];
    displayedColumns = ['STT', 'TenKhoaHoc', 'NamHoc', 'HienThi', 'CachViet', 'CreatedBy', 'CreatedDate', 'actions'];
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    @ViewChild('searchInput', { static: true }) searchInput: ElementRef;
    public pageSize: number = 10;
    queryNow: QueryParamsModel;


    constructor() {
        this.tokenStorage.getPageSize().subscribe(res => {
            this.pageSize = +res;
        });
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

        this.dataSource = new KhoaHocDataSource(this.KhoaHocService);
        this.dataSource.entitySubject.subscribe(res => this.dataResult = res);
        this.loadDataList();
    }

    loadDataList() {

        const queryParams = new QueryParamsModel(
            this.filterConfiguration(), //Trả về từ khóa cần lọc ở đây
            this.sort.direction,
            this.sort.active,
            this.paginator.pageIndex,
            this.paginator.pageSize
        );

        this.queryNow = queryParams;

        if (this.paginator.pageSize)
            this.pageSize = this.paginator.pageSize;


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
                this.KhoaHocService.delete(item.id).subscribe((res) => {
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
        const item = new KhoaHocModel();
        item.clear(); // Set all defaults fields
        item.id = id;
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
        const item = new KhoaHocModel();
        item.clear(); // Set all defaults fields
        this.Update(item);
    }

    Update(_item: KhoaHocModel) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.width = '600px';
        dialogConfig.height = 'auto';
        dialogConfig.data = {
            item: _item,
            isView: false // Add this flag for edit mode
        };

        const dialogRef = this.dialog.open(KhoaHocEditDialogComponent, dialogConfig);
        dialogRef.afterClosed().subscribe(result => {
            this.loadDataList();
        });
    }

    View(_item: KhoaHocModel) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.width = '600px';
        dialogConfig.height = 'auto';
        dialogConfig.data = {
            item: _item,
            isView: true // Add this flag for view mode
        };

        const dialogRef = this.dialog.open(KhoaHocEditDialogComponent, dialogConfig);
        dialogRef.afterClosed().subscribe(result => {
            this.loadDataList();
        });
    }
    getHeight(): any {
        let tmp_height = 0;
        tmp_height = window.innerHeight - 382;
        return tmp_height + 'px';
    }

    exportExcel() {
        if (this.dataSource) {
            this.KhoaHocService.exportExcel(this.queryNow)
                .pipe(
                    finalize(() => {

                    })
                )
                .subscribe({
                    next: (blob: Blob) => {
                        // Tạo file name với timestamp
                        const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, -5);
                        const fileName = `Danh_Sach_Khoa_Hoc_${timestamp}.xlsx`;

                        // Download file
                        const url = window.URL.createObjectURL(blob);
                        const link = document.createElement('a');
                        link.href = url;
                        link.download = fileName;
                        link.click();
                        window.URL.revokeObjectURL(url);

                        // Hiển thị thông báo thành công
                        this.layoutUtilsService.showSuccess(
                            this.translate.instant('EXCEL.xuatexcelthanhcong')
                        );
                    },
                    error: (error) => {
                        console.error('Export error:', error);
                        Swal.fire({
                            icon: 'error',
                            title: this.translate.instant('COMMON.error'),
                            text: error?.error?.error?.message || 'Có lỗi xảy ra khi xuất Excel',
                            confirmButtonText: this.translate.instant('COMMON.dong'),
                        });
                    },
                });
        }
        else {
            Swal.fire({
                title: "Thông báo!",
                icon: "question",
                text: "Không có dữ liệu để xuất excel",
                confirmButtonText: this.translate.instant('COMMON.dong'),
            });
        }
    }

}


