import { Injectable, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, of, Subscription } from 'rxjs';
import { map, catchError, switchMap, finalize } from 'rxjs/operators';
import { UserModel } from '../models/user.model';
import { AuthModel } from '../models/auth.model';
import { AuthHTTPService } from './auth-http';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { AuthSSO } from '../models/authSSO.model';
import { CookieService } from 'ngx-cookie-service';
export type UserType = UserModel | undefined;
import jwt_decode from 'jwt-decode';

const KEY_SSO_TOKEN = 'sso_token';
const KEY_SSO_TOKEN_2 = 'sso_token_2';
const KEY_RESRESH_TOKEN = 'sso_token_refresh';
const API_IDENTITY = `${environment.HOST_IDENTITYSERVER_API}`;
const API_IDENTITY_USER = `${API_IDENTITY}/user/me`;
const API_IDENTITY_REFESHTOKEN = `${API_IDENTITY}/user/refresh`;
const API_IDENTITY_LOGOUT = `${API_IDENTITY}/user/logout`;
@Injectable({
  providedIn: 'root',
})
export class AuthService implements OnDestroy {
  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  private authLocalStorageToken = `${environment.appVersion}-${environment.USERDATA_KEY}`;

  // public fields
  currentUser$: Observable<UserType>;
  isLoading$: Observable<boolean>;
  currentUserSubject: BehaviorSubject<UserType>;
  isLoadingSubject: BehaviorSubject<boolean>;

  get currentUserValue(): UserType {
    return this.currentUserSubject.value;
  }

  set currentUserValue(user: UserType) {
    this.currentUserSubject.next(user);
  }
  //============================================================================
  private userSubject = new BehaviorSubject<any | null>(null);
  User$: Observable<any> = this.userSubject.asObservable();
  constructor(
    private authHttpService: AuthHTTPService,
    private router: Router,
    private http: HttpClient,
    private cookieService: CookieService,
  ) {
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.currentUserSubject = new BehaviorSubject<UserType>(undefined);
    this.currentUser$ = this.currentUserSubject.asObservable();
    this.isLoading$ = this.isLoadingSubject.asObservable();

    if (this.getAccessToken_cookie()) {
      this.getUserMeFromSSO().subscribe(
        (data) => {
          if (data && data.access_token) {
            this.userSubject.next(data);
            this.saveToken_cookie(data.access_token, data.refresh_token);
            this.getCode().subscribe((res) => {
              if (res && res.status == 1) {
                localStorage.setItem('cusCode', res.data);
              }
            })
            // this.getRoles().subscribe((res) => {
            //   if (res && res.status == 1) {
            //     localStorage.setItem('listRoles', res.data);
            //   }
            // })
          }
        },
        (error) => {
          this.refreshToken().subscribe(
            (data: AuthSSO) => {
              if (data && data.access_token) {
                this.userSubject.next(data);
                this.saveToken_cookie(data.access_token, data.refresh_token);
              }
            },
            (error) => {
              this.logout();
            }
          );
        },
        () => {
          setInterval(() => {
            if (!this.getAccessToken_cookie() && !this.getRefreshToken_cookie()) this.prepareLogout();
          }, 3000);
        }
      );
    }
    setInterval(() => this.autoGetUserFromSSO(), 60000);

  }

