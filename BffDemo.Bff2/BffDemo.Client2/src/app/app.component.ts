import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {AuthService} from '../auth-service';
import {NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';

interface UserInfo {
  name: string;
  claims: { type: string; value: string }[];
}
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgIf, NgIf, FormsModule, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'BffDemo.Client2';
  user: UserInfo | null = null;
  data: any;
  constructor(private readonly authService: AuthService, private readonly http: HttpClient) {}
  ngOnInit(): void {
    this.getUserInfo();
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
    this.http.get<UserInfo>('https://bff2.localhost:5002/bff/user', { withCredentials: true })
      .subscribe({
        next: (user) => {
          this.user = user;
        },
        error: (err) => {
          console.error('Not logged in or error fetching user:', err);
          this.user = null;
        }
      });
  }
  getData() {
    const headers = new HttpHeaders({
      'X-CSRF': '1'
    });
    return this.http.get('https://bff2.localhost:5002/api2/weatherforecast', {
      withCredentials: true,
      headers: headers
    });
  }
  onLogin(e: any) {
    this.authService.login()
  };

  onLogout(e: any) {
    this.authService.logout();
  }
}
