import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import * as objectPath from 'object-path';
import { MenuConfigService } from './menu-config.service';
// Services

const emptyMenuConfig = {
	items: []
};

export const DynamicAsideMenuConfig = {
	items: [
	  {
		title: '',
		root: true,
		icon: 'flaticon2-browser-2',
		svg: '',
		page: '', 
		bullet: 'dot',
		submenu: [
		  {
			title: '',
			page: ''
		  },
		]
	  }
	]
  };

@Injectable({
	providedIn: 'root'
})
export class DynamicAsideMenuService {
	private menuConfigSubject = new BehaviorSubject<any>(emptyMenuConfig);
	menuConfig$: Observable<any>;
	constructor(private menuConfigService: MenuConfigService) {
		this.menuConfig$ = this.menuConfigSubject.asObservable();
		this.loadMenu();
		// register on config changed event and set default config
	}

	// Here you able to load your menu from server/data-base/localStorage
	// Default => from DynamicAsideMenuConfig
	private loadMenu() {
		this.setMenu(DynamicAsideMenuConfig);
	}

	private async setMenu(menuConfig: any) {
		const menuItems: any[] = objectPath.get(await this.menuConfigService.getMenus(), 'aside.items');
		menuConfig.items = menuItems;
		this.menuConfigSubject.next(menuConfig);
	}

	private getMenu(): any {
		return this.menuConfigSubject.value;
	}
}
