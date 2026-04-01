import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CatalogService } from '../../../core/services/catalog.service';
import { StoreService } from '../../../core/services/store.service';
import { AuthService } from '../../../core/services/auth.service';
import { ProductDto } from '../../../core/models/api.models';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="max-w-4xl mx-auto px-4 py-10">
      @if (loading()) {
        <div class="animate-pulse space-y-4">
          <div class="h-8 bg-white/5 rounded w-1/2"></div>
          <div class="h-4 bg-white/5 rounded w-1/4"></div>
        </div>
      } @else if (product()) {
        <div class="flex flex-col md:flex-row gap-8">
          <div class="w-full md:w-64 shrink-0">
            <div class="aspect-[3/4] bg-white/5 rounded-xl overflow-hidden">
              @if (product()!.coverImageUrl) {
                <img [src]="product()!.coverImageUrl" [alt]="product()!.title" class="w-full h-full object-cover" />
              } @else {
                <div class="w-full h-full flex items-center justify-center text-6xl text-white/20">
                  {{ product()!.productType === 'EBOOK' ? '📖' : '🃏' }}
                </div>
              }
            </div>
          </div>
          <div class="flex-1">
            <div class="text-xs text-violet-400 font-medium uppercase tracking-widest mb-2">
              {{ product()!.productType === 'EBOOK' ? 'Ebook' : 'Tarot Deck' }}
            </div>
            <h1 class="text-3xl font-bold mb-1">{{ product()!.title }}</h1>
            @if (product()!.author) {
              <p class="text-white/50 mb-4">by {{ product()!.author }}</p>
            }
            <p class="text-white/70 leading-relaxed mb-6">{{ product()!.description }}</p>
            <div class="flex flex-wrap gap-4 text-sm text-white/40 mb-8">
              @if (product()!.totalPages) {
                <span>{{ product()!.totalPages }} pages</span>
              }
              @if (product()!.cardCount) {
                <span>{{ product()!.cardCount }} cards</span>
              }
              @if (product()!.language) {
                <span>{{ product()!.language }}</span>
              }
            </div>
            <div class="flex items-center gap-4">
              <span class="text-3xl font-bold text-violet-400">฿{{ product()!.priceTHB }}</span>
              @if (auth.isLoggedIn()) {
                <button (click)="buyNow()" [disabled]="buying()"
                  class="bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white px-8 py-3 rounded-xl font-medium transition-colors">
                  {{ buying() ? 'Processing...' : 'Buy now' }}
                </button>
              } @else {
                <a routerLink="/login" class="bg-violet-600 hover:bg-violet-500 text-white px-8 py-3 rounded-xl font-medium transition-colors">
                  Login to buy
                </a>
              }
            </div>
            @if (error()) {
              <p class="text-red-400 text-sm mt-3">{{ error() }}</p>
            }
          </div>
        </div>
      }
    </div>
  `,
})
export class ProductDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private catalog = inject(CatalogService);
  private store = inject(StoreService);
  auth = inject(AuthService);

  product = signal<ProductDto | null>(null);
  loading = signal(true);
  buying = signal(false);
  error = signal('');

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    try {
      const res = await this.catalog.getProduct(id);
      this.product.set(res.data ?? null);
    } finally {
      this.loading.set(false);
    }
  }

  async buyNow() {
    const p = this.product();
    if (!p) return;
    this.buying.set(true);
    this.error.set('');
    try {
      const res = await this.store.createOrder([p.id]);
      if (res.code === 'A001' && res.data) {
        this.router.navigate(['/checkout', res.data.id]);
      } else {
        this.error.set(res.message ?? 'Could not create order.');
      }
    } catch {
      this.error.set('An error occurred.');
    } finally {
      this.buying.set(false);
    }
  }
}
