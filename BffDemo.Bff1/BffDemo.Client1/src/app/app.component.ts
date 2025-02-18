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
  imports: [RouterOutlet, NgIf, NgIf, FormsModule, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'BffDemo.Client1';
  userClaims: any;
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
        this.userClaims = null;
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
    window.addEventListener("message", e => {
      if (e.origin !== this.bffUrl) {
        return;
      }
      if (e.data && e.data.source === 'bff-silent-login' && e.data.isLoggedIn) {
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
}
