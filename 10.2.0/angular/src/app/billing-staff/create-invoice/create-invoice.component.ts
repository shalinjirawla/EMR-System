// Updated Component Class
import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { AppComponentBase } from '@shared/app-component-base';
import { AppointmentServiceProxy, CreateUpdateInvoiceDto, InvoiceItemDto, InvoiceServiceProxy, InvoiceStatus, PatientDropDownDto, PatientServiceProxy, PaymentMethod } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { DropdownModule } from 'primeng/dropdown';
import { SelectModule } from 'primeng/select';
import moment from 'moment';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { Calendar } from 'primeng/calendar';

@Component({
  selector: 'app-create-invoice',
  standalone: true,
  imports: [
    FormsModule,
    CalendarModule,
    DropdownModule,
    CommonModule,
    SelectModule,
    AbpModalFooterComponent,
    AbpModalHeaderComponent,
    ProgressSpinnerModule
  ],
  templateUrl: './create-invoice.component.html',
  styleUrl: './create-invoice.component.css',
  providers: [PatientServiceProxy, InvoiceServiceProxy]
})
export class CreateInvoiceComponent extends AppComponentBase implements OnInit {
  @ViewChild('invoiceForm', { static: true }) invoiceForm: NgForm;
  @Output() onSave = new EventEmitter<void>();


  isProcessingPayment = false;
  saving = false;
  patients: PatientDropDownDto[] = [];
  moment = moment;
  createdInvoice: any;
  paymentProcessingError = '';
  selectedPatientId: number | null = null;
  showInvoiceTypeDropdown = false;
  selectedInvoiceType: number | null = null;
   invoiceTypeEnum = {
    DailyInvoice: 0,
    FullInvoice: 1
  };
  invoiceTypeOptions = [
    { label: 'Daily Invoice', value: this.invoiceTypeEnum.DailyInvoice },
    { label: 'Full Invoice', value: this.invoiceTypeEnum.FullInvoice }
  ];

  paymentMethodOptions = [
    { label: 'Cash', value: PaymentMethod._0 },
    { label: 'Card', value: PaymentMethod._1 }
  ];


invoice = {
    tenantId: abp.session.tenantId,
    patientId: null as number | null,
    status: InvoiceStatus._0,
    paymentMethod: null as PaymentMethod | null,
    items: [] as InvoiceItemDto[]
  };

   // Manual new item
  newItem = {
    description: '',
    unitPrice: 0,
    quantity: 1
  };

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _invoiceService: InvoiceServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadPatients();
  }


  loadPatients(): void {
    this._patientService.patientDropDown()
      .subscribe({
        next: (res) => this.patients = res,
        error: () => this.notify.error('Failed to load patients')
      });
  }
  onPatientChange(event: any): void {
    const patientId = event.value;
    this.selectedPatientId = patientId;
    this.invoice.patientId = patientId;

    this._patientService.get(patientId).subscribe(patient => {
      if (patient.isAdmitted) {
        this.showInvoiceTypeDropdown = true;
        this.selectedInvoiceType=undefined;
        this.invoice.items = [];
      } else {
        this.showInvoiceTypeDropdown = false;
        this.loadCharges(patientId, this.invoiceTypeEnum.FullInvoice);
      }
    });
  }

   onInvoiceTypeChange(event: any): void {
    if (this.selectedPatientId) {
      this.loadCharges(this.selectedPatientId, event.value);
    }
  }

   private loadCharges(patientId: number, invoiceType: number): void {
    this._invoiceService.getChargesByPatient(patientId, invoiceType)
      .subscribe(charges => {
        this.invoice.items = charges.map(c => {
          const item = new InvoiceItemDto();
          item.id = 0;
          item.invoiceId = 0;
          item.description = c.description;
          item.unitPrice = c.amount;
          item.quantity = 1;
          (item as any).isRemovable = false;
          (item as any).entryDate = c.entryDate;
          return item;
        });
        this.cd.detectChanges();
      });
  }

   canAddItem(): boolean {
    return this.newItem.description.trim() !== '' &&
      this.newItem.unitPrice > 0 &&
      this.newItem.quantity > 0;
  }
  addItem(): void {
    const item = new InvoiceItemDto();
    item.id = 0;
    item.invoiceId = 0;
    item.description = this.newItem.description;
    item.unitPrice = this.newItem.unitPrice;
    item.quantity = this.newItem.quantity;
    item.entryDate = moment(); 
    (item as any).isRemovable = true;
    this.invoice.items.push(item);

    this.newItem = { description: '', unitPrice: 0, quantity: 1 };
  }

  removeItem(index: number): void {
    this.invoice.items.splice(index, 1);
  }
calculateSubtotal(): number {
    return this.invoice.items.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
  }
  calculateGst(): number {
    return this.calculateSubtotal() * 0.18;
  }
  calculateTotal(): number {
    return this.calculateSubtotal() + this.calculateGst();
  }

  isSaveDisabled(): boolean {
    return !this.invoiceForm?.valid ||
      this.saving ||
      this.invoice.items.length === 0 ||
      !this.invoice.patientId;
  }
  private async redirectToStripeCheckout(invoiceId: number, amount: number): Promise<void> {
    this.isProcessingPayment = true;
    this.paymentProcessingError = '';

    try {
      const baseUrl = window.location.origin;
      const billingStaffBase = '/app/billing-staff/invoices';

      // Properly format query parameters
      const successUrl = `${baseUrl}${billingStaffBase}?payment=success&invoiceId=${invoiceId}&amount=${amount}`;
      const cancelUrl = `${baseUrl}${billingStaffBase}?payment=canceled&invoiceId=${invoiceId}`;

      const checkoutUrl = await this._invoiceService
        .createStripeCheckoutSession(invoiceId, amount, successUrl, cancelUrl)
        .toPromise();

      window.location.href = checkoutUrl;
    } catch (error) {
      this.isProcessingPayment = false;
      this.paymentProcessingError = error.message || 'Failed to start payment process';
      this.notify.error(this.paymentProcessingError);
    }
  }

  save(): void {
    this.saving = true;
    this.isProcessingPayment = true;
    this.cd.detectChanges();

    const input = new CreateUpdateInvoiceDto({
      id: 0,
      tenantId: abp.session.tenantId,
      patientId: this.invoice.patientId,
       invoiceType: this.selectedInvoiceType ?? this.invoiceTypeEnum.FullInvoice,
      invoiceDate: moment(),
      status: this.invoice.status,
      paymentMethod: this.invoice.paymentMethod??null,
      items: this.invoice.items,
      subTotal: this.calculateSubtotal(),
      gstAmount: this.calculateGst(),
      totalAmount: this.calculateTotal()
    });
    this._invoiceService.create(input).subscribe({
      next: (createdInvoice) => {
        this.createdInvoice = createdInvoice;
        this.isProcessingPayment = false;
        this.saving = false;
        this.notify.info(this.l('InvoiceCreatedSuccessfully'));
        this.onSave.emit();
        this.bsModalRef.hide();
        this.cd.detectChanges();
      },
      error: (err) => {
        this.isProcessingPayment = false;
        this.saving = false;
        this.notify.error('Failed to create invoice');
        console.error('Invoice creation error:', err);
      }
    });
  }
}