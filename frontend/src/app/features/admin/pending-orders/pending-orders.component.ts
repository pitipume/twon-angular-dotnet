import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentService } from '../../../core/services/payment.service';
import { PendingOrderDto } from '../../../core/models/api.models';

@Component({
  selector: 'app-pending-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  template: `
    <div class="max-w-4xl mx-auto px-4 py-10">
      <h1 class="text-2xl font-bold mb-2">Pending Orders</h1>
      <p class="text-white/50 mb-8">Review payment slips and approve or reject.</p>

      @if (loading()) {
        <p class="text-white/30">Loading...</p>
      } @else if (orders().length === 0) {
        <p class="text-white/30 text-center py-20">No pending orders.</p>
      } @else {
        <div class="space-y-4">
          @for (order of orders(); track order.id) {
            <div class="bg-white/5 border border-white/10 rounded-xl p-6">
              <div class="flex items-start justify-between mb-4">
                <div>
                  <p class="font-medium">{{ order.user.displayName }}</p>
                  <p class="text-white/40 text-sm">{{ order.user.email }}</p>
                  <p class="text-white/30 text-xs mt-1">{{ order.createdAt | date:'medium' }}</p>
                </div>
                <span class="text-violet-400 font-bold text-lg">฿{{ order.totalTHB }}</span>
              </div>

              <div class="text-sm text-white/60 mb-4">
                @for (item of order.orderItems; track item.productId) {
                  <span class="mr-2">{{ item.product?.title }}</span>
                }
              </div>

              @if (order.payment?.slipUrl) {
                <div class="mb-4">
                  <a [href]="order.payment!.slipUrl" target="_blank"
                    class="text-violet-400 hover:text-violet-300 text-sm">View payment slip →</a>
                  @if (order.payment!.note) {
                    <p class="text-white/40 text-xs mt-1">Note: {{ order.payment!.note }}</p>
                  }
                </div>
              }

              <div class="flex gap-3">
                <button (click)="approve(order.id)" [disabled]="processing() === order.id"
                  class="bg-green-600 hover:bg-green-500 disabled:opacity-50 text-white px-5 py-2 rounded-lg text-sm transition-colors font-medium">
                  {{ processing() === order.id ? 'Processing...' : 'Approve' }}
                </button>
                <div class="flex gap-2 flex-1">
                  <input [(ngModel)]="rejectReasons[order.id]" placeholder="Rejection reason"
                    class="flex-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-white placeholder:text-white/30 focus:outline-none focus:border-red-500" />
                  <button (click)="reject(order.id)" [disabled]="!rejectReasons[order.id] || processing() === order.id"
                    class="bg-red-600 hover:bg-red-500 disabled:opacity-50 text-white px-5 py-2 rounded-lg text-sm transition-colors font-medium">
                    Reject
                  </button>
                </div>
              </div>
            </div>
          }
        </div>
      }
    </div>
  `,
})
export class PendingOrdersComponent implements OnInit {
  private payment = inject(PaymentService);

  orders = signal<PendingOrderDto[]>([]);
  loading = signal(true);
  processing = signal('');
  rejectReasons: Record<string, string> = {};

  async ngOnInit() {
    await this.load();
  }

  async load() {
    this.loading.set(true);
    try {
      const res = await this.payment.getPendingOrders();
      this.orders.set(res.data ?? []);
    } finally {
      this.loading.set(false);
    }
  }

  async approve(orderId: string) {
    this.processing.set(orderId);
    try {
      await this.payment.approvePayment(orderId);
      await this.load();
    } finally {
      this.processing.set('');
    }
  }

  async reject(orderId: string) {
    const reason = this.rejectReasons[orderId];
    if (!reason) return;
    this.processing.set(orderId);
    try {
      await this.payment.rejectPayment(orderId, reason);
      await this.load();
    } finally {
      this.processing.set('');
    }
  }
}
