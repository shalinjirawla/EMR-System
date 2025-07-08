// Updated Component Class
import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { AppComponentBase } from '@shared/app-component-base';
import { AppointmentServiceProxy, CreateUpdateInvoiceDto, InvoiceItemDto, InvoiceItemType, InvoiceServiceProxy, InvoiceStatus, PatientDropDownDto, PatientServiceProxy, PaymentMethod } from '@shared/service-proxies/service-proxies';
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
  @ViewChild(Calendar) calendar: Calendar;
  @Output() onSave = new EventEmitter<void>();


  isProcessingPayment = false;
  saving = false;
  patients: PatientDropDownDto[] = [];
  moment = moment;
  minDate: Date;
  maxDate: Date;
  showDateError = false;
  createdInvoice: any;
  paymentProcessingError = '';
  amountPaid: number = 0;
  amountPaidError = false;
  paymentMethodCashValue = PaymentMethod._0; // For template comparison
  paymentMethodCardValue = PaymentMethod._1; // For template comparison

  // Item type options for dropdown
  itemTypeOptions = [
    { label: 'Consultation', value: InvoiceItemType._0 },
    { label: 'Medicine', value: InvoiceItemType._1 },
    { label: 'Lab Test', value: InvoiceItemType._2 }
  ];

  paymentMethodOptions = [
    { label: 'Cash', value: PaymentMethod._0 },
    { label: 'Card', value: PaymentMethod._1 }
  ];

  // Template for new items
  newItem = {
    itemType: InvoiceItemType._0,  // Initialize with a specific enum value
    description: '',
    unitPrice: 0,
    quantity: 1
  };
  ngAfterViewInit() {
    if (this.calendar) {
      this.calendar.ngOnInit();
    }
  }
  getItemTypeLabel(type: InvoiceItemType): string {
    const option = this.itemTypeOptions.find(opt => opt.value === type);
    return option ? option.label : 'Unknown';
  }
  validateAmountPaid(): void {
    this.amountPaidError = this.amountPaid > this.calculateTotal();
  }

  invoice = {
    tenantId: abp.session.tenantId,
    patientId: null as number | null,
    dueDate: moment().add(15, 'days'),
    status: InvoiceStatus._0,
    paymentMethod: PaymentMethod._0 as PaymentMethod | null,
    items: [] as InvoiceItemDto[]
  };

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _invoiceService: InvoiceServiceProxy
  ) {
    super(injector);
    const today = new Date();
    this.minDate = today;
    this.maxDate = new Date();
    this.maxDate.setDate(today.getDate() + 15);
  }

  ngOnInit(): void {
    this.loadPatients();
  }

  validateDueDate(): void {
    if (this.invoice.dueDate) {
      const selectedDate = moment(this.invoice.dueDate);
      const today = moment();
      const maxAllowedDate = moment().add(15, 'days');

      this.showDateError = selectedDate.isBefore(today, 'day') || selectedDate.isAfter(maxAllowedDate, 'day');
    } else {
      this.showDateError = false;
    }
  }

  loadPatients(): void {
    this._patientService.patientDropDown()
      .subscribe({
        next: (res) => this.patients = res,
        error: () => this.notify.error('Failed to load patients')
      });
  }

  // Item management methods
  canAddItem(): boolean {
    return this.newItem.description.trim() !== '' &&
      this.newItem.unitPrice > 0 &&
      this.newItem.quantity > 0;
  }
  // In your component class
  onItemTypeChange(event: any) {
    this.newItem.itemType = event.value.value;
  }
  addItem(): void {
    const item = new InvoiceItemDto();

    // Set properties directly
    item.itemType = this.newItem.itemType;
    item.description = this.newItem.description;
    item.unitPrice = this.newItem.unitPrice;
    item.quantity = this.newItem.quantity;
    item.totalPrice = this.newItem.unitPrice * this.newItem.quantity;

    // Set default values for other required properties
    item.id = 0;
    item.invoiceId = 0;

    this.invoice.items.push(item);

    // Reset new item form WITHOUT changing the itemType
    this.newItem = {
      itemType: this.newItem.itemType, // Keep the same item type
      description: '',
      unitPrice: 0,
      quantity: 1
    };
  }

  removeItem(index: number): void {
    this.invoice.items.splice(index, 1);
  }



  // Calculation methods
  calculateSubtotal(): number {
    return this.invoice.items.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
  }

  calculateGst(): number {
    return this.calculateSubtotal() * 0.18; // 18% GST
  }

  calculateTotal(): number {
    return this.calculateSubtotal() + this.calculateGst();
  }
  isSaveDisabled(): boolean {
    return !this.invoiceForm?.valid ||
      this.saving ||
      this.invoice.items.length === 0 ||
      !this.invoice.patientId ||
      this.showDateError ||
      this.amountPaidError;
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

    let paymentStatus = InvoiceStatus._0;
    const total = this.calculateTotal();
    if (this.amountPaid === total) {
      paymentStatus = InvoiceStatus._1; // Paid
    } else if (this.amountPaid > 0) {
      paymentStatus = InvoiceStatus._2; // Partially Paid
    }

    const input = new CreateUpdateInvoiceDto({
      id: 0,
      tenantId: abp.session.tenantId,
      appointmentId: 1,
      patientId: this.invoice.patientId,
      invoiceDate: moment(),
      dueDate: moment(this.invoice.dueDate),
      status: this.invoice.status,
      paymentMethod: this.invoice.paymentMethod,
      items: this.invoice.items,
      amountPaid: this.amountPaid,
      subTotal: this.calculateSubtotal(),
      gstAmount: this.calculateGst(),
      totalAmount: this.calculateTotal()
    });

    this._invoiceService.create(input).subscribe({
      next: (createdInvoice) => {
        this.createdInvoice = createdInvoice;

        if (this.invoice.paymentMethod === PaymentMethod._1) {
          const remainingAmount = this.amountPaid;
          const amountToCharge = remainingAmount > 0 ? remainingAmount : 0;
          this.bsModalRef.hide();
          this.redirectToStripeCheckout(
            createdInvoice.id,
            amountToCharge
          );
        } else {
          // Cash payment
          this.isProcessingPayment = false;
          this.saving = false;
          this.notify.info(this.l('InvoiceCreatedSuccessfully'));
          this.onSave.emit();
          this.bsModalRef.hide();
          this.cd.detectChanges();
        }
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