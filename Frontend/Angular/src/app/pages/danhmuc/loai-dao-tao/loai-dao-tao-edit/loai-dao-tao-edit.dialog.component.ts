import { Component, OnInit, Inject, HostListener, ViewChild, ElementRef, ChangeDetectorRef, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { LoaiDaoTaoService } from '../services/loai-dao-tao-service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LoaiDaoTaoModel } from '../model/loai-dao-tao.model';
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
import { AnimationDriver } from '@angular/animations/browser';
@Component({
    selector: 'app-loai-dao-tao-edit-dialog',
    standalone: true,
    providers: [LoaiDaoTaoService, LayoutUtilsService,
        { provide: MAT_DATE_LOCALE, useValue: 'vi' },
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    ],
    imports: [CommonModule, FormsModule, MatFormFieldModule, MatTooltipModule, TranslateModule, ReactiveFormsModule, MatIconModule, MatDatepickerModule, NgxMatSelectSearchModule, MatSelectModule],
    templateUrl: './loai-dao-tao-edit.dialog.component.html',
})
export class LoaiDaoTaoEditDialogComponent implements OnInit {
    private translate = inject(TranslateService);
    private changeDetectorRefs = inject(ChangeDetectorRef);
    private fb = inject(FormBuilder);
    private LoaiDaoTaoService = inject(LoaiDaoTaoService);
    private layoutUtilsService = inject(LayoutUtilsService);

    item: LoaiDaoTaoModel;
    itemForm: FormGroup;
    hasFormErrors: boolean = false;
    viewLoading: boolean = false;
    @ViewChild("focusInput") focusInput!: ElementRef;
    disabledBtn: boolean = false;
    listHinhThucDaoTao: any[] = [];
    isView: boolean = false;

    constructor(public dialogRef: MatDialogRef<LoaiDaoTaoEditDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
    ) {
        this.isView = data.isView;
    }
    /** LOAD DATA */
    ngOnInit() {
        this.item = this.data.item;
        this.reset();
        if (this.item.id > 0) {
            this.viewLoading = true;
            this.LoaiDaoTaoService.getDetail(this.item.id).subscribe((res: any) => {
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
            MaLoaiDT: [this.item.MaLoaiDT || '', [Validators.required]],
            TenLoaiDT: [this.item.TenLoaiDT || '', [Validators.required]],
            TenTiengAnh: [this.item.TenTiengAnh || ''],
            GhiChu: [this.item.GhiChu || '', [Validators.required]],
            SoThuTu: [this.item.SoThuTu],
            NoiDung: [this.item.NoiDung || '']
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

        if (!this.item || !this.item.id) {
            return this.translate.instant('COMMON.themmoi');
        }

        return this.translate.instant('COMMON.capnhat');
    }

    prepareData(): LoaiDaoTaoModel {
        const controls = this.itemForm.controls;
        const _item = new LoaiDaoTaoModel();
        _item.id = this.item.id;
        _item.MaLoaiDT = controls['MaLoaiDT'].value;
        _item.TenLoaiDT = controls['TenLoaiDT'].value;
        _item.TenTiengAnh = controls['TenTiengAnh'].value ?? null;
        _item.NoiDung = controls['NoiDung'].value;
        _item.GhiChu = controls['GhiChu'].value ?? null;
        if (controls['SoThuTu'].value === undefined || controls['SoThuTu'].value === null || controls['SoThuTu'].value === '') {
            _item.SoThuTu = 0;
        }
        else {
            _item.SoThuTu = controls['SoThuTu'].value;
        }
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
        const stt = this.itemForm.get("SoThuTu")?.value;
        if (stt !== null && stt !== undefined && stt !== '') {
            if (Number(stt) <= 0) {
                this.NotificationCustom("Số thứ tự phải lớn hơn không!");
                return;
            }
        }
        if (updatedegree.id > 0) {
            this.Update(updatedegree);
        } else {
            this.Create(updatedegree, withBack);
        }
    }

    Update(_item: LoaiDaoTaoModel) {
        this.disabledBtn = true;
        this.LoaiDaoTaoService.update(_item).subscribe((res: any) => {
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

    Create(_item: LoaiDaoTaoModel, withBack: boolean) {
        this.disabledBtn = true;
        this.LoaiDaoTaoService.create(_item).subscribe((res: any) => {
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
            this.NotificationCustom("Số thứ tự không được vượt quá 2,147,483,647");
            // Chỉ chặn nếu không phải là đang bôi đen để ghi đè
            if (inputValue.selectionStart === inputValue.selectionEnd) {
                e.preventDefault();
            }
        }

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

}

