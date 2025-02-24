import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';
import serverLaunchSettings from '../../../Properties/launchSettings.json'
import appsettings from '../../../appsettings.json'
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ NgIf, NgIf, FormsModule, NgIf],
  template: `
    <main class="main">
      <div class="content">
        <div class="left-side">
          <h1>{{ title }}</h1>
          <iframe id="bff-silent-login" style="display: none"></iframe>
          <div *ngIf="userClaims && userClaims?.length === 0">
            <p>You are not logged in.</p>
            <button id="LoginButton" (click)="onLogin($event)">Log in</button>
          </div>
          <div *ngIf="userClaims && userClaims?.length !== 0;">
            <p>Welcome, {{ getClaim("name") }}!</p>
            <button id="LogoutButton" (click)="onLogout($event)">Log out</button>
          </div>
          <div *ngIf="!userClaims">
            <p>Loading...</p>
          </div>
          <div *ngIf="data">
            <p>{{data}}</p>
          </div>
        </div>
      </div>
    </main>
  `,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'BffDemo.Client1';
  userClaims: any | null = null;
  data: any;
  bffUrl = serverLaunchSettings.profiles.default.applicationUrl
  clientUrl = appsettings.BFF.ClientUrl

  constructor(private readonly http: HttpClient) {}
  ngOnInit(): void {
    this.triggerSilentLogin();
    this.getUserInfo().subscribe({
      next: (claims) => {
        this.userClaims = claims;
      },
      error: (err) => {
        console.error('Not logged in or error fetching user:', err);
        this.userClaims = [];
      }
    });

    this.getData().subscribe({
      next: (data) => {
        this.data = data;
      },
      error: (err) => {
        console.error('Error fetching data:', err);
        this.data = null;
      }
    });
  }
  getClaim(claimType: string): string {
    return this.userClaims.find((c: { type: string; }) => c.type === claimType).value;
  }
  triggerSilentLogin(): void {
    const iframe: any = document.querySelector('#bff-silent-login');
    iframe.src = `${this.bffUrl}/bff/silent-login`;
    window.parent.addEventListener("message", e => {
      console.log("message");

      if (e.origin !== this.bffUrl) {
        return;
      }
      if (e.data && e.data.source === 'bff-silent-login' && e.data.isLoggedIn) {
        console.log("reload");
        window.location.reload();
      }
    });
  }
  getUserInfo() {
    return this.http.get(`${this.bffUrl}/bff/user`, {
      withCredentials: true,
      headers: new HttpHeaders({
        "X-CSRF": "1",
      })
    });
  }
  getData() {
    return this.http.get(`${this.bffUrl}/api/email`, {
      withCredentials: true,
      headers: new HttpHeaders({
        "X-CSRF": "1",
      })
    });
  }
  onLogin(e: any) {
    window.location.href = `${this.bffUrl}/bff/login?returnUrl=${this.clientUrl}`;
  };

  onLogout(e: any) {
    window.location.href = `${this.bffUrl}${this.getClaim("bff:logout_url")}&returnUrl=${this.clientUrl}`
  }

  protected readonly Object = Object;
}
