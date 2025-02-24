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
  constructor(private readonly oauthService: OAuthService, private readonly router: Router) {}
  ngOnInit() {
    this.oauthService.revokeTokenAndLogout()
      .then(r => this.router.navigate(['/main-page']))
      .catch(err => console.error(err));
  }
}
