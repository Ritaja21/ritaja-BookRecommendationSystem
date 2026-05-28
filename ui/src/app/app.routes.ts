import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard.component';
import { CustomerDasboardComponent } from './features/customer/customer-dasboard/customer-dasboard.component';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';
import { customerGuard } from './core/guards/customer.guard';
import { AdminLayoutComponent } from './features/admin/admin-layout/admin-layout.component';
import { CustomerLayoutComponent } from './features/customer/customer-layout/customer-layout.component';

export const routes: Routes = [{
    path: "",
    component: HomeComponent
},
{
    path: "login",
    component: LoginComponent
},
{
    path: "register",
    component: RegisterComponent
},
{
    path: "admin",
    component: AdminLayoutComponent,
    canActivate: [authGuard, adminGuard],
    children: [
        { path: "dashboard", component: AdminDashboardComponent }
    ]
},
{
    path: "customer",
    component: CustomerLayoutComponent,
    canActivate: [authGuard, customerGuard],
    children: [
        { path: "dashboard", component: CustomerDasboardComponent }
    ]
}];
