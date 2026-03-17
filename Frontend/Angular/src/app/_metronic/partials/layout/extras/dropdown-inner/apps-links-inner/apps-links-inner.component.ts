import { ChangeDetectorRef, Component, HostBinding, OnInit } from '@angular/core';
import { SocketioService } from 'src/app/_metronic/core/services/socketio.service';

@Component({
  selector: 'app-apps-links-inner',
  templateUrl: './apps-links-inner.component.html',
  styleUrls: ['./apps-links-inner.component.scss'],
})
export class AppsLinksInnerComponent implements OnInit {
  @HostBinding('class') class =
    'menu menu-sub menu-sub-dropdown menu-column w-250px w-lg-325px';
  @HostBinding('attr.data-kt-menu') dataKtMenu = 'true';
  listApp: any = [];
  constructor(private socketService: SocketioService, private changeDetectorRefs: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.socketService.getListApp().subscribe((res:any) => {
      if (res.status == 1) {
        this.listApp = res.data;
      }else{
        this.listApp = [];
      }
      this.changeDetectorRefs.detectChanges();
    });
  }
}
