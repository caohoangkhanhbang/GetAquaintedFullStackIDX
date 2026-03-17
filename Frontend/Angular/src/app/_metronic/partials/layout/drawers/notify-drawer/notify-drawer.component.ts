import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MenuServices } from 'src/app/_metronic/core/services/menu.service';
import { SocketioService } from 'src/app/_metronic/core/services/socketio.service';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-notify-drawer',
  templateUrl: './notify-drawer.component.html',
  styleUrls: ['./notify-drawer.component.scss'],
})
export class NotifyDrawerComponent implements OnInit {
  listNoti: any = [];
  @Output() loadUnreadList = new EventEmitter();
  constructor(
    public translate: TranslateService,
    private socketService: SocketioService,
    private changeDetectorRefs: ChangeDetectorRef,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.getListNoti();
    this.socketService.connect();
    this.socketService.listen().subscribe((res: any) => {
      this.loadUnreadList.emit(true)
      this.getListNoti();
    });
  }

  getListNoti() {
    this.socketService.getNotificationList('').subscribe((res) => {
      res.forEach((x: any) => {
        x.createdDate = moment(x.createdDate).format('hh:mm A - DD/MM/YYYY');
        if ((x.message_json == null || x.message_json.Content == null) && x.message_text == null) {
          x.message_text = 'Thông báo không có nội dung';
        }
      });
      this.listNoti = res;
      this.loadUnreadList.emit(true); //load thành công list load số thông báo chưa đọc
      this.changeDetectorRefs.detectChanges();
    });
  }

  clickRead(noti: any) {
    this.socketService.readNotification(noti._id).subscribe((res) => {
      this.listNoti.forEach((x: any) => {
        if (x.id == noti.id) {
          x.read = true;
        }
      });
      this.getListNoti();
      if (noti.message_json.Link != null && noti.message_json.Link != '') {
        let domain = '';
        if (noti.message_json.AppCode != environment.APPCODE) {
          domain = noti.message_json.Domain;
          window.open(domain + noti.message_json.Link, '_blank');
        } else {
          this.router.navigate([noti.message_json.Link]);
        }
      }
      this.loadUnreadList.emit(true);
    });
  }

  DanhDauDaXem() {
    this.socketService.ReadAll().subscribe((res) => {
      this.getListNoti();
      this.loadUnreadList.emit(true);
    });
  }
}
