import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ElementRef, ChangeDetectorRef, inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TutorialService } from '../services/tutorial-service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TutorialModel } from '../model/tutorial.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { LayoutUtilsService } from 'src/app/_metronic/core/utils/layout-utils.service';
import Swal from 'sweetalert2';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { FORMATSDMY } from '../custom-date-format';
import { ReplaySubject } from 'rxjs';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatSelectModule } from '@angular/material/select';
import { FileServerModel, UploadFileConfig } from 'src/app/_metronic/core/component/upload-file-custom/models/upload-file-custom.model';
import { UploadFileCustomModule } from 'src/app/_metronic/core/component/upload-file-custom/upload-file-custom.module';
import { QuillModule } from 'ngx-quill';
import { formats, quillConfig } from 'src/app/_metronic/core/component/quill-editor/quill_config';

@Component({
	selector: 'app-tutorial-edit-dialog',
	standalone: true,
	providers: [TutorialService, LayoutUtilsService,
		{ provide: MAT_DATE_LOCALE, useValue: 'vi' },
		{ provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
		{ provide: MAT_DATE_FORMATS, useValue: FORMATSDMY },
	],
	imports: [CommonModule, FormsModule, MatFormFieldModule, MatTooltipModule, TranslateModule, ReactiveFormsModule, MatIconModule,
		MatDatepickerModule, NgxMatSelectSearchModule, MatSelectModule, UploadFileCustomModule, QuillModule,
	],
	templateUrl: './tutorial-edit.dialog.component.html',
})
export class TutorialEditDialogComponent implements OnInit {
	private translate = inject(TranslateService);
	private changeDetectorRefs = inject(ChangeDetectorRef);
	private fb = inject(FormBuilder);
	private tutorialService = inject(TutorialService);
	private layoutUtilsService = inject(LayoutUtilsService);

	item: TutorialModel;
	itemForm: FormGroup;
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	@ViewChild("focusInput") focusInput!: ElementRef;
	disabledBtn: boolean = false;
	//===Khai báo sử dụng uploadfile
	uploadFileConfig: UploadFileConfig = new UploadFileConfig();
	fileServerList: FileServerModel[] = [];
	FileDinhKem: File[] = [];
	FileDinhKemTuKhoLuuTru: [] = [];
	lstFileDelete: string = "";

	//Khai báo sử dụng quill-editor
	quillConfig = quillConfig;
	formats = formats;
	editorStyles = {
		height: '200px',
		'font-size': '12pt',
		'overflow-y': 'auto',
		border: '1px solid #ccc',
		'border-bottom-right-radius': '4px',
		'border-bottom-left-radius': '4px',
	};
	_description: string = '';

	constructor(public dialogRef: MatDialogRef<TutorialEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
	) { }
	/** LOAD DATA */
	ngOnInit() {
		this.item = this.data;
		this.KhoiTaoUploadConfig();
		this.reset();
		if (this.item.RowID > 0) {
			this.viewLoading = true;
			this.tutorialService.getDetail(this.item.RowID).subscribe((res: any) => {
				this.item = res.data;
				this.createForm();
				this.changeDetectorRefs.detectChanges();
			});
		}
		else {
			this.viewLoading = false;
			this.createForm();
		}
		this.loadGeneral();
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
			EventCode: ['' + this.item.EventCode, Validators.required],
			EventName: ['' + this.item.EventName, [Validators.required]],
			EventDate: ['' + this.item.EventDate, [Validators.required]],
			EventSelect: ['' + this.item.EventSelect, [Validators.required]],
			EventSelectsearch: ['', [Validators.required]],
			EventTextarea: ['', [Validators.required]]
		});
		this.itemForm.controls["EventCode"].markAsTouched();
		this.itemForm.controls["EventName"].markAsTouched();
		this.itemForm.controls["EventDate"].markAsTouched();
		this.itemForm.controls["EventSelect"].markAsTouched();
		this.itemForm.controls["EventSelectsearch"].markAsTouched();
		this.itemForm.controls["EventTextarea"].markAsTouched();

	}

	/** UI */
	getTitle(): string {
		let result = this.translate.instant('COMMON.themmoi');
		if (!this.item || !this.item.RowID) {
			return result;
		}

		result = this.translate.instant('COMMON.capnhat');
		return result;
	}
	/** ACTIONS */
	prepareCustomer(): TutorialModel {
		const controls = this.itemForm.controls;
		const _item = new TutorialModel();
		_item.RowID = this.item.RowID;
		_item.EventCode = controls['EventCode'].value;
		_item.EventName = controls['EventName'].value;
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
		const updatedegree = this.prepareCustomer();
		if (updatedegree.RowID > 0) {
			this.Update(updatedegree);
		} else {
			this.Create(updatedegree, withBack);
		}
	}

	Update(_item: TutorialModel) {
		this.disabledBtn = true;
		this.tutorialService.update(_item).subscribe((res: any) => {
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

	Create(_item: TutorialModel, withBack: boolean) {
		this.disabledBtn = true;
		this.tutorialService.create(_item).subscribe((res: any) => {
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

	loadGeneral() {
		this.tutorialService.getAllGio().subscribe(res => {
			this.GioNghi = res.data;
			this.setUpDropSearchTuGio();
		});
	}

	//=========================
	GioNghi: any[] = [];
	public bankTuGio: FormControl = new FormControl();
	public filteredBanksTuGio: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
	setUpDropSearchTuGio() {
		this.bankTuGio.setValue('');
		this.filterBanksTuGio();
		this.bankTuGio.valueChanges
			.pipe()
			.subscribe(() => {
				this.filterBanksTuGio();
			});
	}

	protected filterBanksTuGio() {
		if (!this.GioNghi) {
			return;
		}
		// get the search keyword
		let search = this.bankTuGio.value;
		if (!search) {
			this.filteredBanksTuGio.next(this.GioNghi.slice());
			return;
		} else {
			search = search.toLowerCase();
		}
		// filter the banks
		this.filteredBanksTuGio.next(
			this.GioNghi.filter(bank => bank.Gio.toLowerCase().indexOf(search) > -1)
		);
	}

	//Function sử fungj upload file
	KhoiTaoUploadConfig() {
		this.uploadFileConfig.IsMultiFile = true;
		this.uploadFileConfig.Display = this.item.RowID > 0 ? 0 : 1;
	}

	GetListFileDelete(listDel: any) {
		this.lstFileDelete = "";
		listDel.forEach((element: any) => {
			if (this.lstFileDelete == "") this.lstFileDelete += element.Id;
			else this.lstFileDelete += "," + element.Id;
		});
	}
	GetListFileUploadNew(listUpload: any) {
		this.FileDinhKem = [];
		listUpload.forEach((element: any) => {
			this.FileDinhKem.push(element.File);
		});
	}
	GetListFileUploadTuKhoLuuTruNew(listUpload: any) {
		if (listUpload.length > 0) {
			this.FileDinhKemTuKhoLuuTru = listUpload;
		}
	}

	//=============Control sử dụng editor==============
	onChangeNote(event: any) {

	}
}
