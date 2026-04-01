import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/catalog/product-list/product-list.component').then(m => m.ProductListComponent) },
  { path: 'products/:id', loadComponent: () => import('./features/catalog/product-detail/product-detail.component').then(m => m.ProductDetailComponent) },
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
  {
    path: 'library',
    canActivate: [authGuard],
    children: [
      { path: '', loadComponent: () => import('./features/library/library-list/library-list.component').then(m => m.LibraryListComponent) },
      { path: 'ebook/:productId', loadComponent: () => import('./features/library/ebook-reader/ebook-reader.component').then(m => m.EbookReaderComponent) },
      { path: 'tarot/:productId', loadComponent: () => import('./features/library/tarot-session/tarot-session.component').then(m => m.TarotSessionComponent) },
    ]
  },
  { path: 'checkout/:orderId', canActivate: [authGuard], loadComponent: () => import('./features/store/checkout/checkout.component').then(m => m.CheckoutComponent) },
  {
    path: 'admin',
    canActivate: [authGuard, adminGuard],
    children: [
      { path: '', loadComponent: () => import('./features/admin/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'upload', loadComponent: () => import('./features/admin/upload-product/upload-product.component').then(m => m.UploadProductComponent) },
      { path: 'orders', loadComponent: () => import('./features/admin/pending-orders/pending-orders.component').then(m => m.PendingOrdersComponent) },
    ]
  },
  { path: '**', redirectTo: '' },
];
