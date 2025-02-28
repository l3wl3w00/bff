import { OAuthService} from 'angular-oauth2-oidc';
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
    console.log('Signing out...');
    this.oauthService.logOut(true);
    console.log('Signed out.');
  }
}
