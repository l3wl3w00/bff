import { Component } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';
import { RouterOutlet } from '@angular/router';
import {JsonPipe, NgIf} from '@angular/common';

const authConfig: AuthConfig = {
  issuer: 'https://localhost:5000',
  redirectUri: window.location.origin,
  clientId: 'no_bff',
  responseType: 'code',
  scope: 'no_bff'
};

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgIf, RouterOutlet, JsonPipe],
  template: `
    <main class="main">
      <div class="content">
        <div style="text-align: center; margin-top: 2rem;">
          <h1>{{ title }}</h1>
          <div *ngIf="jwtToken">
            <p style="text-align: center;">You’re logged in</p>
            <h2>Decoded JWT:</h2>
            <pre style="text-align: left;"><code>{{ decodedToken | json }}</code></pre>
            <div style="text-align: center;">
              <button (click)="logout()">Logout</button>
            </div>
          </div>
          <div *ngIf="jwtToken === null" style="text-align: center;">
            <p>Loading...</p>
          </div>
          <div *ngIf="jwtToken === ''" style="text-align: center;">
            <p>You ain’t logged in yet, lazy fuck.</p>
            <button (click)="login()">Login</button>
          </div>
        </div>
      </div>
    </main>
    <router-outlet/>
  `,
})
export class AppComponent {
  title = 'BffDemo.NoBffClient';
  jwtToken: string | null = null;
  decodedToken: any = {};

  constructor(
    private readonly oauthService: OAuthService
  ) {

    this.oauthService.configure(authConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      if (this.oauthService.hasValidAccessToken()) {
        this.jwtToken = this.oauthService.getAccessToken();
        this.decodedToken = this.decodeJwt(this.jwtToken);
        console.log('JWT token stored:', this.jwtToken);
      }
    });
  }

  decodeJwt(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch (error) {
      console.error('Error decoding token:', error);
      return {};
    }
  }

  login(): void {
    this.oauthService.initCodeFlow();
  }

  logout(): void {
    this.oauthService.logOut();
    this.jwtToken = '';
    this.decodedToken = {};
  }
}
