import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { StoreService } from '../../../core/services/store.service';
import { PaymentService } from '../../../core/services/payment.service';
import { OrderDto } from '../../../core/models/api.models';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="max-w-2xl mx-auto px-4 py-10">
      <h1 class="text-2xl font-bold mb-6">Checkout</h1>

      @if (loading()) {
        <div class="animate-pulse space-y-3">
          <div class="h-6 bg-white/5 rounded w-1/2"></div>
          <div class="h-4 bg-white/5 rounded w-3/4"></div>
        </div>
      } @else if (order()) {
        <!-- Order summary -->
        <div class="bg-white/5 border border-white/10 rounded-xl p-6 mb-6">
          <h2 class="font-semibold mb-4">Order summary</h2>
          @for (item of order()!.orderItems; track item.id) {
            <div class="flex justify-between items-center py-2 border-b border-white/5 last:border-0">
              <span class="text-sm">{{ item.product?.title ?? item.productId }}</span>
              <span class="text-sm text-white/70">฿{{ item.priceTHB }}</span>
            </div>
          }
          <div class="flex justify-between items-center mt-4 text-lg font-semibold">
            <span>Total</span>
            <span class="text-violet-400">฿{{ order()!.totalTHB }}</span>
          </div>
        </div>

        <!-- Payment instructions -->
        @if (order()!.checkoutInfo) {
          <div class="bg-white/5 border border-white/10 rounded-xl p-6 mb-6">
            <h2 class="font-semibold mb-4">Transfer to</h2>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between">
                <span class="text-white/50">Bank</span>
                <span>{{ order()!.checkoutInfo!.bankName }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-white/50">Account name</span>
                <span>{{ order()!.checkoutInfo!.accountName }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-white/50">Account number</span>
                <span class="font-mono">{{ order()!.checkoutInfo!.accountNumber }}</span>
              </div>
            </div>
            @if (order()!.checkoutInfo!.qrImageUrl) {
              <div class="mt-4 flex justify-center">
                <img [src]="order()!.checkoutInfo!.qrImageUrl" alt="PromptPay QR" class="w-48 h-48 rounded-lg" />
              </div>
            }
          </div>
        }

        <!-- Slip upload -->
        @if (order()!.status === 'PENDING') {
          <div class="bg-white/5 border border-white/10 rounded-xl p-6">
            <h2 class="font-semibold mb-4">Upload payment slip</h2>
            @if (submitted()) {
              <div class="bg-green-500/10 border border-green-500/30 text-green-400 rounded-lg px-4 py-3 text-sm">
                Slip submitted! We'll verify and grant access shortly.
              </div>
            } @else {
              <div class="space-y-4">
                <div>
                  <label class="block text-sm text-white/60 mb-1">Transfer slip (image)</label>
                  <input type="file" accept="image/*" (change)="onFile($event)"
                    class="w-full text-sm text-white/70 file:mr-3 file:bg-violet-600 file:text-white file:border-0 file:px-4 file:py-1.5 file:rounded-lg file:cursor-pointer" />
                </div>
                <div>
                  <label class="block text-sm text-white/60 mb-1">Transfer date/time</label>
                  <input type="datetime-local" [(ngModel)]="transferredAt"
                    class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:border-violet-500" />
                </div>
                <div>
                  <label class="block text-sm text-white/60 mb-1">Note (optional)</label>
                  <input type="text" [(ngModel)]="note" placeholder="e.g. transferred at 14:30"
                    class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500" />
                </div>
                @if (error()) {
                  <p class="text-red-400 text-sm">{{ error() }}</p>
                }
                <button (click)="submitSlip()" [disabled]="!slipFile || !transferredAt || uploading()"
                  class="w-full bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white py-2.5 rounded-lg transition-colors font-medium">
                  {{ uploading() ? 'Uploading...' : 'Submit slip' }}
                </button>
              </div>
            }
          </div>
        } @else {
          <div class="bg-green-500/10 border border-green-500/30 text-green-400 rounded-lg px-4 py-3 text-sm">
            Payment status: {{ order()!.status }}
          </div>
        }
      }
    </div>
  `,
})
export class CheckoutComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private store = inject(StoreService);
  private payment = inject(PaymentService);

  order = signal<OrderDto | null>(null);
  loading = signal(true);
  uploading = signal(false);
  submitted = signal(false);
  error = signal('');
  slipFile: File | null = null;
  transferredAt = '';
  note = '';

  async ngOnInit() {
    const orderId = this.route.snapshot.paramMap.get('orderId')!;
    try {
      const res = await this.store.getOrder(orderId);
      this.order.set(res.data ?? null);
    } finally {
      this.loading.set(false);
    }
  }

  onFile(event: Event) {
    const input = event.target as HTMLInputElement;
    this.slipFile = input.files?.[0] ?? null;
  }

  async submitSlip() {
    if (!this.slipFile || !this.order()) return;
    this.uploading.set(true);
    this.error.set('');
    try {
      const res = await this.payment.submitSlip(
        this.order()!.id,
        this.slipFile,
        this.transferredAt,
        this.note || undefined
      );
      if (res.code === 'A001') {
        this.submitted.set(true);
      } else {
        this.error.set(res.message ?? 'Failed to submit slip.');
      }
    } catch {
      this.error.set('An error occurred.');
    } finally {
      this.uploading.set(false);
    }
  }
}
