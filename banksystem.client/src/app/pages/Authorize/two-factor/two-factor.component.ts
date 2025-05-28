import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { AuthorizeService } from "../service/authorize.service";
import { MatDialogModule } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
  standalone:true,
  selector: 'app-two-factor',
  templateUrl: './two-factor.component.html',
  styleUrls: ['./two-factor.component.css'],
  imports: [
    MatDialogModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule

  ]
})
export class TwoFactorComponent {
  code: string = '';
  error: string | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { username: string },
    public dialogRef: MatDialogRef<TwoFactorComponent>,
    private authorizeService: AuthorizeService
  ) { }

  submit(): void {
    const model = {
      username: this.data.username,
      code: this.code
    };

    this.authorizeService.verify2FA(model).then((response) => {
      if (response.statusCode == 200) {
        this.dialogRef.close({ success: true, token: response.content.token });
      } else {
        this.error = 'Invalid code. Try again.';
      }
    }).catch(() => {
      this.error = 'Verification failed. Try later.';
    });
  }
}
