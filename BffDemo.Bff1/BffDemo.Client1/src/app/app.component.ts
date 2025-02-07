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
  user: any;
  data: any;
  constructor(private readonly http: HttpClient) {}
  ngOnInit(): void {
    this.getUserInfo().subscribe({
      next: (user) => {
        this.user = user;
      },
      error: (err) => {
        console.error('Not logged in or error fetching user:', err);
        this.user = null;
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

  getUserInfo() {
    return this.http.get('https://localhost:5001/bff/user', { withCredentials: true })
  }
  getData() {
    const headers = new HttpHeaders({
      'X-CSRF': '1'
    });
    return this.http.get('https://localhost:5001/api1/email', {
      withCredentials: true,
      headers: headers
    });
  }
  onLogin(e: any) {
    window.location.href = 'https://localhost:5001/bff/login';
  };

  onLogout(e: any) {
    window.location.href = 'https://localhost:5001/bff/logout';
  }
}
