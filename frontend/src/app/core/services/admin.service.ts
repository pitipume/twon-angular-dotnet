import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);

  uploadEbook(form: FormData) {
    return firstValueFrom(
      this.http.post<BaseResult<{ productId: string; mongoId: string }>>(`${environment.apiUrl}/api/admin/ebooks`, form, { withCredentials: true })
    );
  }

  uploadTarotDeck(form: FormData) {
    return firstValueFrom(
      this.http.post<BaseResult<{ productId: string; mongoId: string; cardCount: number }>>(`${environment.apiUrl}/api/admin/tarot-decks`, form, { withCredentials: true })
    );
  }

  publish(productId: string) {
    return firstValueFrom(
      this.http.patch<BaseResult<null>>(`${environment.apiUrl}/api/admin/products/${productId}/publish`, {}, { withCredentials: true })
    );
  }

  unpublish(productId: string) {
    return firstValueFrom(
      this.http.patch<BaseResult<null>>(`${environment.apiUrl}/api/admin/products/${productId}/unpublish`, {}, { withCredentials: true })
    );
  }

  setPaymentConfig(bankName: string, accountName: string, accountNumber: string) {
    return firstValueFrom(
      this.http.put<BaseResult<null>>(`${environment.apiUrl}/api/admin/payment-config`, { bankName, accountName, accountNumber }, { withCredentials: true })
    );
  }

  uploadPaymentQr(qr: File) {
    const form = new FormData();
    form.append('qr', qr);
    return firstValueFrom(
      this.http.post<BaseResult<null>>(`${environment.apiUrl}/api/admin/payment-config/qr`, form, { withCredentials: true })
    );
  }
}
