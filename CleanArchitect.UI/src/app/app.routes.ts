import { Routes } from '@angular/router';

import { authGuard } from './user/auth.guard';
import { userGuard } from './user/user-role.guard';
import { LoginComponent } from './user/login/login.component';
import { RegisterComponent } from './user/register/register.component';
import { HomeComponent } from './home/home.component';
import { adminGuard } from './admin/admin.guard';
import { ProductListComponent } from './admin/product-list/product-list.component';
import { ProductComponent } from './admin/product/product.component';
import { AdminOrderListComponent } from './admin/order-list/order-list.component';
import { CheckoutComponent } from './cart/checkout.component';
import { OrderHistoryComponent } from './order/order-history.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'admin/product', component: ProductListComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/product/new', component: ProductComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/product/:id/edit', component: ProductComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/order', component: AdminOrderListComponent, canActivate: [authGuard, adminGuard] },
    { path: 'cart', component: CheckoutComponent, canActivate: [authGuard] },
    { path: 'orders', component: OrderHistoryComponent, canActivate: [authGuard, userGuard] },
    { path: '', pathMatch: 'full', component: HomeComponent, canActivate: [authGuard] },
    { path: '**', redirectTo: '' }
];
