import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  username = '';
  password = '';
  confirm_password = '';
  message: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  register() {
    this.authService.register(this.username, this.password, this.confirm_password).subscribe({
      next: (response) => {
        console.log(response);
        this.message = response.message;
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.message = err.error?.message
      }
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
