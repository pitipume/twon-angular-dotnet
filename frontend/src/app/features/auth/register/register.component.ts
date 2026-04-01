import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="min-h-screen flex items-center justify-center p-4">
      <div class="w-full max-w-md bg-white/5 border border-white/10 rounded-2xl p-8">
        <h1 class="text-2xl font-bold mb-2">Create account</h1>
        @if (!otpSent()) {
          <p class="text-white/50 text-sm mb-6">We'll send a verification code to your email.</p>
          @if (error()) {
            <div class="bg-red-500/10 border border-red-500/30 text-red-400 rounded-lg px-4 py-3 text-sm mb-4">{{ error() }}</div>
          }
          <div class="space-y-4">
            <div>
              <label class="block text-sm text-white/60 mb-1">Display name</label>
              <input [(ngModel)]="displayName" type="text" placeholder="Your name"
                class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500" />
            </div>
            <div>
              <label class="block text-sm text-white/60 mb-1">Email</label>
              <input [(ngModel)]="email" type="email" placeholder="you@example.com"
                class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500" />
            </div>
            <div>
              <label class="block text-sm text-white/60 mb-1">Password</label>
              <input [(ngModel)]="password" type="password" placeholder="At least 8 characters"
                class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500" />
            </div>
            <button (click)="sendOtp()" [disabled]="loading()"
              class="w-full bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white py-2.5 rounded-lg transition-colors font-medium">
              {{ loading() ? 'Sending...' : 'Send verification code' }}
            </button>
          </div>
        } @else {
          <p class="text-white/50 text-sm mb-6">Enter the 6-digit code sent to <span class="text-white">{{ email }}</span></p>
          @if (error()) {
            <div class="bg-red-500/10 border border-red-500/30 text-red-400 rounded-lg px-4 py-3 text-sm mb-4">{{ error() }}</div>
          }
          <div class="space-y-4">
            <div>
              <label class="block text-sm text-white/60 mb-1">Verification code</label>
              <input [(ngModel)]="otp" type="text" placeholder="000000" maxlength="6"
                class="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500 text-center text-2xl tracking-[0.5em]" />
            </div>
            <button (click)="verify()" [disabled]="loading()"
              class="w-full bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white py-2.5 rounded-lg transition-colors font-medium">
              {{ loading() ? 'Verifying...' : 'Create account' }}
            </button>
            <button (click)="otpSent.set(false)" class="w-full text-white/40 hover:text-white/70 text-sm transition-colors">
              Use a different email
            </button>
          </div>
        }
        <p class="text-center text-sm text-white/50 mt-6">
          Already have an account? <a routerLink="/login" class="text-violet-400 hover:text-violet-300">Sign in</a>
        </p>
      </div>
    </div>
  `,
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  displayName = '';
  email = '';
  password = '';
  otp = '';
  loading = signal(false);
  error = signal('');
  otpSent = signal(false);

  async sendOtp() {
    if (!this.email || !this.password || !this.displayName) return;
    this.loading.set(true);
    this.error.set('');
    try {
      const res = await this.auth.initiateRegister(this.email, this.displayName, this.password);
      if (res.code === 'A001') {
        this.otpSent.set(true);
      } else {
        this.error.set(res.message ?? 'Failed to send code.');
      }
    } catch {
      this.error.set('An error occurred. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  async verify() {
    if (!this.otp) return;
    this.loading.set(true);
    this.error.set('');
    try {
      const res = await this.auth.verifyRegister(this.email, this.otp, this.displayName, this.password);
      if (res.code === 'A001') {
        this.router.navigate(['/']);
      } else {
        this.error.set(res.message ?? 'Verification failed.');
      }
    } catch {
      this.error.set('An error occurred. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }
}
