import {AuthConfig, OAuthService} from 'angular-oauth2-oidc';
import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';

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
    this.oauthService.revokeTokenAndLogout()
      .then(r => window.location.href = '/main-page')
      .catch(err => console.error(err));
    // const endSessionUrl =
    //   this.oauthService.logoutUrl +
    //   '?id_token_hint=' + this.oauthService.getIdToken() +
    //   '&post_logout_redirect_uri=' + encodeURIComponent(this.oauthService.postLogoutRedirectUri!);
    // window.location.href = endSessionUrl;
  }
}
