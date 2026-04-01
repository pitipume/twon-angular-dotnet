import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CatalogService } from '../../../core/services/catalog.service';
import { ProductDto } from '../../../core/models/api.models';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="max-w-6xl mx-auto px-4 py-10">
      <div class="mb-8">
        <h1 class="text-3xl font-bold mb-2">Discover</h1>
        <p class="text-white/50">Ebooks and tarot decks for your journey.</p>
      </div>

      <!-- Filter tabs -->
      <div class="flex gap-2 mb-8">
        @for (tab of tabs; track tab.value) {
          <button (click)="setFilter(tab.value)"
            [class]="activeFilter() === tab.value
              ? 'bg-violet-600 text-white px-4 py-1.5 rounded-full text-sm font-medium'
              : 'bg-white/5 text-white/60 hover:text-white px-4 py-1.5 rounded-full text-sm transition-colors'">
            {{ tab.label }}
          </button>
        }
      </div>

      @if (loading()) {
        <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          @for (n of [1,2,3,4,5,6,7,8]; track n) {
            <div class="bg-white/5 rounded-xl aspect-[3/4] animate-pulse"></div>
          }
        </div>
      } @else {
        <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          @for (product of products(); track product.id) {
            <a [routerLink]="['/products', product.id]"
              class="group bg-white/5 hover:bg-white/10 border border-white/5 hover:border-white/20 rounded-xl overflow-hidden transition-all">
              <div class="aspect-[3/4] bg-white/5 relative overflow-hidden">
                @if (product.coverImageUrl) {
                  <img [src]="product.coverImageUrl" [alt]="product.title"
                    class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                } @else {
                  <div class="w-full h-full flex items-center justify-center text-white/20 text-4xl">
                    {{ product.productType === 'EBOOK' ? '📖' : '🃏' }}
                  </div>
                }
                <div class="absolute top-2 right-2">
                  <span class="text-xs bg-black/60 backdrop-blur px-2 py-0.5 rounded-full text-white/70">
                    {{ product.productType === 'EBOOK' ? 'Ebook' : 'Tarot' }}
                  </span>
                </div>
              </div>
              <div class="p-3">
                <h3 class="font-medium text-sm leading-tight line-clamp-2">{{ product.title }}</h3>
                @if (product.author) {
                  <p class="text-white/40 text-xs mt-0.5">{{ product.author }}</p>
                }
                <p class="text-violet-400 text-sm font-semibold mt-2">฿{{ product.priceTHB }}</p>
              </div>
            </a>
          }
        </div>
        @if (products().length === 0) {
          <div class="text-center py-20 text-white/30">No products found.</div>
        }
      }
    </div>
  `,
})
export class ProductListComponent implements OnInit {
  private catalog = inject(CatalogService);

  tabs = [
    { label: 'All', value: '' },
    { label: 'Ebooks', value: 'EBOOK' },
    { label: 'Tarot Decks', value: 'TAROT_DECK' },
  ];

  products = signal<ProductDto[]>([]);
  loading = signal(true);
  activeFilter = signal('');

  async ngOnInit() {
    await this.loadProducts();
  }

  async setFilter(value: string) {
    this.activeFilter.set(value);
    await this.loadProducts();
  }

  private async loadProducts() {
    this.loading.set(true);
    try {
      const res = await this.catalog.getProducts(this.activeFilter() as any || undefined);
      this.products.set(res.data ?? []);
    } finally {
      this.loading.set(false);
    }
  }
}
