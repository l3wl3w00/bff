﻿import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private readonly http: HttpClient) {}

  login() {
    // Redirect to the BFF login endpoint
    window.location.href = 'https://bff2.localhost:5002/bff/login';
  }

  logout() {
    // Redirect to the BFF logout endpoint
    window.location.href = 'https://bff2.localhost:5002/bff/logout';
  }

  getUser() {
    // Query a BFF endpoint that returns the current user info.
    // The BFF can read the cookie and return user details.
    return this.http.get('https://bff2.localhost:5002/bff/user');
  }
}
