import { AfterViewInit, ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MenuServices } from 'src/app/_metronic/core/services/menu.service';
import { SocketioService } from 'src/app/_metronic/core/services/socketio.service';
import { menuReinitialization } from 'src/app/_metronic/kt/kt-helpers';
import { UserModel } from 'src/app/modules/auth/models/user.model';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

@Component({
	selector: 'app-navbar',
	templateUrl: './navbar.component.html',
	styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent implements OnInit, AfterViewInit {
	@Input() appHeaderDefaulMenuDisplay: boolean;
	@Input() isRtl: boolean;

	itemClass: string = 'ms-1 ms-lg-3';
	btnClass: string = 'btn btn-icon btn-custom btn-icon-muted btn-active-light btn-active-color-primary w-35px h-35px w-md-40px h-md-40px';
	userAvatarClass: string = 'symbol-35px symbol-md-40px';
	btnIconClass: string = 'fs-2 fs-md-1';

	user: any;
	Avatar: string = '';
	Name: string = '';
	BgColor: string = '';
	NameSplit: string = '';
	SoLuongNhacNho: number = 0;
	numberInfo: number;
	constructor(
		private auth: AuthService,
		public socket: SocketioService,
		private changeDetectorRefs: ChangeDetectorRef,
		private menuServices: MenuServices,
	) {
		this.user = this.auth.getAuthFromLocalStorage();
		this.Avatar = this.user['user']['customData']['personalInfo'].Avatar.Avatar;
	}

	ngAfterViewInit(): void {
		menuReinitialization();
	}

	ngOnInit(): void {
		this.LoadDataNhacNho();
	 }

	updateNumberNoti(value: any) {
		if (value == true) {
			this.getNotiUnread();
		}
	}

	getNotiUnread() {
		this.socket.getNotificationList('unread').subscribe((res) => {
			let dem = 0;
			res.forEach((x: any) => dem++);
			this.numberInfo = dem;
			this.changeDetectorRefs.detectChanges();
		});
	}

	LoadDataNhacNho() {
		this.menuServices.Count_SoLuongNhacNho().subscribe((res: any) => {
			if (res) {
				this.SoLuongNhacNho = res.data;
				this.changeDetectorRefs.detectChanges();
			}
		});
	}
}
