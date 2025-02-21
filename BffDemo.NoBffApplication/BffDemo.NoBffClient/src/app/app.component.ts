import { Component } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';
import { RouterOutlet } from '@angular/router';
import {NgIf} from '@angular/common';

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
  imports: [RouterOutlet, NgIf],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'BffDemo.NoBffClient';
  jwtToken: string = '';

  constructor(private readonly oauthService: OAuthService) {
    this.oauthService.configure(authConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      if (this.oauthService.hasValidAccessToken()) {
        this.jwtToken = this.oauthService.getAccessToken();
        console.log('JWT token stored:', this.jwtToken);
      }
    });
  }

  login(): void {
    this.oauthService.initCodeFlow();
  }

  logout(): void {
    this.oauthService.logOut();
    this.jwtToken = '';
  }
}
