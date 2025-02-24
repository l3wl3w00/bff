import {Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {AuthConfig, OAuthService} from 'angular-oauth2-oidc';

const authConfig: AuthConfig = {
  issuer: 'https://localhost:5000',
  redirectUri: window.location.origin + '/main-page',
  clientId: 'no_bff',
  responseType: 'code',
  dummyClientSecret: 'no-bff-secret',
  scope: 'openid profile no_bff',
  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',
  useSilentRefresh: true,
  sessionChecksEnabled: true,
  logoutUrl: 'https://localhost:5000/connect/endsession',
  postLogoutRedirectUri: window.location.origin + '/main-page',
  oidc: true,
};

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <h1>{{ title }}</h1>
    <router-outlet/>
  `,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  title = 'BffDemo.NoBffClient';
  constructor(private readonly oauthService: OAuthService) {}
  ngOnInit(): void {
    this.oauthService.configure(authConfig);
    this.oauthService.setStorage(localStorage);
    this.oauthService.setupAutomaticSilentRefresh()
  }
}
