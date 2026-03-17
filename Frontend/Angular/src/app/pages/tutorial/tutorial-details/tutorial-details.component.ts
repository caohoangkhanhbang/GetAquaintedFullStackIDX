import { AfterViewInit, ChangeDetectorRef, Component, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IUserModel, UserService } from 'src/app/_fake/services/user-service';
import { ICodeModel, TutorialService } from '../services/tutorial-service';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, NgForm } from '@angular/forms';
import { SweetAlertOptions } from 'sweetalert2';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';

@Component({
  selector: 'app-tutorial-details',
  providers: [TranslateService],
  templateUrl: './tutorial-details.component.html',
  styleUrls: ['./tutorial-details.component.scss']
})
export class TutorialDetailsComponent implements OnInit, AfterViewInit {
  private apiService = inject(TutorialService);
  public translate = inject(TranslateService);
  private activatedRoute = inject(ActivatedRoute);
  public router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  swalOptions: SweetAlertOptions = {};
  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;

  itemModel: any

  itemForm: FormGroup;

  isLoading = false;

  //=============================================================
  selectedDate: string;
  constructor() { }

  ngOnInit(): void {
    const id = Number(this.activatedRoute.snapshot.paramMap.get('id'));
    if (id > 0) {
      this.apiService.getDetail(id).subscribe((res: any) => {
        this.itemModel = res.data;
        this.cdr.detectChanges();
      });
    }
  }


  ngAfterViewInit(): void {
  }

  getTitle() {
    const id = Number(this.activatedRoute.snapshot.paramMap.get('id'));
    let result = id > 0 ? this.translate.instant("COMMON.capnhat")
      : this.translate.instant("COMMON.themmoi");
    return result;
  }

  close() {
    let _backUrl = 'apps/tutorial';
    this.router.navigateByUrl(_backUrl);
  }

  onSubmit(event: Event, myForm: NgForm) {
    if (myForm && myForm.invalid) {
      return;
    }

    this.isLoading = true;

    const successAlert: SweetAlertOptions = {
      icon: 'success',
      title: 'Success!',
      text: this.itemModel.RowID > 0 ? 'User updated successfully!' : 'User created successfully!',
    };
    const errorAlert: SweetAlertOptions = {
      icon: 'error',
      title: 'Error!',
      text: '',
    };

    const completeFn = () => {
      this.isLoading = false;
    };

    const updateFn = () => {
      this.apiService.update(this.itemModel).subscribe((res: any) => {
        if (res && res.status == 1) {
          successAlert.text = res.error.message;
          this.close();
        } else {
          errorAlert.text = res.error.message;
          this.showAlert(errorAlert);
          this.isLoading = false;
        }
      });
    };

    const createFn = () => {
      this.apiService.create(this.itemModel).subscribe(
        (res: any) => {
          if (res && res.status == 1) {
            successAlert.text = res.error.message;
            this.close();
          } else {
            errorAlert.text = res.error.message;
            this.showAlert(errorAlert);
            this.isLoading = false;
          }
        }
      );
    };

    if (this.itemModel.RowID > 0) {
      updateFn();
    } else {
      createFn();
    }
  }

  showAlert(swalOptions: SweetAlertOptions) {
    let style = swalOptions.icon?.toString() || 'success';
    if (swalOptions.icon === 'error') {
      style = 'danger';
    }
    this.swalOptions = Object.assign({
      buttonsStyling: false,
      confirmButtonText: "Ok, got it!",
      customClass: {
        confirmButton: "btn btn-" + style
      }
    }, swalOptions);
    this.cdr.detectChanges();
    this.noticeSwal.fire();
  }
}
