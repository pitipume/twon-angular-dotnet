import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResult, AuthResponse, User } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private _accessToken = signal<string | null>(null);
  private _user = signal<User | null>(null);

  readonly accessToken = this._accessToken.asReadonly();
  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => !!this._accessToken());
  readonly isAdmin = computed(() => {
    const role = this._user()?.role;
    return role === 'ADMIN' || role === 'SUPER_ADMIN';
  });

  setSession(token: string, user: User) {
    this._accessToken.set(token);
    this._user.set(user);
  }

  clearSession() {
    this._accessToken.set(null);
    this._user.set(null);
  }

  async silentRefresh(): Promise<boolean> {
    try {
      const res = await firstValueFrom(
        this.http.post<BaseResult<AuthResponse>>(
          `${environment.apiUrl}/api/auth/refresh`, {}, { withCredentials: true }
        )
      );
      if (res.code === 'A001' && res.data) {
        this.setSession(res.data.accessToken, res.data.user);
        return true;
      }
      return false;
    } catch {
      return false;
    }
  }

  async initiateRegister(email: string, displayName: string, password: string) {
    return firstValueFrom(
      this.http.post<BaseResult<null>>(
        `${environment.apiUrl}/api/auth/register/initiate`,
        { email, displayName, password }
      )
    );
  }

  async verifyRegister(email: string, otp: string, displayName: string, password: string) {
    const res = await firstValueFrom(
      this.http.post<BaseResult<AuthResponse>>(
        `${environment.apiUrl}/api/auth/register/verify`,
        { email, otp, displayName, password },
        { withCredentials: true }
      )
    );
    if (res.code === 'A001' && res.data) {
      this.setSession(res.data.accessToken, res.data.user);
    }
    return res;
  }

  async login(email: string, password: string) {
    const res = await firstValueFrom(
      this.http.post<BaseResult<AuthResponse>>(
        `${environment.apiUrl}/api/auth/login`,
        { email, password },
        { withCredentials: true }
      )
    );
    if (res.code === 'A001' && res.data) {
      this.setSession(res.data.accessToken, res.data.user);
    }
    return res;
  }

  async logout() {
    try {
      await firstValueFrom(
        this.http.post(`${environment.apiUrl}/api/auth/logout`, {}, { withCredentials: true })
      );
    } finally {
      this.clearSession();
      this.router.navigate(['/login']);
    }
  }
}
