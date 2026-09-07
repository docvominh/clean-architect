import { Routes } from '@angular/router';

import { authGuard } from './user/auth.guard';
import { LoginComponent } from './user/login/login.component';
import { RegisterComponent } from './user/register/register.component';
import { AccountPageComponent } from './user/account-page.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: '', pathMatch: 'full', component: AccountPageComponent, canActivate: [authGuard] },
    { path: '**', redirectTo: '' },
];
