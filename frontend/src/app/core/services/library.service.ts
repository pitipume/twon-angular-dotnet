import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseResult, LibraryItemDto, EbookSession, TarotSession } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class LibraryService {
  private http = inject(HttpClient);

  getLibrary() {
    return firstValueFrom(
      this.http.get<BaseResult<LibraryItemDto[]>>(`${environment.apiUrl}/api/library`, { withCredentials: true })
    );
  }

  getEbookSession(productId: string) {
    return firstValueFrom(
      this.http.get<BaseResult<EbookSession>>(`${environment.apiUrl}/api/library/ebook/${productId}/session`, { withCredentials: true })
    );
  }

  saveProgress(productId: string, page: number) {
    return firstValueFrom(
      this.http.post<BaseResult<null>>(`${environment.apiUrl}/api/library/ebook/${productId}/progress`, { page }, { withCredentials: true })
    );
  }

  getTarotSession(productId: string) {
    return firstValueFrom(
      this.http.get<BaseResult<TarotSession>>(`${environment.apiUrl}/api/library/tarot/${productId}/session`, { withCredentials: true })
    );
  }
}
