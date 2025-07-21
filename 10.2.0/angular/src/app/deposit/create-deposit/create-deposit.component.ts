import { Component, Injector, OnInit, EventEmitter, Output, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { DropdownModule } from 'primeng/dropdown';
import { DatePickerModule } from 'primeng/datepicker';
import { PatientDropDownDto, PatientServiceProxy, PaymentMethod, BillingMethod, DepositServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-create-deposit',
  standalone: true,
  imports: [FormsModule, CommonModule, SelectModule, DropdownModule, DatePickerModule, AbpModalHeaderComponent, AbpModalFooterComponent],
  templateUrl: './create-deposit.component.html',
  styleUrl: './create-deposit.component.css',
  providers: [PatientServiceProxy, DepositServiceProxy]
})
export class CreateDepositComponent extends AppComponentBase implements OnInit {
  @ViewChild('createDepositForm', { static: true }) createDepositForm: NgForm;
  @Output() onSave = new EventEmitter<any>();
  patients: PatientDropDownDto[] = [];
  paymentMethodOptions = [
    { label: 'Cash', value: PaymentMethod._0 },
    { label: 'Card', value: PaymentMethod._1 }
  ];
  billingMethodOptions = [
    { label: 'Insurance Only', value: BillingMethod._0 },
    { label: 'Self Pay', value: BillingMethod._1 },
    { label: 'Insurance + SelfPay', value: BillingMethod._2 }
  ];
  deposit: any = {
    id: 0,
    tenantId: this.appSession.tenantId,
    patientId: null,
    amount: null,
    paymentMethod: null,
    billingMethod: null,
    depositDateTime: null
  };
  saving = false;

  constructor(
    injector: Injector,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    public bsModalRef: BsModalRef,
    private _depositService: DepositServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this._patientService.patientDropDown().subscribe({
      next: (res) => (this.patients = res),
      error: () => {}
    });
  }

  get isFormValid(): boolean {
    return this.createDepositForm?.form?.valid;
  }

  save(): void {
    if (!this.isFormValid) {
      if (this.notify && typeof this.notify.warn === 'function') {
        this.notify.warn('Please complete the form properly.');
      } else {
        alert('Please complete the form properly.');
      }
      return;
    }
    this.saving = true;
    this._depositService.createDepositWithStripeSupport(this.deposit).subscribe({
      next: (res) => {
        if (res.stripeRedirectUrl) {
          this.notify.info('Redirecting to Stripe...');
          this.bsModalRef.hide();
          window.location.href = res.stripeRedirectUrl;
        } else {
          this.notify.info('Deposit created successfully');
          this.bsModalRef.hide();
          this.onSave.emit();
        }
      },
      error: (err) => {
        this.notify.error(err?.error?.message || 'Failed to create deposit');
        this.saving = false;
      }
    });
    
  }
}
