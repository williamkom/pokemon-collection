import {Component,signal} from '@angular/core';
import {FormBuilder,ReactiveFormsModule,Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {finalize} from 'rxjs';
import {AuthService} from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
/** * The LoginComponent is responsible for handling user authentication.
 * It provides a form for users to enter their email and password, and manages the login process.
 */
export class LoginComponent {
 // Signals to manage the loading state and error messages during the login process.
  readonly loading = signal(false);
  // Signal to hold any error message that may occur during the login process.
  readonly errorMessage = signal('');
  readonly form;
  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    // Initialize the form with email and password fields, along with their respective validators
    this.form =
      this.formBuilder.nonNullable.group({
        email: [
          '',
          [
            Validators.required,
            Validators.email
          ]
        ],
        password: [
          '',
          [
            Validators.required
          ]
        ]
      });
  }

// Submits the login form, performs validation, and handles the login process.
  submit(): void {
    if (
      this.form.invalid ||
      this.loading()
    ) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    this.errorMessage.set('');
    this.authService
      .login(
        this.form.getRawValue()
      )
      .pipe(
        finalize(() => {
          this.loading.set(false);
        })
      )
      .subscribe({
        next: () => {
          this.router.navigate([
            '/home'
          ]);
        },
        error: error => {
          console.error(
            'Login failed:',
            error
          );
          if (error.status === 401) {

            this.errorMessage.set(
              'Invalid email or password.'
            );

            return;
          }
          this.errorMessage.set(
            'Server is currently unavailable.'
          );
        }
      });
  }
}