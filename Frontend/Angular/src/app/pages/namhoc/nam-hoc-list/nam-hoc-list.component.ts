import { NamHocService } from "../services/nam-hoc-service";
import { NamHocDataSource } from "../model/data-sources/nam-hoc.datasource";
import { NamHocModel } from "../model/nam-hoc.model";
import { NamHocEditDialogComponent } from "../nam-hoc-edit/nam-hoc-edit.dialog.component";
import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, fromEvent, merge, tap } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { QueryParamsModel } from 'src/app/_metronic/core/models/query-models/query-params.model';
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTableModule } from '@angular/material/table';
import { SharedModule } from 'src/app/_metronic/shared/shared.module';
import { TokenStorage } from 'src/app/modules/auth/services/token-storage.service';
import { MatPaginatorIntlCustom } from 'src/app/modules/auth/services/config-mat-page';
import { LayoutUtilsService } from 'src/app/_metronic/core/utils/layout-utils.service';
import { OnInit } from "@angular/core";
import Swal, { SweetAlertOptions } from 'sweetalert2';
import { finalize } from "rxjs";



@Component({
    selector: 'app-nam-hoc-list',
    standalone: true,
    providers: [NamHocService, LayoutUtilsService, TokenStorage, { provide: MatPaginatorIntl, useClass: MatPaginatorIntlCustom }],
    imports: [CommonModule, ReactiveFormsModule, MatPaginatorModule, MatSortModule, MatIconModule, TranslateModule, MatTooltipModule, MatTableModule, SharedModule],
    templateUrl: './nam-hoc-list.component.html'
})

export class NamHocTableListComponent implements OnInit {

    private translate = inject(TranslateService);
    public dialog = inject(MatDialog);
    private tokenStorage = inject(TokenStorage);
    private NamHocService = inject(NamHocService);
    private layoutUtilsService = inject(LayoutUtilsService);

    dataSource: NamHocDataSource;
    dataResult: any[] = [];
    itemModel: any;
    displayedColumns = ['STT', 'NamHoc', 'NienHoc', 'HienThi', 'NguoiTao', 'NgayTao', 'actions'];
    filter: any = {};
    queryNow: QueryParamsModel;

    @ViewChild('searchInput', { static: true }) searchInput: ElementRef;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;

    public pageSize: number = 50;
    constructor() {
        this.tokenStorage.getPageSize().subscribe(res => {
            this.pageSize = +res;
        })
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
        this.dataSource = new NamHocDataSource(this.NamHocService);
        this.dataSource.entitySubject.subscribe(res => this.dataResult = res)
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

    edit(id: number) {
        const item = new NamHocModel();
        item.clear(); // Set all defaults fields
        item.id = id;
        this.Update(item);
    }

    getTitle() {
        let result = this.itemModel.RowID > 0 ? this.translate.instant("COMMON.capnhat")
            : this.translate.instant("COMMON.themmoi");
        return result;
    }

    Add() {
        const item = new NamHocModel();
        item.clear(); // Set all defaults fields
        this.Update(item);
    }

    Update(_item: NamHocModel) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.width = '600px';
        dialogConfig.height = 'auto';
        dialogConfig.data = {
            item: _item,
            isView: false // Add this flag for edit mode
        };

        const dialogRef = this.dialog.open(NamHocEditDialogComponent, dialogConfig);
        dialogRef.afterClosed().subscribe(result => {
            this.loadDataList();
        });
    }

    View(_item: NamHocModel) {
        const dialogConfig = new MatDialogConfig();
        dialogConfig.width = '600px';
        dialogConfig.height = 'auto';
        dialogConfig.data = {
            item: _item,
            isView: true // Add this flag for view mode
        };

        const dialogRef = this.dialog.open(NamHocEditDialogComponent, dialogConfig);
        dialogRef.afterClosed().subscribe(result => {
            this.loadDataList();
        });
    }

    getHeight(): any {
        let tmp_height = 0;
        tmp_height = window.innerHeight - 382;
        return tmp_height + 'px';
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
                this.NamHocService.delete(item.id).subscribe((res) => {
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

    exportExcel() {
        if (this.dataSource) {
            this.NamHocService.exportExcel(this.queryNow)
                .pipe(
                    finalize(() => {

                    })
                )
                .subscribe({
                    next: (blob: Blob) => {
                        // Tạo file name với timestamp
                        const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, -5);
                        const fileName = `Danh_Sach_Nam_Hoc_${timestamp}.xlsx`;

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

