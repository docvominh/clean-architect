import { Routes } from '@angular/router';

import { authGuard } from './user/auth.guard';
import { LoginComponent } from './user/login/login.component';
import { RegisterComponent } from './user/register/register.component';
import { AccountPageComponent } from './user/account-page.component';
import { adminGuard } from './admin/admin.guard';
import { ProductListComponent } from './admin/product-list/product-list.component';
import { ProductComponent } from './admin/product/product.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'admin/product', component: ProductListComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/product/new', component: ProductComponent, canActivate: [authGuard, adminGuard] },
    { path: 'admin/product/:id/edit', component: ProductComponent, canActivate: [authGuard, adminGuard] },
    { path: '', pathMatch: 'full', component: AccountPageComponent, canActivate: [authGuard] },
    { path: '**', redirectTo: '' },
];
