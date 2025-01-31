import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5170/auth';

  constructor(private http: HttpClient, private router: Router) {}

  register(username: string, password: string, confirm_password: string): Observable<any> {
    console.log(username, password, confirm_password);
    return this.http.post(`${this.apiUrl}/register`, { 
      email: username, 
      password: password, 
      ConfirmPassword: confirm_password 
    });
}
  
  login(username: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, { username, password });
  }

  saveToken(token: string) {
    localStorage.setItem('jwt_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  logout() {
    localStorage.removeItem('jwt_token');
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}


