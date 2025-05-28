import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { LoginModel } from '../models/login.model';
import { MatProgressBar, MatProgressBarModule } from '@angular/material/progress-bar';
import { AuthorizeService } from '../service/authorize.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenModel } from '../models/token.model';
import { Response } from '../../../core/models/response.model';
import { LocalStorageService } from '../service/storage.service';
import { jwtDecode } from 'jwt-decode';
import { MatButtonModule } from '@angular/material/button';

import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';
import { HttpStatusCode } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { TwoFactorComponent } from '../two-factor/two-factor.component';

@Component({
  standalone: true,
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatProgressBarModule,
    MatButtonModule
  ]
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  @ViewChild(MatProgressBar) progressBar!: MatProgressBar;
  errorMessage: string | null = null;
  redirectToUrl: any;

  constructor(
    private fb: FormBuilder,
    private authorizeService: AuthorizeService,
    private localStorageService: LocalStorageService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.localStorageService.clear();

    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.activatedRoute.queryParams.subscribe((params: any) => {
      this.redirectToUrl = params.redirectTo;
      if (params.callFromIbs) {
        this.localStorageService.set('callFromIbs', params.callFromIbs);
      }

      if (params.token) {
        let isSaved: boolean = this.saveDataToLocalStorage(params.token);
        if (isSaved && this.redirectToUrl) {
          this.redirectToPage();
        }
      }
    });
  }

  onLogin(): void {
    if (this.loginForm.valid) {
      const loginModel: LoginModel = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password
      };

      //this.progressBar.mode = 'indeterminate';

      this.authorizeService.login(loginModel).then((response: Response<TokenModel>) => {
        if (response.statusCode == HttpStatusCode.Ok) {
          console.log(response);
          if (response.content.twoFAEnabled) {
            this.open2FADialog(loginModel.username);
          }
          else {
            this.errorMessage = '';
            this.router.navigate(['home']);
            this.progressBar.mode = 'determinate';
          }
        }
      }).catch((response: any) => {
        switch (response.status) {
          case 400:
            alert('Incorrect credentials. Please try again');
            this.errorMessage = 'Incorrect credentials. Please try again';
            break;
          case 401:
            alert('User is not active please contact customer support!');
            break;
          default:
            this.errorMessage = 'Server error. Please try again later';
            break;
        }
        this.progressBar.mode = 'determinate';
      });
    }
  }
  open2FADialog(username: string): void {
    const dialogRef = this.dialog.open(TwoFactorComponent, {
      width: '400px',
      data: { username: username }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success && result.token) {
          this.router.navigate(['/home']);
      } else {
        this.errorMessage = '2FA failed. Please try again.';
      }
    });
  }
  saveDataToLocalStorage(token: any): boolean {
    let result: boolean = false;
    if (token) {
      try {
        const jwtDecoded: any = jwtDecode(token);
        const username: any = jwtDecoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];

        if (!username) {
          this.errorMessage = 'Invalid token';
          return false;
        }

        this.localStorageService.set('token', token);
        this.localStorageService.set('username', username);
        //this.localStorageService.set('role', role);

        result = true;
      } catch (e) {
        this.errorMessage = 'Invalid credentials. Please, try again';
      }
    } else {
      this.errorMessage = 'Invalid credentials. Please, try again';
    }
    return result;
  }

  redirectToPage() {
    if (this.redirectToUrl) {
      this.router.navigate([this.redirectToUrl]);
    } else {
      this.router.navigate(['home']);
    }
  }
}
