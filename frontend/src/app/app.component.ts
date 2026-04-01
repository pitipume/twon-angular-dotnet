import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  template: `
    <nav class="flex items-center justify-between px-6 py-4 border-b border-white/10 bg-black/30 backdrop-blur sticky top-0 z-50">
      <a routerLink="/" class="text-xl font-bold tracking-tight text-white">Twon</a>
      <div class="flex items-center gap-4 text-sm">
        <a routerLink="/" class="text-white/70 hover:text-white transition-colors">Shop</a>
        @if (auth.isLoggedIn()) {
          <a routerLink="/library" class="text-white/70 hover:text-white transition-colors">Library</a>
          @if (auth.isAdmin()) {
            <a routerLink="/admin" class="text-white/70 hover:text-white transition-colors">Admin</a>
          }
          <button (click)="auth.logout()" class="text-white/50 hover:text-white transition-colors">Logout</button>
        } @else {
          <a routerLink="/login" class="text-white/70 hover:text-white transition-colors">Login</a>
          <a routerLink="/register" class="bg-violet-600 hover:bg-violet-500 text-white px-4 py-1.5 rounded-lg transition-colors">Sign up</a>
        }
      </div>
    </nav>
    <main>
      <router-outlet />
    </main>
  `,
})
export class AppComponent {
  auth = inject(AuthService);
}
