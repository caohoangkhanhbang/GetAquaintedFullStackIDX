import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { DynamicAsideMenuService } from 'src/app/_metronic/core/services/dynamic-aside-menu.service';

@Component({
  selector: 'app-sidebar-menu',
  templateUrl: './sidebar-menu.component.html',
  styleUrls: ['./sidebar-menu.component.scss']
})
export class SidebarMenuComponent implements OnInit {
  private menu = inject(DynamicAsideMenuService);
  private cdr = inject(ChangeDetectorRef);
  currentUrl: string;
  menuConfig: any;
  subscriptions: Subscription[] = [];
  constructor() { }

  ngOnInit(): void {
    // menu load
    const menuSubscr = this.menu.menuConfig$.subscribe(res => {
      this.menuConfig = res;
      // console.log(res)
      this.cdr.detectChanges();
    });
    this.subscriptions.push(menuSubscr);
  }

  isMenuItemActive(path: any) {
    if (!this.currentUrl || !path) {
      return false;
    }

    if (this.currentUrl === path) {
      return true;
    }

    if (this.currentUrl.indexOf(path) > -1) {
      return true;
    }

    return false;
  }

}
