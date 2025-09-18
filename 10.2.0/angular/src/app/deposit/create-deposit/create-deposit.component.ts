import { Component, Injector, OnInit, EventEmitter, Output, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { DropdownModule } from 'primeng/dropdown';
import { DatePickerModule } from 'primeng/datepicker';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateUpdateDepositTransactionDto, DepositTransactionServiceProxy, PaymentMethod } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-deposit',
  standalone: true,
  imports: [FormsModule, CommonModule, SelectModule, DropdownModule, DatePickerModule, AbpModalHeaderComponent, AbpModalFooterComponent],
  templateUrl: './create-deposit.component.html',
  styleUrl: './create-deposit.component.css',
  providers: [DepositTransactionServiceProxy]
})
export class CreateDepositComponent extends AppComponentBase implements OnInit {
  @ViewChild('createDepositForm', { static: true }) createDepositForm: NgForm;
  @Output() onSave = new EventEmitter<any>();

  saving = false;
  deposit: any = {
    tenantId: abp.session.tenantId,
    patientDepositId: null,
    amount: null,
    paymentMethod: null
  };
  PaymentMethod = PaymentMethod;
  depositId?: number;
  patientId?: number;
  amount: number = 0;
  paymentMethod: PaymentMethod = PaymentMethod._0; // default Cash
  isSaving = false;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
     private _depositTransactionService: DepositTransactionServiceProxy
  ) {
    super(injector);
  }

   setPaymentMethod(method: PaymentMethod) {
    this.paymentMethod = method;
  }
  ngOnInit(): void {
    if (this.depositId) {
      this.deposit.patientDepositId = this.depositId;
    }
  }


  save(): void {
  if (!this.createDepositForm?.form?.valid || this.paymentMethod == null) {
    this.message.warn('Please fill all required fields and select payment method.');
    return;
  }

  this.saving = true;

  const input = new CreateUpdateDepositTransactionDto();
  input.tenantId = abp.session.tenantId;
  input.patientDepositId = this.depositId!;
  input.amount = this.amount;
  input.paymentMethod = this.paymentMethod;
  this._depositTransactionService
    .createDepositTransaction(input)
    .subscribe({
      next: (result) => {
        if (this.paymentMethod === PaymentMethod._0) {
          // Cash case
          this.notify.success('Deposit collected successfully!');
          this.onSave.emit();
          this.bsModalRef.hide();
        } else {
          // Card case (Stripe redirect)
          if (result) {
            window.location.href = result; // Stripe checkout redirect
          }
        }
      },
      error: () => {
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      }
    });
}
}