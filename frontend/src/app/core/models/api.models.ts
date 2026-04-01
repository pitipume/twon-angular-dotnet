export interface BaseResult<T> {
  code: string;
  status: 'success' | 'failure';
  message?: string;
  data?: T;
}

export interface User {
  id: string;
  email: string;
  displayName: string;
  role: 'CUSTOMER' | 'PREMIUM' | 'ADMIN' | 'SUPER_ADMIN';
}

export interface AuthResponse {
  accessToken: string;
  user: User;
}

export interface ProductDto {
  id: string;
  mongoRefId: string;
  productType: 'EBOOK' | 'TAROT_DECK';
  title: string;
  priceTHB: number;
  isPublished: boolean;
  author?: string;
  description?: string;
  coverImageUrl?: string;
  language?: string;
  categories?: string[];
  tags?: string[];
  cardCount?: number;
  totalPages?: number;
}

export interface LibraryItemDto {
  id: string;
  productId: string;
  grantedAt: string;
  product?: {
    id: string;
    productType: string;
    title: string;
    priceTHB: number;
    coverImageUrl?: string;
    author?: string;
    cardCount?: number;
  };
}

export interface EbookSession {
  signedUrl: string;
  totalPages: number;
  currentPage: number;
}

export interface TarotSession {
  cards: { cardNumber: number; name: string; imageUrl: string }[];
  backImageUrl?: string;
}

export interface OrderDto {
  id: string;
  userId: string;
  status: string;
  totalTHB: number;
  orderItems: OrderItemDto[];
  payment?: PaymentInfoDto;
  checkoutInfo?: CheckoutInfoDto;
}

export interface OrderItemDto {
  id: string;
  productId: string;
  priceTHB: number;
  product?: { title: string; productType: string };
}

export interface PaymentInfoDto {
  id: string;
  status: string;
  amountTHB: number;
  transferredAt?: string;
  note?: string;
}

export interface CheckoutInfoDto {
  bankName: string;
  accountName: string;
  accountNumber: string;
  qrImageUrl: string;
}

export interface PendingOrderDto {
  id: string;
  status: string;
  totalTHB: number;
  createdAt: string;
  user: { id: string; email: string; displayName: string };
  orderItems: { productId: string; priceTHB: number; product?: { title: string; productType: string } }[];
  payment?: { status: string; amountTHB: number; transferredAt?: string; note?: string; slipUrl?: string };
}
