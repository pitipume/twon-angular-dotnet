import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LibraryService } from '../../../core/services/library.service';
import { LibraryItemDto } from '../../../core/models/api.models';

@Component({
  selector: 'app-library-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="max-w-6xl mx-auto px-4 py-10">
      <h1 class="text-3xl font-bold mb-2">My Library</h1>
      <p class="text-white/50 mb-8">Your purchased ebooks and tarot decks.</p>

      @if (loading()) {
        <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          @for (n of [1,2,3,4]; track n) {
            <div class="bg-white/5 rounded-xl aspect-[3/4] animate-pulse"></div>
          }
        </div>
      } @else if (items().length === 0) {
        <div class="text-center py-20 text-white/30">
          <p class="text-lg mb-2">Your library is empty.</p>
          <a routerLink="/" class="text-violet-400 hover:text-violet-300">Browse products</a>
        </div>
      } @else {
        <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          @for (item of items(); track item.id) {
            <a [routerLink]="getLink(item)"
              class="group bg-white/5 hover:bg-white/10 border border-white/5 hover:border-violet-500/50 rounded-xl overflow-hidden transition-all">
              <div class="aspect-[3/4] bg-white/5 relative overflow-hidden">
                @if (item.product?.coverImageUrl) {
                  <img [src]="item.product!.coverImageUrl" [alt]="item.product!.title"
                    class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                } @else {
                  <div class="w-full h-full flex items-center justify-center text-4xl text-white/20">
                    {{ item.product?.productType === 'EBOOK' ? '📖' : '🃏' }}
                  </div>
                }
                <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity flex items-end p-4">
                  <span class="text-white font-medium text-sm">
                    {{ item.product?.productType === 'EBOOK' ? 'Read now' : 'Open deck' }}
                  </span>
                </div>
              </div>
              <div class="p-3">
                <h3 class="font-medium text-sm line-clamp-2">{{ item.product?.title }}</h3>
                @if (item.product?.author) {
                  <p class="text-white/40 text-xs mt-0.5">{{ item.product!.author }}</p>
                }
              </div>
            </a>
          }
        </div>
      }
    </div>
  `,
})
export class LibraryListComponent implements OnInit {
  private library = inject(LibraryService);
  items = signal<LibraryItemDto[]>([]);
  loading = signal(true);

  async ngOnInit() {
    try {
      const res = await this.library.getLibrary();
      this.items.set(res.data ?? []);
    } finally {
      this.loading.set(false);
    }
  }

  getLink(item: LibraryItemDto): string[] {
    const type = item.product?.productType;
    if (type === 'EBOOK') return ['/library/ebook', item.productId];
    return ['/library/tarot', item.productId];
  }
}
