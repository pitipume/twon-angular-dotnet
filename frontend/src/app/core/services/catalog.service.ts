import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResult, ProductDto } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class CatalogService {
  private http = inject(HttpClient);

  getProducts(type?: 'EBOOK' | 'TAROT_DECK') {
    const params = type ? `?type=${type}` : '';
    return firstValueFrom(
      this.http.get<BaseResult<ProductDto[]>>(`${environment.apiUrl}/api/catalog/products${params}`)
    );
  }

  getProduct(id: string) {
    return firstValueFrom(
      this.http.get<BaseResult<ProductDto>>(`${environment.apiUrl}/api/catalog/products/${id}`)
    );
  }
}
