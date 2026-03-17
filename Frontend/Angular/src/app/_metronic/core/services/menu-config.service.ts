// Angular
import { Injectable } from '@angular/core';
// RxJS
import { Subject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import { MenuServices } from './menu.service';

@Injectable()
export class MenuConfigService {
	onConfigUpdated$: Subject<any>;
	private menuConfig: any;

	/**
	 * Service Constructor
	 */
	constructor(
		private menuPhanQuyenServices: MenuServices,) {
		this.onConfigUpdated$ = new Subject();
	}

	/**
	 * Returns the menuConfig
	 */
	async getMenus() {
		//lấy menu phân quyền
		let res: any = await this.layMenu().then();
		let menu;
		menu = this.fs_AssignSales(res.data); //lấy config menu 
		return menu;
	}

	getLanguge() {
		try {
			const lang = localStorage.getItem("language");
			if (lang === null) return 'vi';
			return lang.toString();
		} catch (error) {
			return 'vi';
		}
	}

	layMenu() {
		return this.menuPhanQuyenServices.layMenuChucNang().toPromise();
	}

	fs_AssignSales(dt: any) {
		let config: any = {
			header: {
				self: {},
				items: []
			},
			aside: {
				self: {},
				items: []
			}
		};

		// let arr = [];
		dt.forEach((item: any, index: any) => {
			if (item.Child.length > 0) {
				let _module: any = {
					title: '' + item.MenuName,
					root: item.Child ? item.Child.length > 0 : true,
					icon: '' + item.Icon,
					svg: ''
				}
				if (item.Child.length > 0) {
					_module["bullet"] = 'dot';
					_module["submenu"] = [];
					item.Child.forEach((itemE: any, indexE: any) => {
						let _submenu1: any = {
							title: '' + itemE.MenuName,
							icon: '' + itemE.Icon,
							root: itemE.Child ? itemE.Child.length == 0 : true,
							page: '' + itemE.Link,
						};
						if (itemE.Child.length > 0) {
							_submenu1["bullet"] = 'dot';
							_submenu1["submenu"] = [];
							itemE.Child.forEach((itemF: any, indexF: any) => {
								let _submenu2 = {
									title: '' + itemF.MenuName,
									icon: '' + itemF.Icon,
									root: itemF.Child ? itemF.Child.length == 0 : true,
									page: '' + itemF.Link,
								};
								_submenu1["submenu"].push(_submenu2)
							});
						}

						_module["submenu"].push(_submenu1)
					});
				}
				config.aside.items.push(_module);
			}
			else {
				if (item.Link != '#') {
					let _module = {
						title: '' + item.MenuName,
						root: item.Child ? item.Child.length == 0 : true,
						icon: '' + item.Icon,
						page: '' + item.Link
					};
					config.aside.items.push(_module);
				}
			}

		});
		return config;
	}
	/**
	 * Load config
	 *
	 * @param config: any
	 */
	loadConfigs(config: any) {
		this.menuConfig = config;
		this.onConfigUpdated$.next(this.menuConfig);
	}
}