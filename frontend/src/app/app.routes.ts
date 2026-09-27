import {Routes} from '@angular/router';

import {authGuard} from './core/auth/auth.guard';


export const routes: Routes = [

  {path: '',pathMatch: 'full',redirectTo: 'home'},
  {path: 'login',loadComponent: () =>import('./features/login/login.component').then(module => module.LoginComponent)},
  {path: 'home',canActivate: [authGuard],loadComponent: () =>import('./features/home/home.component').then(module =>module.HomeComponent)},
  {path: '**',redirectTo: 'home'}

];