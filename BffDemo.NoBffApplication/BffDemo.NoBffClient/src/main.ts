import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import {provideOAuthClient} from 'angular-oauth2-oidc';
import {provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {provideZoneChangeDetection} from '@angular/core';
import {provideRouter} from '@angular/router';
import {routes} from './app/app.routes';

bootstrapApplication(AppComponent, {
  providers: [
    provideOAuthClient(),
    provideHttpClient(withInterceptorsFromDi()),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes)]
}).catch((err) => console.error(err));
