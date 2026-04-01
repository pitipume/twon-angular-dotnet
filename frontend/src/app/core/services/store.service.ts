import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResult, OrderDto } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class StoreService {
  private http = inject(HttpClient);

  createOrder(productIds: string[]) {
    return firstValueFrom(
      this.http.post<BaseResult<OrderDto>>(`${environment.apiUrl}/api/store/orders`, { productIds }, { withCredentials: true })
    );
  }

  getOrder(orderId: string) {
    return firstValueFrom(
      this.http.get<BaseResult<OrderDto>>(`${environment.apiUrl}/api/store/orders/${orderId}`, { withCredentials: true })
    );
  }
}
