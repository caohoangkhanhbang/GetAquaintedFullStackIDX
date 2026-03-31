import { Component, OnInit, Inject, HostListener, ViewChild, ElementRef, ChangeDetectorRef, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { from } from 'rxjs';
import { values } from 'lodash';
//thêm các thư viện xử lý ngày giờ
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import moment from 'moment'; // Đảm bảo đã import moment ở đầu file

//thêm
export const MY_FORMATS = {
    parse: {
        dateInput: 'DD/MM/YYYY',
    },
    display: {
        dateInput: 'DD/MM/YYYY',
        monthYearLabel: 'MMM YYYY',
        dateA11yLabel: 'LL',
        monthYearA11yLabel: 'MMMM YYYY',
    },
};


@Component({
    selector: 'app-dot-tuyen-sinh-edit-dialog',
    standalone: true,
    providers: [DotTuyenSinhService, LayoutUtilsService,
        { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
        { provide: MAT_DATE_LOCALE, useValue: 'vi' },
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    ],
    imports: [CommonModule, FormsModule, MatFormFieldModule, MatTooltipModule, TranslateModule, ReactiveFormsModule, MatIconModule, MatDatepickerModule, NgxMatSelectSearchModule, MatSelectModule,

        MatInputModule,
        MatDatepickerModule,
        MatNativeDateModule,
    ],
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
    listNamHoc: any[] = [];
    listKhoaHoc: any[] = [];

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
        this.getListNamHoc();
        this.getListKhoaHoc();
        const fromDateNhanHS = this.formatDateCustom(this.item.ThoiGianNhanHSTuNgay);
        const toDateNhanHS = this.formatDateCustom(this.item.ThoiGianNhanHSDenNgay);
        const fromDateLayHS = this.formatDateCustom(this.item.ThoiGianLayHSTuNgay);
        const toDateLayHS = this.formatDateCustom(this.item.ThoiGianLayHSDenNgay);
        const ngayNhapHocDK = this.formatDateCustom(this.item.NgayNhapHocDK);
        const ngayinGBTT = this.formatDateCustom(this.item.NgayInGBTT);



        this.itemForm = this.fb.group({
            Id: [this.item.Id],
            NamHoc: [this.item.NamHoc || '', [Validators.required]],
            Dot: [this.item.Dot || '', [Validators.required]],
            TenDotTS: [this.item.TenDotTS || '', [Validators.required]],
            KhoaHoc: [this.item.KhoaHoc || '', [Validators.required]],
            ThoiGianNhanHSTuNgay: fromDateNhanHS,
            ThoiGianNhanHSDenNgay: toDateNhanHS,
            NgayInGBTT: ngayinGBTT,
            ThoiGianLayHSTuNgay: fromDateLayHS,
            ThoiGianLayHSDenNgay: toDateLayHS,
            NgayNhapHocDK: ngayNhapHocDK,
            GhiChu: [this.item.GhiChu || ''],
            HienThi: [this.item.HienThi],
            KichHoat: [this.item.KichHoat],
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

        // Hàm hỗ trợ format an toàn để lưu xuống database thành yyyy-mm-dd
        const formatSafe = (val: any) => {
            if (!val) return null;
            const m = moment(val);
            return m.isValid() ? m.format("YYYY-MM-DD") : null;
        };

        const ThoiGianLayHSTuNgay = controls['ThoiGianLayHSTuNgay']?.value;
        const ThoiGianLayHSDenNgay = controls['ThoiGianLayHSDenNgay']?.value;
        const ThoiGianNhanHSTuNgay = controls['ThoiGianNhanHSTuNgay']?.value;
        const ThoiGianNhanHSDenNgay = controls['ThoiGianNhanHSDenNgay']?.value;
        const NgayInGBTT = controls['NgayInGBTT']?.value;
        const NgayNhapHocDK = controls['NgayNhapHocDK']?.value;


        _item.Id = this.item.Id;
        _item.NamHoc = controls['NamHoc']?.value ?? 0;
        _item.Dot = controls['Dot']?.value ?? '';
        _item.TenDotTS = controls['TenDotTS']?.value ?? '';
        _item.KhoaHoc = controls['KhoaHoc']?.value ?? 0;
        _item.ThoiGianNhanHSTuNgay = formatSafe(ThoiGianNhanHSTuNgay);
        _item.ThoiGianNhanHSDenNgay = formatSafe(ThoiGianNhanHSDenNgay);
        _item.NgayInGBTT = formatSafe(NgayInGBTT);
        _item.ThoiGianLayHSTuNgay = formatSafe(ThoiGianLayHSTuNgay);
        _item.ThoiGianLayHSDenNgay = formatSafe(ThoiGianLayHSDenNgay);
        _item.NgayNhapHocDK = formatSafe(NgayNhapHocDK);
        _item.GhiChu = controls['GhiChu']?.value ?? '';
        _item.HienThi = controls['HienThi']?.value ?? false;
        _item.KichHoat = controls['KichHoat']?.value ?? false;
        _item.TenNamHoc = '';
        _item.TenKhoaHoc = '';
        return _item;
    }

    styleConversionNull(item: any) {
        if (item === '' || item === undefined || item === null)
            return null;
        return item;
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

        const ThoiGianLayHSTuNgay = this.itemForm.get('ThoiGianLayHSTuNgay')?.value;
        const ThoiGianLayHSDenNgay = this.itemForm.get('ThoiGianLayHSDenNgay')?.value;
        const ThoiGianNhanHSTuNgay = this.itemForm.get('ThoiGianNhanHSTuNgay')?.value;
        const ThoiGianNhanHSDenNgay = this.itemForm.get('ThoiGianNhanHSDenNgay')?.value;

        if (ThoiGianNhanHSTuNgay && ThoiGianNhanHSDenNgay) {
            if (this.isValidDate(ThoiGianNhanHSTuNgay, ThoiGianNhanHSDenNgay)) {
                this.NotificationCustom("Thời gian nhận hồ sơ đến ngày phải lớn hơn thời gian nhận hồ sơ từ ngày");
                return;
            }
        }

        if (ThoiGianLayHSTuNgay && ThoiGianLayHSDenNgay) {
            if (this.isValidDate(ThoiGianLayHSTuNgay, ThoiGianLayHSDenNgay)) {
                this.NotificationCustom("Thời gian lấy hồ sơ đến ngày phải lớn hơn thời gian lấy hồ sơ từ ngày");
                return;
            }
        }

        if (Number(updatedegree.Dot) > 255) {
            this.NotificationCustom("Đợt không vượt quá 255");
            return;
        }



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
        const keyCode = e.keyCode;
        const allowedKeys = [8, 9, 37, 39, 46]; // Backspace, Tab, Left, Right, Delete
        const inputValue = e.target as HTMLInputElement;
        const maxSoThuTu = 2147483647;

        // 1. Nếu là phím điều khiển (Xóa, Di chuyển) -> CHO QUA LUÔN (return sớm)
        if (allowedKeys.includes(keyCode)) {
            return;
        }

        // 2. Kiểm tra xem có phải là phím số không
        const isNumber = (keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105);

        // 3. Nếu KHÔNG PHẢI số HOẶC (LÀ số nhưng đã đủ 10 ký tự) -> CHẶN
        if (!isNumber || (isNumber && Number(inputValue.value) > maxSoThuTu)) {
            //Hiển thị thông báo nếu nhập quá 10 chữ số
            this.NotificationCustom("Đợt không được quá 2,147,483,647");
            // Chỉ chặn nếu không phải là đang bôi đen để ghi đè
            if (inputValue.selectionStart === inputValue.selectionEnd) {
                e.preventDefault();
            }
        }
    }

    valiDateRange(strartDateString: string, endDateString: string): boolean {
        if (!strartDateString || !endDateString)
            return true;
        const startDate = new Date(strartDateString);
        const endDate = new Date(endDateString);
        return endDate.getTime() >= startDate.getTime();
    }

    getListNamHoc() {
        this.DotTuyenSinhService.getListNamHoc().subscribe(data => {
            if (data || data.status === 1) {
                this.listNamHoc = data.data;
                //là một phương thức được sử dụng để ép buộc (force) Angular thực hiện quy trình kiểm tra thay đổi
                this.changeDetectorRefs.detectChanges();
            }
            else {
                this.listNamHoc = [];
            }
        })
    }

    getListKhoaHoc() {
        this.DotTuyenSinhService.getListKhoaHoc().subscribe(data => {
            if (data || data.status === 1) {
                this.listKhoaHoc = data.data;
            }
            else {
                this.listKhoaHoc = [];
            }
        })
    }

    NotificationCustom(text: string) {
        Swal.fire({
            title: "Thông báo!",
            text: text,
            icon: "info",
            confirmButtonText: "Đồng ý",
            confirmButtonColor: "rgb(255, 0, 68)"
        })
    }

    isValidDate(fromDate: any, toDate: any): boolean {
        const date1 = fromDate ? new Date(fromDate).getTime() : 0;
        const date2 = toDate ? new Date(toDate).getTime() : 0;
        if (toDate < fromDate)
            return true;
        return false;
    }

    formatDateCustom(date: any): string {
        if (!date) return '';
        const d = new Date(date);
        const month = '' + (d.getMonth() + 1);
        const day = '' + d.getDate();
        const year = d.getFullYear();
        return [year, month.padStart(2, '0'), day.padStart(2, '0')].join('-');
    }

    checkDate(categori: any) {
        switch (categori) {
            case "ThoiGianNhanHSDenNgay": {
                this.checkDateRange(this.itemForm.get("ThoiGianNhanHSTuNgay"), this.itemForm.get("ThoiGianNhanHSDenNgay"), "Vui lòng nhập Thời gian nhận hồ sơ từ ngày", "Thời gian nhận hồ sơ đến ngày phải lớn hơn thời gian nhận hồ sơ từ ngày");
                this.itemForm.get("ThoiGianNhanHSTuNgay")?.setValue("27/03/2025");
                break;
            }
            case "ThoiGianLayHSDenNgay": {
                this.checkDateRange(this.itemForm.get("ThoiGianLayHSTuNgay"), this.itemForm.get("ThoiGianLayHSDenNgay"), "Vui lòng nhập Thời gian lấy hồ sơ từ ngày", "Thời gian lấy hồ sơ đến ngày phải lớn hơn thời gian lấy hồ sơ từ ngày");
                break;
            }
        }
    }

    checkDateRange(fromDate: AbstractControl | null, toDate: AbstractControl | null, fromDateNoti: string, toDateNoti: string) {
        const tuNgayVal = fromDate?.value;
        const denNgayVal = toDate?.value;
        const tuNgay = new Date(tuNgayVal)?.getTime();
        const denNgay = new Date(denNgayVal)?.getTime();
        if (!tuNgay && denNgay) {
            this.NotificationCustom(fromDateNoti)
            toDate?.setValue(null);
        }
        if (denNgay < tuNgay) {
            this.NotificationCustom(toDateNoti);
            toDate?.setValue(null);
        }
    }

}
