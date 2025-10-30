import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';

export const routes: Routes = [
    {
        path: 'Home', component: HomeComponent
    },
    {
        path: '',redirectTo: 'Home', pathMatch: 'full'
    }
];
