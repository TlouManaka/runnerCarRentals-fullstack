import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';

export const routes: Routes = [
    {
        path: 'Home', component: HomeComponent
        
    },
    {
        path: '',redirectTo: 'Home', pathMatch: 'full'
    },
    {
        path: 'Login', component: LoginComponent
    }
];
