import { Routes } from '@angular/router';
import {SignoutOidcComponent} from "./signout-oidc-component";
import {MainPageComponent} from "./main.component";

export const routes: Routes = [
    { path: '', redirectTo: '/main-page', pathMatch: 'full' },
    { path: 'main-page', component: MainPageComponent },
    { path: 'signout-oidc', component: SignoutOidcComponent }
];
