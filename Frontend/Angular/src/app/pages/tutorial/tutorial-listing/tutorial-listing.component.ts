import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, inject, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { Observable } from 'rxjs';
import { SweetAlertOptions } from 'sweetalert2';
import moment from 'moment';
import { IRoleModel, RoleService } from 'src/app/_fake/services/role.service';
import { Config } from 'datatables.net';
import { DataTablesResponse, TutorialService } from '../services/tutorial-service';
import { TranslateService } from '@ngx-translate/core';
import { TutorialModel } from '../model/tutorial.model';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { TutorialEditDialogComponent } from '../tutorial-edit/tutorial-edit.dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tutorial-listing',
  templateUrl: './tutorial-listing.component.html',
  styleUrls: ['./tutorial-listing.component.scss']
})
export class TutorialListingComponent implements OnInit, AfterViewInit, OnDestroy {
  private apiService = inject(TutorialService);
  private cdr = inject(ChangeDetectorRef);
  private translate = inject(TranslateService);
  public dialog = inject(MatDialog);

  isCollapsed1 = false;
  isCollapsed2 = true;

  isLoading = false;

  users: DataTablesResponse;

  datatableConfig: Config = {};

  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();

  // Single model
  item: Observable<any>;
  itemModel: any

  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;

  swalOptions: SweetAlertOptions = {};

  roles$: Observable<DataTablesResponse>;

  datatableConfigNew: Config = {};


  constructor() { }

  ngAfterViewInit(): void {
  }

  ngOnInit(): void {
    this.datatableConfigNew = {
      serverSide: true,
      ajax: (dataTablesParameters: any, callback) => {
        this.apiService.getData(dataTablesParameters).subscribe(resp => {
          callback(resp);
        });
      },
      columns: [
        {
          title: 'ID', data: 'RowID', render: function (data, type, row) {
            return `${data}`;
          }
        },
        {
          title: 'Code', data: 'EventCode', render: (data, type, full) => {
            return `<span>${data}</span>`;
          }
        },
        {
          title: 'Name', data: 'EventName', render: function (data) {
            return `<span>${data}</span>`;
          }
        }
      ],
    };
  }

  delete(id: number) {
    this.apiService.delete(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  edit(id: number) {
    const tutorialModel = new TutorialModel();
    tutorialModel.clear(); // Set all defaults fields
    tutorialModel.RowID = id;
    this.Update(tutorialModel);
  }

  ngOnDestroy(): void {
    this.reloadEvent.unsubscribe();
  }

  getTitle() {
    let result = this.itemModel.RowID > 0 ? this.translate.instant("COMMON.capnhat")
      : this.translate.instant("COMMON.themmoi");
    return result;
  }

  Add() {
    const tutorialModel = new TutorialModel();
    tutorialModel.clear(); // Set all defaults fields
    this.Update(tutorialModel);
  }

  Update(_item: TutorialModel) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '600px';
    dialogConfig.height = 'auto';
    dialogConfig.data = _item;

    const dialogRef = this.dialog.open(TutorialEditDialogComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(result => {
      this.reloadEvent.emit(true);
    });
  }

  getHeight(): any {
    let tmp_height = 0;
    tmp_height = window.innerHeight - 300;
    return tmp_height + 'px';
  }
}