import { Component, signal, inject, OnInit, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { LibraryService } from '../../../core/services/library.service';
import { gsap } from 'gsap';

interface TarotCard {
  cardNumber: number;
  name: string;
  imageUrl: string;
}

@Component({
  selector: 'app-tarot-session',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen flex flex-col items-center justify-center py-10 px-4">
      <h1 class="text-2xl font-bold mb-2">Tarot Reading</h1>
      <p class="text-white/50 text-sm mb-10">Shuffle and draw three cards.</p>

      @if (loading()) {
        <p class="text-white/30">Loading deck...</p>
      } @else {
        <!-- Deck pile -->
        @if (!drawn()) {
          <div class="relative w-40 h-56 mb-10 cursor-pointer" (click)="shuffle()">
            <div #deckRef class="relative w-full h-full">
              @for (card of deck(); track card.cardNumber; let i = $index) {
                <div class="absolute inset-0 bg-gradient-to-br from-violet-900 to-indigo-900 border border-violet-500/30 rounded-xl shadow-lg"
                  [style.transform]="'translateY(' + (-i * 0.5) + 'px)'">
                </div>
              }
            </div>
            <div class="absolute inset-0 flex items-center justify-center">
              <span class="text-white/70 text-sm font-medium">{{ shuffling() ? 'Shuffling...' : 'Tap to shuffle' }}</span>
            </div>
          </div>
          @if (!shuffling() && shuffled()) {
            <button (click)="draw()"
              class="bg-violet-600 hover:bg-violet-500 text-white px-8 py-3 rounded-xl font-medium transition-colors">
              Draw 3 cards
            </button>
          }
        }

        <!-- Drawn cards -->
        @if (drawn()) {
          <div class="flex flex-wrap justify-center gap-6">
            @for (card of drawnCards(); track card.cardNumber) {
              <div class="text-center">
                <div class="w-36 h-52 bg-gradient-to-br from-violet-900/50 to-indigo-900/50 border border-violet-500/30 rounded-xl overflow-hidden mb-3">
                  <img [src]="card.imageUrl" [alt]="card.name" class="w-full h-full object-cover" />
                </div>
                <p class="text-sm font-medium">{{ card.name }}</p>
              </div>
            }
          </div>
          <button (click)="reset()" class="mt-10 text-white/40 hover:text-white text-sm transition-colors">
            Shuffle again
          </button>
        }
      }
    </div>
  `,
})
export class TarotSessionComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private library = inject(LibraryService);

  deck = signal<TarotCard[]>([]);
  drawnCards = signal<TarotCard[]>([]);
  loading = signal(true);
  shuffling = signal(false);
  shuffled = signal(false);
  drawn = signal(false);

  @ViewChild('deckRef') deckRef!: ElementRef;

  async ngOnInit() {
    const productId = this.route.snapshot.paramMap.get('productId')!;
    try {
      const res = await this.library.getTarotSession(productId);
      if (res.code === 'A001' && res.data) {
        this.deck.set(res.data.cards);
      }
    } finally {
      this.loading.set(false);
    }
  }

  shuffle() {
    if (this.shuffling()) return;
    this.shuffling.set(true);
    this.shuffled.set(false);

    // Fisher-Yates shuffle
    const cards = [...this.deck()];
    for (let i = cards.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [cards[i], cards[j]] = [cards[j], cards[i]];
    }

    // GSAP fan animation
    const cardEls = this.deckRef?.nativeElement?.children;
    if (cardEls && cardEls.length > 0) {
      const tl = gsap.timeline({
        onComplete: () => {
          gsap.to(Array.from(cardEls), { x: 0, rotation: 0, duration: 0.4, stagger: 0.02, ease: 'power2.out' });
          this.deck.set(cards);
          this.shuffling.set(false);
          this.shuffled.set(true);
        }
      });
      Array.from(cardEls).forEach((el: any, i: number) => {
        const offset = (Math.random() - 0.5) * 120;
        tl.to(el, { x: offset, rotation: offset * 0.8, duration: 0.25, ease: 'power2.out' }, i * 0.01);
      });
    } else {
      setTimeout(() => {
        this.deck.set(cards);
        this.shuffling.set(false);
        this.shuffled.set(true);
      }, 600);
    }
  }

  draw() {
    const cards = [...this.deck()];
    const drawn = cards.sort(() => Math.random() - 0.5).slice(0, 3);
    this.drawnCards.set(drawn);
    this.drawn.set(true);
  }

  reset() {
    this.drawn.set(false);
    this.shuffled.set(false);
    this.drawnCards.set([]);
  }
}
