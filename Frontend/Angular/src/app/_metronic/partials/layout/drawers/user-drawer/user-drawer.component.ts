import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MenuServices } from 'src/app/_metronic/core/services/menu.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-user-drawer',
  templateUrl: './user-drawer.component.html',
  styleUrls: ['./user-drawer.component.scss'],
})
export class UserDrawerComponent implements OnInit {
  _user: any;
  listNhacNho: any[] = [];
  AppCode: string = environment.APPCODE;
  constructor(
    public translate: TranslateService,
    private auth: AuthService,
    private menuServices: MenuServices,
    private changeDetectorRefs: ChangeDetectorRef,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this._user = this.auth.getAuthFromLocalStorage();
    this.LoadDataNhacNho();
  }

  quanlytaikhoan() {
    window.open(`${environment.HOST_JEELANDINGPAGE}/ThongTinCaNhan`, '_blank');
  }

  logout() {
    this.auth.logout();
  }

  public LoadDataNhacNho() {
    this.menuServices.Get_DSNhacNho().subscribe((res) => {
      if (res.status == 1 && res.data.length > 0) {
        this.listNhacNho = res.data;
      } else {
        this.listNhacNho = [];
      }
      this.changeDetectorRefs.detectChanges();
    });
  }

  ChangeLink(item: any) {
    if (item.WebAppLink != null && item.WebAppLink != '') {
      if (this.AppCode == item.AppCode) {
        this.router.navigate([item.WebAppLink]);
      } else {
        let link = environment.HOST_JEELANDINGPAGE + item.WebAppLink;
        window.open(link, '_blank');
      }
    } else {
      window.open(environment.HOST_JEELANDINGPAGE, '_blank');
    }
  }
}