  // public methods
  login(email: string, password: string): Observable<UserType> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.login(email, password).pipe(
      map((auth: AuthModel) => {
        const result = this.setAuthFromLocalStorage(auth);
        return result;
      }),
      switchMap(() => this.getUserByToken()),
      catchError((err) => {
        console.error('err', err);
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  // logout() {
  //   localStorage.removeItem(this.authLocalStorageToken);
  //   this.router.navigate(['/auth/login'], {
  //     queryParams: {},
  //   });
  // }

  getUserByToken(): Observable<UserType> {
    const auth = this.getAuthFromLocalStorage();
    if (!auth || !auth.authToken) {
      return of(undefined);
    }

    this.isLoadingSubject.next(true);
    return this.authHttpService.getUserByToken(auth.authToken).pipe(
      map((user: UserType) => {
        if (user) {
          this.currentUserSubject.next(user);
        } else {
          this.logout();
        }
        return user;
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  // need create new user then login
  registration(user: UserModel): Observable<any> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.createUser(user).pipe(
      map(() => {
        this.isLoadingSubject.next(false);
      }),
      switchMap(() => this.login(user.email, user.password)),
      catchError((err) => {
        console.error('err', err);
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  forgotPassword(email: string): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .forgotPassword(email)
      .pipe(finalize(() => this.isLoadingSubject.next(false)));
  }

  // private methods
  private setAuthFromLocalStorage(auth: AuthModel): boolean {
    // store auth authToken/refreshToken/epiresIn in local storage to keep user logged in between page refreshes
    if (auth && auth.authToken) {
      localStorage.setItem(this.authLocalStorageToken, JSON.stringify(auth));
      return true;
    }
    return false;
  }

  // private getAuthFromLocalStorage(): AuthModel | undefined {
  //   try {
  //     const lsValue = localStorage.getItem(this.authLocalStorageToken);
  //     if (!lsValue) {
  //       return undefined;
  //     }

  //     const authData = JSON.parse(lsValue);
  //     return authData;
  //   } catch (error) {
  //     console.error(error);
  //     return undefined;
  //   }
  // }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }

  //===================================================================================================

  autoGetUserFromSSO() {
    const auth = this.getAuthFromLocalStorage();
    if (auth) {
      this.saveNewUserMe();
    }
  }

  saveNewUserMe(data?: any) {
    if (data) {
      this.userSubject.next(data);
      this.saveToken_cookie(data.access_token, data.refresh_token);
    }
    this.getUserMeFromSSO().subscribe(
      (data) => {
        if (data && data.access_token) {
          this.userSubject.next(data);
          this.saveToken_cookie(data.access_token, data.refresh_token);
        }
      },
      (error) => {
        this.refreshToken().subscribe(
          (data: AuthSSO) => {
            if (data && data.access_token) {
              this.userSubject.next(data);
              this.saveToken_cookie(data.access_token, data.refresh_token);
            }
          },
          (error) => {
            this.logout();
          }
        );
      }
    );
  }

  saveToken_cookie(access_token?: string, refresh_token?: string) {
    const DOMAIN = this.getDomainCookie();
    if (access_token) {
      this.cookieService.set(KEY_SSO_TOKEN, access_token.slice(0, 4000), 365, '/', DOMAIN);
      this.cookieService.set(KEY_SSO_TOKEN_2, access_token.slice(4000), 365, '/', DOMAIN);
    }
    if (refresh_token) this.cookieService.set(KEY_RESRESH_TOKEN, refresh_token, 365, '/', DOMAIN);
  }

  //Get domain cookie - Redirect
  getDomainCookie(): string {
    let domain = '';
    let _hostname = window.location.hostname;
    if (_hostname == 'localhost') {
      domain = _hostname
    } else {
      let hostname = ''
      hostname = _hostname.replace(_hostname.split('.')[0] + '.', '');
      domain = hostname
    }
    return domain;
  }
  getDomainRedirect(): string {
    let domain = '';
    let _hostname = window.location.hostname;
    if (_hostname == 'localhost') {
      domain = 'jee.vn'
    } else {
      let hostname = ''
      hostname = _hostname.replace(_hostname.split('.')[0] + '.', '');
      domain = hostname
    }
    return domain;
  }
  //Start - Get or delete access và refresh token cookie
  getAccessToken_cookie() {
    const access_token = this.cookieService.get(KEY_SSO_TOKEN) + this.cookieService.get(KEY_SSO_TOKEN_2);
    return access_token;
  }
  getRefreshToken_cookie() {
    const sso_token = this.cookieService.get(KEY_RESRESH_TOKEN);
    return sso_token;
  }
  deleteAccessRefreshToken_cookie() {
    const DOMAIN = this.getDomainCookie();
    this.cookieService.delete(KEY_SSO_TOKEN, '/', DOMAIN);
    this.cookieService.delete(KEY_SSO_TOKEN_2, '/', DOMAIN);
    this.cookieService.delete(KEY_RESRESH_TOKEN, '/', DOMAIN);
  }
  //End - Get access và refresh token cookie
  //Start - Code xử lý cho phần logout or hết hạn token====
  prepareLogout() {
    this.deleteAccessRefreshToken_cookie();
    let url = '';
    const redirectUrl = "https://portal." + this.getDomainRedirect() + '/sso?redirectUrl=';
    if (document.location.port) {
      url = redirectUrl + document.location.protocol + '//' + document.location.hostname + ':' + document.location.port;
    } else {
      url = redirectUrl + document.location.protocol + '//' + document.location.hostname;
    }
    window.location.href = url;
  }

  code: any;
  logout() {
    const access_token = this.getAccessToken_cookie();
    if (access_token) {
      this.getCode().subscribe((res) => {
        if (res && res.status == 1) {
          this.code = res.data;
        }
        this.logoutToSSO().subscribe(
          (res) => {
            this.prepareLogoutCode(this.code)
          },
          (err) => {
            this.prepareLogoutCode(this.code)
          }
        );
      },
        (error) => {
          this.prepareLogoutCode(this.code)
        })
    } else {
      this.prepareLogoutNotToken();
    }
  }

  prepareLogoutCode(code: any) {
    if (!window.navigator.onLine) return;
    this.LogOutOs();
    this.deleteAccessRefreshToken_cookie();
    localStorage.setItem('id_process', "");
    localStorage.removeItem('filterSearchLog');
    localStorage.removeItem('isSearchLog');
    let url = '';
    const redirectUrl = "https://portal." + this.getDomainRedirect() + '/sso?redirectUrl=';
    if (document.location.port) {
      url = redirectUrl + document.location.protocol + '//' + document.location.hostname + ':' + document.location.port + '&code=' + code;
    } else {
      url = redirectUrl + document.location.protocol + '//' + document.location.hostname + '&code=' + code;
    }
    window.location.href = url;
  }

  prepareLogoutNotToken() {
    if (!window.navigator.onLine) return;
    this.deleteAccessRefreshToken_cookie();
    localStorage.setItem('id_process', "");
    localStorage.removeItem('filterSearchLog');
    localStorage.removeItem('isSearchLog');
    let url = '';
    const link = window.location.href;
    const redirectUrl = "https://portal." + this.getDomainRedirect() + '/sso?redirectUrl=';
    if (link.includes('?')) {
      const httpParams = new HttpParams({ fromString: link.split('?')[1] });
      if (httpParams.get('code') != "") {
        if (document.location.port) {
          url = redirectUrl + document.location.protocol + '//' + document.location.hostname + ':' + document.location.port + '&code=' + httpParams.get('code');
        } else {
          url = redirectUrl + document.location.protocol + '//' + document.location.hostname + '&code=' + httpParams.get('code');
        }
      }
    } else {
      if (document.location.port) {
        if (localStorage.getItem('cusCode') != "" && localStorage.getItem('cusCode') != null) {
          url = redirectUrl + document.location.protocol + '//' + document.location.hostname + ':' + document.location.port + '&code=' + localStorage.getItem('cusCode');
        } else {
          url = redirectUrl + document.location.protocol + '//' + document.location.hostname + ':' + document.location.port;
        }
      } else {
        if (localStorage.getItem('cusCode') != "" && localStorage.getItem('cusCode') != null) {
          url = redirectUrl + document.location.protocol + '//' + document.location.hostname + '&code=' + localStorage.getItem('cusCode');
        } else {
          url = redirectUrl + document.location.protocol + '//' + document.location.hostname;
        }
      }
    }

    window.location.href = url;
  }

  LogOutOs() {
    let host = "https://portal." + this.getDomainRedirect();
    const iframeSource = `${host}/?logout=true`
    const iframe = document.createElement('iframe')
    iframe.setAttribute('src', iframeSource)
    iframe.style.display = 'none'
    document.body.appendChild(iframe)
    window.addEventListener(
      'message',
      () => {
        this.logout();
      },
      false
    )
  }
  //End - Code xử lý cho phần logout or hết hạn token====

  //=====Gán giá trị storage====
  getAuthFromLocalStorage() {
    return this.userSubject.value;
  }
  getUserId() {
    var auth = this.getAuthFromLocalStorage();
    return auth.user.customData['jee-account'].userID;
  }
  //============================
  //========Start - Call API Service liên quan==============
  getUserMeFromSSO(): Observable<any> {
    const access_token = this.getAccessToken_cookie();
    const url = API_IDENTITY_USER;
    const httpHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${access_token}`,
    });
    return this.http.get<any>(url, { headers: httpHeader });
  }

  refreshToken(): Observable<any> {
    const refresh_token = this.getRefreshToken_cookie();
    const url = API_IDENTITY_REFESHTOKEN;
    const httpHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${refresh_token}`,
    });
    return this.http.post<any>(url, null, { headers: httpHeader });
  }
  getCode(): Observable<any> {
    const auth = this.getAuthFromLocalStorage();
    const httpHeader = new HttpHeaders({
      Authorization: `Bearer ${auth != null ? auth.access_token : ''}`,
    });
    return this.http.get<any>(environment.HOST_JEEACCOUNT_API + '/api/accountpassword/getCode', {
      headers: httpHeader,
    });
  }
  logoutToSSO(): Observable<any> {
    const access_token = this.getAccessToken_cookie();
    const url = API_IDENTITY_LOGOUT;
    const httpHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${access_token}`,
    });
    return this.http.post<any>(url, null, { headers: httpHeader });
  }
  //========End - Call API Service liên quan==============

  //============Call từ auth.guard.ts==========
  isAuthenticated(): boolean {
    const access_token = this.getAccessToken_cookie();
    const refresh_token = this.getRefreshToken_cookie();
    if (access_token) {
      if (this.isTokenExpired(access_token)) {
        this.saveToken_cookie(access_token);
        return true;
      }
    }
    if (refresh_token) {
      if (this.isTokenExpired(refresh_token)) {
        this.saveToken_cookie(undefined, refresh_token);
        return true;
      }
    }
    return false;
  }

  isTokenExpired(token: string): boolean {
    const date = this.getTokenExpirationDate(token);
    if (!date) return false;
    return date.valueOf() > new Date().valueOf();
  }

  getTokenExpirationDate(auth: string): any {
    let decoded: any = jwt_decode(auth);
    if (!decoded.exp) return null;
    const date = new Date(0);
    date.setUTCSeconds(decoded.exp);
    return date;
  }

  getParamsSSO(): any {
    const url = window.location.href;
    let paramValue = undefined;
    if (url.includes('?')) {
      const httpParams = new HttpParams({ fromString: url.split('?')[1] });
      paramValue = httpParams.get('sso_token');
    }
    return paramValue;
  }
}

