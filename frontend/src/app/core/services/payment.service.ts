import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResult, PendingOrderDto } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private http = inject(HttpClient);

  submitSlip(orderId: string, slip: File, transferredAt: string, note?: string) {
    const form = new FormData();
    form.append('orderId', orderId);
    form.append('slip', slip);
    form.append('transferredAt', transferredAt);
    if (note) form.append('note', note);
    return firstValueFrom(
      this.http.post<BaseResult<null>>(`${environment.apiUrl}/api/payment/slip`, form, { withCredentials: true })
    );
  }

  getPendingOrders() {
    return firstValueFrom(
      this.http.get<BaseResult<PendingOrderDto[]>>(`${environment.apiUrl}/api/payment/orders/pending`, { withCredentials: true })
    );
  }

  approvePayment(orderId: string) {
    return firstValueFrom(
      this.http.post<BaseResult<null>>(`${environment.apiUrl}/api/payment/orders/${orderId}/approve`, {}, { withCredentials: true })
    );
  }

  rejectPayment(orderId: string, reason: string) {
    return firstValueFrom(
      this.http.post<BaseResult<null>>(`${environment.apiUrl}/api/payment/orders/${orderId}/reject`, { reason }, { withCredentials: true })
    );
  }
}
