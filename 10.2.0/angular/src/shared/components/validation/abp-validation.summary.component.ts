import { Component, Input, Injector, Renderer2, ElementRef, OnInit } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpValidationError } from './abp-validation.api';

@Component({
    selector: 'abp-validation-summary',
    templateUrl: './abp-validation.summary.component.html',
    standalone: true,
})
export class AbpValidationSummaryComponent extends AppComponentBase implements OnInit {
    @Input() control: AbstractControl;
    @Input() controlEl: ElementRef;

    defaultValidationErrors: Partial<AbpValidationError>[] = [
        { name: 'required', localizationKey: 'ThisFieldIsRequired' },
        {
            name: 'minlength',
            localizationKey: 'PleaseEnterAtLeastNCharacter',
            propertyKey: 'requiredLength',
        },
        {
            name: 'maxlength',
            localizationKey: 'PleaseEnterNoMoreThanNCharacter',
            propertyKey: 'requiredLength',
        },
        {
            name: 'email',
            localizationKey: 'InvalidEmailAddress',
        },
        {
            name: 'pattern',
            localizationKey: 'InvalidPattern',
            propertyKey: 'requiredPattern',
        },
        {
            name: 'validateEqual',
            localizationKey: 'PairsDoNotMatch',
        },
    ];
    validationErrors = <AbpValidationError[]>this.defaultValidationErrors;

    constructor(
        injector: Injector,
        public _renderer: Renderer2
    ) {
        super(injector);
    }

    @Input() set customValidationErrors(val: AbpValidationError[]) {
        if (val && val.length > 0) {
            const defaults = this.defaultValidationErrors.filter(
                (defaultValidationError) =>
                    !val.find((customValidationError) => customValidationError.name === defaultValidationError.name)
            );
            this.validationErrors = <AbpValidationError[]>[...defaults, ...val];
        }
    }

    ngOnInit() {
 if (this.control && this.controlEl?.nativeElement) {
        this.control.valueChanges.subscribe(() => {
            if (this.control.valid && (this.control.dirty || this.control.touched)) {
                this._renderer.removeClass(this.controlEl.nativeElement, 'is-invalid');
            }
        });
    }
    }

   getValidationErrorMessage(error: AbpValidationError): string {
    if (this.controlEl?.nativeElement) {
        this._renderer.addClass(this.controlEl.nativeElement, 'is-invalid');
    }

    const errorDetails = this.control.errors?.[error.name];
    const propertyValue = errorDetails?.[error.propertyKey];

    return propertyValue ? this.l(error.localizationKey, propertyValue) : this.l(error.localizationKey);
}

}
