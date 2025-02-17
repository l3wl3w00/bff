import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';

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
    iframe.src = 'https://localhost:5001/bff/silent-login';
    window.addEventListener("message", e => {
      if (e.origin !== "https://localhost:5001") {
        return;
      }
      if (e.data && e.data.source === 'bff-silent-login' && e.data.isLoggedIn) {
           window.location.reload();
      }
    });
  }
  getUserInfo() {
    return this.http.get('https://localhost:5001/bff/user', {
      withCredentials: true,
      headers: new HttpHeaders({
        "X-CSRF": "1",
      })
    })
  }
  getData() {
    return this.http.get('https://localhost:5001/api1/email', {
      withCredentials: true,
      headers: new HttpHeaders({
        'X-CSRF': '1'
      })
    });
  }
  onLogin(e: any) {
    window.location.href = 'https://localhost:5001/bff/login?returnUrl=http://localhost:4200';
  };

  onLogout(e: any) {
    window.location.href = `https://localhost:5001${this.getClaim("bff:logout_url")}&returnUrl=http://localhost:4200`
  }
}
