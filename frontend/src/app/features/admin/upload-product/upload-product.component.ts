import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-upload-product',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="max-w-2xl mx-auto px-4 py-10">
      <h1 class="text-2xl font-bold mb-2">Upload product</h1>

      <!-- Tabs -->
      <div class="flex gap-2 mb-8">
        <button (click)="tab.set('ebook')"
          [class]="tab() === 'ebook' ? 'bg-violet-600 text-white px-4 py-1.5 rounded-full text-sm' : 'bg-white/5 text-white/60 px-4 py-1.5 rounded-full text-sm'">
          Ebook
        </button>
        <button (click)="tab.set('tarot')"
          [class]="tab() === 'tarot' ? 'bg-violet-600 text-white px-4 py-1.5 rounded-full text-sm' : 'bg-white/5 text-white/60 px-4 py-1.5 rounded-full text-sm'">
          Tarot Deck
        </button>
      </div>

      @if (success()) {
        <div class="bg-green-500/10 border border-green-500/30 text-green-400 rounded-lg px-4 py-3 text-sm mb-4">
          Uploaded successfully! Product ID: {{ success() }}
        </div>
      }
      @if (error()) {
        <div class="bg-red-500/10 border border-red-500/30 text-red-400 rounded-lg px-4 py-3 text-sm mb-4">
          {{ error() }}
        </div>
      }

      @if (tab() === 'ebook') {
        <div class="space-y-4">
          <div><label class="block text-sm text-white/60 mb-1">Title</label>
            <input [(ngModel)]="title" class="input-field" /></div>
          <div><label class="block text-sm text-white/60 mb-1">Author</label>
            <input [(ngModel)]="author" class="input-field" /></div>
          <div><label class="block text-sm text-white/60 mb-1">Description</label>
            <textarea [(ngModel)]="description" rows="3" class="input-field resize-none"></textarea></div>
          <div class="grid grid-cols-2 gap-4">
            <div><label class="block text-sm text-white/60 mb-1">Price (THB)</label>
              <input [(ngModel)]="price" type="number" class="input-field" /></div>
            <div><label class="block text-sm text-white/60 mb-1">Language</label>
              <input [(ngModel)]="language" placeholder="Thai / English" class="input-field" /></div>
          </div>
          <div><label class="block text-sm text-white/60 mb-1">Categories (comma-separated)</label>
            <input [(ngModel)]="categories" placeholder="Self-help, Spirituality" class="input-field" /></div>
          <div><label class="block text-sm text-white/60 mb-1">PDF file</label>
            <input type="file" accept=".pdf" (change)="onPdf($event)" class="file-input" /></div>
          <div><label class="block text-sm text-white/60 mb-1">Cover image (optional)</label>
            <input type="file" accept="image/*" (change)="onCover($event)" class="file-input" /></div>
          <button (click)="uploadEbook()" [disabled]="uploading()"
            class="w-full bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white py-2.5 rounded-lg transition-colors font-medium">
            {{ uploading() ? 'Uploading...' : 'Upload ebook' }}
          </button>
        </div>
      }

      @if (tab() === 'tarot') {
        <div class="space-y-4">
          <div><label class="block text-sm text-white/60 mb-1">Deck name</label>
            <input [(ngModel)]="title" class="input-field" /></div>
          <div><label class="block text-sm text-white/60 mb-1">Description</label>
            <textarea [(ngModel)]="description" rows="3" class="input-field resize-none"></textarea></div>
          <div><label class="block text-sm text-white/60 mb-1">Price (THB)</label>
            <input [(ngModel)]="price" type="number" class="input-field" /></div>
          <div><label class="block text-sm text-white/60 mb-1">Cards ZIP (naming: 01_name.webp)</label>
            <input type="file" accept=".zip" (change)="onZip($event)" class="file-input" /></div>
          <div><label class="block text-sm text-white/60 mb-1">Cover image (optional)</label>
            <input type="file" accept="image/*" (change)="onCover($event)" class="file-input" /></div>
          <div><label class="block text-sm text-white/60 mb-1">Card back image (optional)</label>
            <input type="file" accept="image/*" (change)="onBack($event)" class="file-input" /></div>
          <button (click)="uploadTarot()" [disabled]="uploading()"
            class="w-full bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white py-2.5 rounded-lg transition-colors font-medium">
            {{ uploading() ? 'Uploading...' : 'Upload tarot deck' }}
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .input-field {
      @apply w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder:text-white/30 focus:outline-none focus:border-violet-500;
    }
    .file-input {
      @apply w-full text-sm text-white/70 file:mr-3 file:bg-violet-600 file:text-white file:border-0 file:px-4 file:py-1.5 file:rounded-lg file:cursor-pointer;
    }
  `],
})
export class UploadProductComponent {
  private admin = inject(AdminService);

  tab = signal<'ebook' | 'tarot'>('ebook');
  uploading = signal(false);
  success = signal('');
  error = signal('');

  title = ''; author = ''; description = ''; language = 'Thai';
  price = 0; categories = ''; tags = '';
  pdfFile: File | null = null;
  coverFile: File | null = null;
  zipFile: File | null = null;
  backFile: File | null = null;

  onPdf(e: Event) { this.pdfFile = (e.target as HTMLInputElement).files?.[0] ?? null; }
  onCover(e: Event) { this.coverFile = (e.target as HTMLInputElement).files?.[0] ?? null; }
  onZip(e: Event) { this.zipFile = (e.target as HTMLInputElement).files?.[0] ?? null; }
  onBack(e: Event) { this.backFile = (e.target as HTMLInputElement).files?.[0] ?? null; }

  async uploadEbook() {
    if (!this.pdfFile) return;
    this.uploading.set(true); this.error.set(''); this.success.set('');
    const form = new FormData();
    form.append('title', this.title);
    form.append('author', this.author);
    form.append('description', this.description);
    form.append('priceTHB', String(this.price));
    form.append('language', this.language);
    form.append('categories', this.categories);
    form.append('previewPages', '5');
    form.append('pdf', this.pdfFile);
    if (this.coverFile) form.append('cover', this.coverFile);
    try {
      const res = await this.admin.uploadEbook(form);
      if (res.code === 'A001') this.success.set(res.data?.productId ?? 'done');
      else this.error.set(res.message ?? 'Upload failed.');
    } catch { this.error.set('Upload failed.'); }
    finally { this.uploading.set(false); }
  }

  async uploadTarot() {
    if (!this.zipFile) return;
    this.uploading.set(true); this.error.set(''); this.success.set('');
    const form = new FormData();
    form.append('name', this.title);
    form.append('description', this.description);
    form.append('priceTHB', String(this.price));
    form.append('zip', this.zipFile);
    if (this.coverFile) form.append('cover', this.coverFile);
    if (this.backFile) form.append('back', this.backFile);
    try {
      const res = await this.admin.uploadTarotDeck(form);
      if (res.code === 'A001') this.success.set(`${res.data?.productId} (${res.data?.cardCount} cards)`);
      else this.error.set(res.message ?? 'Upload failed.');
    } catch { this.error.set('Upload failed.'); }
    finally { this.uploading.set(false); }
  }
}
