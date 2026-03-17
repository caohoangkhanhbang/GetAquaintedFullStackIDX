import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthGuard {
  constructor(private authService: AuthService) { }
  appCode = environment.APPCODE;
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    return new Promise<boolean>((resolve, reject) => {
      if (!this.authService.isAuthenticated()) {
        if (this.authService.getParamsSSO()) {
          this.authService.saveToken_cookie(this.authService.getParamsSSO());
        }

        resolve(this.canPassGuard());
      } else {
        resolve(this.canPassGuard());
      }
    });
  }

  canPassGuard(): Promise<boolean> {
    return new Promise<boolean>((resolve, reject) => {
      this.authService.getUserMeFromSSO().subscribe(
        (data) => {
          resolve(this.canPassGuardAccessToken(data));
        },
        (error) => {
          this.authService.refreshToken().subscribe(
            (data) => {
              resolve(this.canPassGuardAccessToken(data));
            },
            (error) => {
              resolve(this.unauthorizedGuard());
            }
          );
        }
      );
    });
  }

  canPassGuardAccessToken(data: any) {
    return new Promise<boolean>((resolve, reject) => {
      if (data && data.access_token) {
        this.authService.saveNewUserMe(data);
        // const lstAppCode: string[] = data['user']['customData']['jee-account']['appCode'];
        // if (lstAppCode) {
        //   if (lstAppCode.indexOf(this.appCode) === -1) {
        //     return resolve(this.unAppCodeAuthorizedGuard());
        //   } else {
        //     return resolve(true);
        //   }
        // } else {
        //   return resolve(this.unAppCodeAuthorizedGuard());
        // }
        return resolve(true);
      }
    });
  }

  unauthorizedGuard() {
    return new Promise<boolean>((resolve, reject) => {
      this.authService.logout();
      return resolve(false);
    });
  }

  unAppCodeAuthorizedGuard() {
    return new Promise<boolean>((resolve, reject) => {
      // const popup = this.layoutUtilsService.showActionNotification(
      //   'Bạn không có quyền truy cập trang này',
      //   MessageType.Read,
      //   999999999,
      //   true,
      //   false,
      //   3000,
      //   'top',
      //   0
      // );
      // this.authService.logoutToSSO().subscribe(() => {
      //   popup.afterDismissed().subscribe((res: any) => {
      //     const redirectUrl = "https://app." + this.authService.getDomainRedirect()
      //     window.open(redirectUrl);
      //     return resolve(false);
      //   });
      // });
      return resolve(false);
    });
  }
}
