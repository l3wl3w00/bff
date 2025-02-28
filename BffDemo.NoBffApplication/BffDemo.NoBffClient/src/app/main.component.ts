import { Component } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';
import {JsonPipe, NgIf} from '@angular/common';

@Component({
  selector: 'main-page',
  standalone: true,
  imports: [NgIf, JsonPipe],
  template: `
    <main class="main">
      <div class="content">
        <div class="left-side">
          <div style="text-align: center; margin-top: 2rem;">
            <div *ngIf="jwtToken">
              <p style="text-align: center;">You’re logged in</p>
              <h2>Decoded JWT:</h2>
              <pre style="text-align: left;"><code>{{ decodedToken | json }}</code></pre>
              <div style="text-align: center;">
                <button (click)="logout()">Log out</button>
              </div>
            </div>
            <div *ngIf="jwtToken === null" style="text-align: center;">
              <p>Loading...</p>
            </div>
            <div *ngIf="jwtToken === ''" style="text-align: center;">
              <p>You aren’t logged in</p>
              <button (click)="login()">Log in</button>
            </div>
          </div>
        </div>
      </div>
    </main>
  `,
  styleUrl: './app.component.css'
})
export class MainPageComponent {
  jwtToken: string | null = null;
  decodedToken: any = {};

  constructor(private readonly oauthService: OAuthService) { }

  ngOnInit(): void {
    this.initializeAuthentication();
  }

  private initializeAuthentication(): void {
    this.oauthService.loadDiscoveryDocumentAndTryLogin()
      .then(_ => {
        console.log('Discovery document loaded');
      })
      .catch(err => {
        console.error('Discovery document loading failed:', err);
      });
    // removed silent login temporarily to be able to demonstrate a bug with global logout

    // this.oauthService.loadDiscoveryDocumentAndTryLogin()
    //   .then(loginResult => {
    //     if (this.oauthService.hasValidAccessToken()) {
    //       this.jwtToken = this.oauthService.getAccessToken();
    //       this.decodedToken = this.decodeJwt(this.jwtToken);
    //       console.log('JWT token stored:', this.jwtToken);
    //     } else {
    //       this.jwtToken = '';
    //       console.log('Invalid access token:', this.oauthService.getAccessToken(), loginResult);
    //     }
    //
    //     // Attempt silent refresh
    //     return this.oauthService.silentRefresh();
    //   })
    //   .then(refreshResult => {
    //     console.log('Silent refresh succeeded:', refreshResult);
    //     if (this.oauthService.hasValidAccessToken()) {
    //       this.jwtToken = this.oauthService.getAccessToken();
    //       this.decodedToken = this.decodeJwt(this.jwtToken);
    //       console.log('JWT token stored:', this.jwtToken);
    //     } else {
    //       this.jwtToken = '';
    //       // sessionStorage.clear();
    //       console.log('Invalid access token after refresh:', this.oauthService.getAccessToken());
    //     }
    //   })
    //   .catch(err => {
    //     console.error('Silent refresh failed:', err);
    //     this.jwtToken = '';
    //     // sessionStorage.clear();
    //   });
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
    this.oauthService.revokeTokenAndLogout().then(_ => {
      this.jwtToken = '';
      this.decodedToken = {};
    });
  }

  handleGlobalLogout(): void {
    if (this.oauthService.hasValidAccessToken()) {
      this.oauthService.logOut();
      this.jwtToken = '';
      this.decodedToken = {};
    }
  }
}
