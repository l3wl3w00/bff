import {AuthConfig, OAuthService} from 'angular-oauth2-oidc';
import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {Router} from '@angular/router';

@Component({
  selector: 'signout-oidc',
  standalone: true,
  imports: [CommonModule],
  template: `
    <p>Signing out...</p>
  `,
  styleUrl: './app.component.css',
})
export class SignoutOidcComponent implements OnInit {
  constructor(private readonly oauthService: OAuthService) {}
  ngOnInit() {
    console.log('Logging out...');
    if (document.requestStorageAccess) {
      document.requestStorageAccess()
        .then(() => {
          console.log('Storage access granted');
          this.oauthService.revokeTokenAndLogout()
            .then(() => console.log('Logged out'))
            .catch(e => console.error(e));
        })
        .catch(err => console.error('Storage access denied', err));
    } else {
      console.warn('Storage Access API not supported');
      this.oauthService.revokeTokenAndLogout()
        .then(() => console.log('Logged out'))
        .catch(e => console.error(e));
    }
  }
}
