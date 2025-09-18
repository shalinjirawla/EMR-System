import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import {
  PurchaseInvoiceServiceProxy,
  CreateUpdatePurchaseInvoiceDto,
  CreateUpdatePurchaseInvoiceItemDto,
  MedicineMasterServiceProxy,
  MedicineMasterDto
} from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';
import moment from 'moment';

@Component({
  selector: 'app-edit-purchase-invoice',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    ButtonModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    CalendarModule,
    DatePickerModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent
  ],
  providers: [PurchaseInvoiceServiceProxy, MedicineMasterServiceProxy],
  templateUrl: './edit-purchase-invoice.component.html',
  styleUrl: './edit-purchase-invoice.component.css'
})
export class EditPurchaseInvoiceComponent extends AppComponentBase implements OnInit {
  @ViewChild('purchaseInvoiceForm', { static: true }) purchaseInvoiceForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  today: Date = new Date();
  invoiceId?: number;
  invoice: CreateUpdatePurchaseInvoiceDto = new CreateUpdatePurchaseInvoiceDto();
  medicineOptions: MedicineMasterDto[] = [];

  // Properties for UI date binding (PrimeNG works with Date objects)
  uiInvoiceDate: Date | null = null;
  uiExpiryDates: (Date | null)[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _invoiceService: PurchaseInvoiceServiceProxy,
    private _medicineService: MedicineMasterServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector);

    if (this.bsModalRef.content?.invoiceId) {
      this.invoiceId = this.bsModalRef.content.invoiceId;
    }

    this.invoice.items = [];
  }

  ngOnInit(): void {
    this.loadMedicines();

    if (this.invoiceId) {
      this.loadInvoice(this.invoiceId);
    }
  }

  loadMedicines() {
    this._medicineService.getAllListOfMedicine().subscribe(res => {
      this.medicineOptions = res;
      this.cd.detectChanges();
    });
  }

  loadInvoice(id: number) {
    this._invoiceService.get(id).subscribe(res => {
      const result = res;
      this.invoice.id = result.id;
      this.invoice.invoiceNo = result.invoiceNo;
      // safe conversion: result.invoiceDate could be string or Moment
      this.uiInvoiceDate = this.toDateSafe(result.invoiceDate);
      // do NOT directly assign a Date to DTO if DTO expects Moment; we'll sync at save
      this.invoice.supplierName = result.supplierName;
      this.invoice.totalAmount = result.totalAmount;

      this.invoice.items = (result.items || []).map(i => {
        const item = new CreateUpdatePurchaseInvoiceItemDto();
        item.tenantId = i.tenantId;
        item.medicineMasterId = i.medicineMasterId;
        item.batchNo = i.batchNo;
        // keep DTO expiry as Moment (safe)
        item.expiryDate = i.expiryDate ? this.toMomentSafe(i.expiryDate) : null;
        item.quantity = i.quantity;
        item.purchasePrice = i.purchasePrice;
        item.sellingPrice = i.sellingPrice;
        item.id = i.id;
        return item;
      });

      // build uiExpiryDates as Date[] using safe conversion
      this.uiExpiryDates = this.invoice.items.map(it => this.toDateSafe(it.expiryDate));

      this.updateInvoiceTotal();
      this.cd.detectChanges();
    });
  }

  private toDateSafe(input: any): Date | null {
    if (!input) return null;
    // अगर input Moment है
    if ((moment as any).isMoment && (moment as any).isMoment(input)) {
      return input.toDate();
    }
    // अगर input already Date
    if (input instanceof Date) {
      return input;
    }
    // otherwise try native Date parse
    const d = new Date(input);
    return isNaN(d.getTime()) ? null : d;
  }

  private toMomentSafe(input: any): moment.Moment | null {
    if (!input) return null;
    // अगर already moment
    if ((moment as any).isMoment && (moment as any).isMoment(input)) {
      return input;
    }
    // अगर Date object
    if (input instanceof Date) {
      return (moment as any)(input);
    }
    // string / number
    return (moment as any)(input);
  }


  addItem() {
    const item = new CreateUpdatePurchaseInvoiceItemDto();
    item.tenantId = abp.session.tenantId;
    item.quantity = 1;
    item.purchasePrice = 0;
    item.sellingPrice = 0;
    this.invoice.items.push(item);

    // keep UI array in sync
    this.uiExpiryDates.push(null);
  }

  removeItem(index: number) {
    this.invoice.items.splice(index, 1);
    this.uiExpiryDates.splice(index, 1);
  }
  private syncUiDatesToDto() {
    // invoice date
    (this.invoice as any).invoiceDate = this.uiInvoiceDate ? this.toMomentSafe(this.uiInvoiceDate) : null;

    // items expiry
    for (let i = 0; i < this.invoice.items.length; i++) {
      const uiDate = this.uiExpiryDates[i];
      this.invoice.items[i].expiryDate = uiDate ? this.toMomentSafe(uiDate) : null;
    }
  }

  // Convert UI date to Moment for DTO when date changes
  onInvoiceDateChange(date: Date) {
    this.invoice.invoiceDate = date ? moment(date) : null;
  }

  // Convert UI date to Moment for DTO when item expiry date changes
  onExpiryDateChange(index: number, date: Date) {
    this.invoice.items[index].expiryDate = date ? moment(date) : null;
  }

  get isFormValid(): boolean {
    if (!(this.purchaseInvoiceForm?.form?.valid && this.invoice.items.length > 0)) {
      return false;
    }

    for (let item of this.invoice.items) {
      if (!item.purchasePrice || !item.sellingPrice || item.purchasePrice <= 0 || item.sellingPrice <= 0) {
        return false;
      }
      if (item.sellingPrice < item.purchasePrice) {
        return false;
      }
    }
    return true;
  }

  isExpiryInvalid(item: CreateUpdatePurchaseInvoiceItemDto): boolean {
    if (!item.expiryDate) return true;

    let exp: Date;
    if (moment.isMoment(item.expiryDate)) {
      exp = item.expiryDate.toDate();
    } else {
      exp = new Date(item.expiryDate as any);
    }

    return exp < this.today;
  }

  calculateItemTotal(item: CreateUpdatePurchaseInvoiceItemDto): number {
    return (item.quantity || 0) * (item.purchasePrice || 0);
  }

  updateInvoiceTotal() {
    this.invoice.totalAmount = this.invoice.items
      .reduce((sum, it) => sum + this.calculateItemTotal(it), 0);
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn("Please complete the form properly and add at least one valid item.");
      return;
    }

    this.saving = true;
    this.updateInvoiceTotal();
    this.syncUiDatesToDto();
    debugger
    this._invoiceService.update(this.invoice).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => this.saving = false
    });
  }
}