import { Component, OnInit, Inject, HostListener, ViewChild, ElementRef, ChangeDetectorRef, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { DotTuyenSinhService } from '../services/dot-tuyen-sinh.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DotTuyenSinhModel } from '../model/dot-tuyen-sinh.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { LayoutUtilsService } from 'src/app/_metronic/core/utils/layout-utils.service';
import Swal from 'sweetalert2';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { DateAdapter, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatSelectModule } from '@angular/material/select';
import { BacDaoTaoService } from '../../danhmuc/bac-dao-tao/services/bac-dao-tao-service';
import { BacDaoTaoModel } from '../../danhmuc/bac-dao-tao/model/bac-dao-tao.model';
@Component({
    selector: 'app-dot-tuyen-sinh-edit-dialog',
    standalone: true,
    providers: [DotTuyenSinhService, LayoutUtilsService,
        { provide: MAT_DATE_LOCALE, useValue: 'vi' },
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    ],
    imports: [CommonModule, FormsModule, MatFormFieldModule, MatTooltipModule, TranslateModule, ReactiveFormsModule, MatIconModule, MatDatepickerModule, NgxMatSelectSearchModule, MatSelectModule],
    templateUrl: './dot-tuyen-sinh-edit.dialog.component.html',
})
export class DotTuyenSinhEditDialogComponent implements OnInit {
    private translate = inject(TranslateService);
    private changeDetectorRefs = inject(ChangeDetectorRef);
    private fb = inject(FormBuilder);
    private DotTuyenSinhService = inject(DotTuyenSinhService);
    private layoutUtilsService = inject(LayoutUtilsService);

    item: DotTuyenSinhModel;
    itemForm: FormGroup;
    hasFormErrors: boolean = false;
    viewLoading: boolean = false;
    @ViewChild("focusInput") focusInput!: ElementRef;
    disabledBtn: boolean = false;
    listDOTTUYENSINHDaoTao: any[] = [];
    isView: boolean = false;

    constructor(public dialogRef: MatDialogRef<DotTuyenSinhEditDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
    ) {
        this.isView = data.isView;
    }
    /** LOAD DATA */
    ngOnInit() {
        this.item = this.data.item;
        this.reset();
        if (this.item.Id > 0) {
            this.viewLoading = true;
            this.DotTuyenSinhService.getDetail(this.item.Id).subscribe((res: any) => {
                this.item = res.data;
                this.createForm();
                this.changeDetectorRefs.detectChanges();
            });
        }
        else {
            this.viewLoading = false;
            this.createForm();
        }
    }

    reset() {
        this.item = Object.assign({}, this.item);
        this.createForm();
        this.hasFormErrors = false;
        this.itemForm.markAsPristine();
        this.itemForm.markAsUntouched();
        this.itemForm.updateValueAndValidity();
    }

    createForm() {
        this.itemForm = this.fb.group({
            // Id : [this.item.Code || '', [Validators.required]],
            NamHoc: [this.item.NamHoc || 0, [Validators.required]],
            Dot: [this.item.Dot || '', [Validators.required]],
            TenDotTS: [this.item.TenDotTS || '', [Validators.required]],
            KhoaHoc: [this.item.KhoaHoc || '', [Validators.required]],
            thoiGianNhanHSTuNgay: [this.item.ThoiGianLayHSTuNgay || new Date()],
            thoiGianNhanHSDenNgay: [this.item.ThoiGianNhanHSDenNgay || new Date()],
            NgayInGBTT: [this.item.NgayInGBTT || new Date()],
            thoiGianLayHSTuNgay: [this.item.ThoiGianLayHSTuNgay || new Date()],
            thoiGianLayHSDenNgay: [this.item.ThoiGianLayHSDenNgay || new Date()],
            NgayNhapHocDK: [this.item.NgayNhapHocDK || new Date()],
            GhiChu: [this.item.GhiChu || ''],
            // NguoiTao : [this.item.NguoiTao || ''],
            NgayTao: [this.item.NgayTao || new Date()],
            Isdel: [this.item.Isdel || false],
        });
        this.itemForm.markAllAsTouched();
        if (this.isView) {
            this.itemForm.disable();
        }
    }

    /** UI */
    getTitle(): string {
        if (this.isView) {
            return this.translate.instant('COMMON.xemchitiet');
        }

        if (!this.item || !this.item.Id) {
            return this.translate.instant('COMMON.themmoi');
        }

        return this.translate.instant('COMMON.capnhat');
    }
    prepareData(): DotTuyenSinhModel {
        const controls = this.itemForm.controls;
        const _item = new DotTuyenSinhModel();
        // _item.Id = this.item.Id;
        _item.NamHoc = controls['NamHoc'].value;
        _item.Dot = controls['Dot'].value;
        _item.TenDotTS = controls['TenDotTS'].value;
        _item.KhoaHoc = controls['KhoaHoc'].value;
        _item.ThoiGianNhanHSTuNgay = controls['ThoiGianNhanHSTuNgay'].value;
        _item.ThoiGianNhanHSDenNgay = controls['ThoiGianNhanHSDenNgay'].value;
        _item.NgayInGBTT = controls['NgayInGBTT'].value;
        _item.ThoiGianLayHSTuNgay = controls['ThoiGianLayHSTuNgay'].value;
        _item.ThoiGianLayHSDenNgay = controls['ThoiGianLayHSDenNgay'].value;
        _item.NgayNhapHocDK = controls['NgayNhapHocDK'].value;
        _item.GhiChu = controls['GhiChu'].value;
        _item.NguoiTao = controls['NguoiTao'].value;
        _item.NgayTao = controls['NgayTao'].value;

        return _item;
    }
    onSubmit(withBack: boolean = false) {
        this.hasFormErrors = false;
        const controls = this.itemForm.controls;
        /* check form */
        if (this.itemForm.invalid) {
            Object.keys(controls).forEach(controlName =>
                controls[controlName].markAsTouched()
            );
            this.hasFormErrors = true;
            return;
        }
        const updatedegree = this.prepareData();
        if (updatedegree.Id > 0) {
            this.Update(updatedegree);
        } else {
            this.Create(updatedegree, withBack);
        }
    }

    Update(_item: DotTuyenSinhModel) {
        this.disabledBtn = true;
        this.DotTuyenSinhService.update(_item).subscribe((res: any) => {
            this.disabledBtn = false;
            this.changeDetectorRefs.detectChanges();
            if (res && res.status === 1) {
                const _messageType = this.translate.instant('COMMON.capnhatthanhcong');
                this.layoutUtilsService.showSuccess(_messageType);
                this.dialogRef.close({
                    _item
                });
            }
            else {
                this.layoutUtilsService.showError(res.error.message);
            }
        });
    }

    Create(_item: DotTuyenSinhModel, withBack: boolean) {
        this.disabledBtn = true;
        this.DotTuyenSinhService.create(_item).subscribe((res: any) => {
            this.disabledBtn = false;
            this.changeDetectorRefs.detectChanges();
            if (res && res.status === 1) {
                if (withBack == true) {
                    const _messageType = this.translate.instant('COMMON.themmoithanhcong');
                    this.layoutUtilsService.showSuccess(_messageType);
                    this.dialogRef.close({
                        _item
                    });
                }
                else {
                    const _messageType = this.translate.instant('COMMON.themmoithanhcong');
                    this.layoutUtilsService.showSuccess(_messageType);
                    this.focusInput.nativeElement.focus();
                    this.ngOnInit();
                }
            }
            else {
                this.layoutUtilsService.showError(res.error.message);
            }
        });
    }

    @HostListener('document:keydown', ['$event'])
    onKeydownHandler(event: KeyboardEvent) {
        if (event.ctrlKey && event.keyCode == 13)//phím Enter
        {
            if (this.viewLoading == true) {
                this.onSubmit(true);
            }
            else {
                this.onSubmit(false);
            }
        }
    }

    get isFormDirty(): boolean {
        return this.itemForm.dirty; // Nếu bất kỳ form control nào bị thay đổi, dirty sẽ là true
    }

    close() {
        if (this.isFormDirty) {
            const successAlert = this.layoutUtilsService.showWarning();
            Swal.fire(successAlert).then((clicked) => {
                if (clicked.isConfirmed) {
                    this.dialogRef.close();
                }
            });
        } else {
            this.dialogRef.close();
        }
    }

    validateNumber(e: KeyboardEvent) {
        // Cho phép: số từ 0-9 (48-57), numpad (96-105), backspace (8), delete (46)
        // tab (9), left arrow (37), right arrow (39)
        const allowedKeys = [8, 9, 37, 39, 46];

        if (
            !(
                (e.keyCode >= 48 && e.keyCode <= 57) ||     // Numbers
                (e.keyCode >= 96 && e.keyCode <= 105) ||    // Numpad numbers
                allowedKeys.includes(e.keyCode)             // Other allowed keys
            )
        ) {
            e.preventDefault();
        }
    }
}
