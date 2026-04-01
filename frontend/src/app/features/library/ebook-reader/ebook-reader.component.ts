import { Component, signal, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { LibraryService } from '../../../core/services/library.service';

@Component({
  selector: 'app-ebook-reader',
  standalone: true,
  imports: [CommonModule, PdfViewerModule],
  template: `
    <div class="flex flex-col h-screen bg-[#0f0f13]">
      <!-- Top bar -->
      <div class="flex items-center justify-between px-6 py-3 border-b border-white/10 bg-black/30">
        <button onclick="history.back()" class="text-white/50 hover:text-white text-sm transition-colors">← Back</button>
        <span class="text-sm text-white/50">
          {{ currentPage() }} / {{ totalPages() }}
        </span>
        <div class="flex items-center gap-2">
          <button (click)="prevPage()" [disabled]="currentPage() <= 1"
            class="text-white/50 hover:text-white disabled:opacity-30 transition-colors px-2">‹</button>
          <button (click)="nextPage()" [disabled]="currentPage() >= totalPages()"
            class="text-white/50 hover:text-white disabled:opacity-30 transition-colors px-2">›</button>
        </div>
      </div>

      <!-- PDF viewer -->
      <div class="flex-1 overflow-auto flex justify-center py-4 px-4">
        @if (loading()) {
          <div class="flex items-center text-white/30">Loading ebook...</div>
        } @else if (signedUrl()) {
          <pdf-viewer
            [src]="signedUrl()!"
            [page]="currentPage()"
            [original-size]="false"
            [fit-to-page]="true"
            [show-all]="false"
            [render-text]="false"
            [external-link-target]="'blank'"
            (after-load-complete)="onLoaded($event)"
            style="width: 100%; max-width: 800px; display: block;">
          </pdf-viewer>
        }
      </div>
    </div>
  `,
})
export class EbookReaderComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private library = inject(LibraryService);

  private productId = '';
  signedUrl = signal<string | null>(null);
  currentPage = signal(1);
  totalPages = signal(0);
  loading = signal(true);
  private saveTimer?: ReturnType<typeof setTimeout>;

  async ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('productId')!;
    try {
      const res = await this.library.getEbookSession(this.productId);
      if (res.code === 'A001' && res.data) {
        this.signedUrl.set(res.data.signedUrl);
        this.totalPages.set(res.data.totalPages);
        this.currentPage.set(res.data.currentPage || 1);
      }
    } finally {
      this.loading.set(false);
    }
  }

  ngOnDestroy() {
    if (this.saveTimer) clearTimeout(this.saveTimer);
  }

  onLoaded(pdf: any) {
    this.totalPages.set(pdf.numPages);
  }

  prevPage() {
    if (this.currentPage() > 1) {
      this.currentPage.update(p => p - 1);
      this.scheduleSave();
    }
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(p => p + 1);
      this.scheduleSave();
    }
  }

  private scheduleSave() {
    if (this.saveTimer) clearTimeout(this.saveTimer);
    this.saveTimer = setTimeout(() => {
      this.library.saveProgress(this.productId, this.currentPage());
    }, 1500);
  }
}
