import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="min-h-screen flex items-center justify-center p-4">
      <div class="w-full max-w-md bg-white/5 border border-white/10 rounded-2xl p-8">
        <h1 class="text-2xl font-bold mb-6">Sign in to Twon</h1>
        @if (error()) {
          <div class="bg-red-500/10 border border-red-500/30 text-red-400 rounded-lg px-4 py-3 text-sm mb-4">
            {{ error() }}
          </div>
        }
        <div class="space-y-4">
          <div>
            <label class="block text-sm text-white/60 mb-1">Email</label>
            <input [(ngModel)]="email" type="email" placeholder="you@example.com"
              class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500" />
          </div>
          <div>
            <label class="block text-sm text-white/60 mb-1">Password</label>
            <input [(ngModel)]="password" type="password" placeholder="••••••••"
              class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500" />
          </div>
          <button (click)="submit()" [disabled]="loading()"
            class="w-full bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white py-2.5 rounded-lg transition-colors font-medium">
            {{ loading() ? 'Signing in...' : 'Sign in' }}
          </button>
        </div>
        <p class="text-center text-sm text-white/50 mt-6">
          Don't have an account? <a routerLink="/register" class="text-violet-400 hover:text-violet-300">Sign up</a>
        </p>
      </div>
    </div>
  `,
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  email = '';
  password = '';
  loading = signal(false);
  error = signal('');

  async submit() {
    if (!this.email || !this.password) return;
    this.loading.set(true);
    this.error.set('');
    try {
      const res = await this.auth.login(this.email, this.password);
      if (res.code === 'A001') {
        this.router.navigate(['/']);
      } else {
        this.error.set(res.message ?? 'Login failed.');
      }
    } catch {
      this.error.set('An error occurred. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }
}
