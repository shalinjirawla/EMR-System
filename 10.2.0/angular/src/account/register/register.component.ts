import { Component, Injector, Input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/app-component-base';
import { AccountServiceProxy, RegisterInput, RegisterOutput } from '@shared/service-proxies/service-proxies';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { FormsModule } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../shared/components/validation/abp-validation.summary.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

@Component({
    templateUrl: './register.component.html',
    animations: [accountModuleAnimation()],
    standalone: true,
    imports: [FormsModule, AbpValidationSummaryComponent, RouterLink, LocalizePipe],
})
export class RegisterComponent extends AppComponentBase {
    model: RegisterInput = new RegisterInput();
    saving = false;

    constructor(
        injector: Injector,
        private _accountService: AccountServiceProxy,
        private _router: Router,
        private authService: AppAuthService
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;
        this._accountService
            .register(this.model)
            .pipe(
                finalize(() => {
                    this.saving = false;
                })
            )
            .subscribe((result: RegisterOutput) => {
                if (!result.canLogin) {
                    this.notify.success(this.l('SuccessfullyRegistered'));
                    this._router.navigate(['/login']);
                    return;
                }

                // Autheticate
                this.saving = true;
                this.authService.authenticateModel.userNameOrEmailAddress = this.model.userName;
                this.authService.authenticateModel.password = this.model.password;
                this.authService.authenticate(() => {
                    this.saving = false;
                });
            });
    }
}
