import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="max-w-4xl mx-auto px-4 py-10">
      <h1 class="text-3xl font-bold mb-2">Admin Dashboard</h1>
      <p class="text-white/50 mb-10">Manage products, payments, and settings.</p>
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <a routerLink="/admin/upload"
          class="bg-white/5 hover:bg-white/10 border border-white/10 rounded-xl p-6 transition-colors">
          <div class="text-2xl mb-3">📤</div>
          <h3 class="font-semibold mb-1">Upload product</h3>
          <p class="text-white/40 text-sm">Add new ebook or tarot deck</p>
        </a>
        <a routerLink="/admin/orders"
          class="bg-white/5 hover:bg-white/10 border border-white/10 rounded-xl p-6 transition-colors">
          <div class="text-2xl mb-3">💳</div>
          <h3 class="font-semibold mb-1">Pending orders</h3>
          <p class="text-white/40 text-sm">Review and approve payments</p>
        </a>
        <a routerLink="/"
          class="bg-white/5 hover:bg-white/10 border border-white/10 rounded-xl p-6 transition-colors">
          <div class="text-2xl mb-3">🛍️</div>
          <h3 class="font-semibold mb-1">View shop</h3>
          <p class="text-white/40 text-sm">See the public catalog</p>
        </a>
      </div>
    </div>
  `,
})
export class DashboardComponent {}
