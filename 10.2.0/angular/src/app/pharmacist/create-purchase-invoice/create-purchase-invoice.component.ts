import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
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
import { PurchaseInvoiceServiceProxy, CreateUpdatePurchaseInvoiceDto, CreateUpdatePurchaseInvoiceItemDto, MedicineMasterServiceProxy, MedicineMasterDto } from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';
import moment from 'moment';

@Component({
  selector: 'app-create-purchase-invoice',
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
  templateUrl: './create-purchase-invoice.component.html',
  styleUrl:'./create-purchase-invoice.component.css'
})
export class CreatePurchaseInvoiceComponent extends AppComponentBase implements OnInit {
  @ViewChild('purchaseInvoiceForm', { static: true }) purchaseInvoiceForm: NgForm;
    @Output() onSave = new EventEmitter<void>();
  
  saving = false;
  today: Date = new Date();

  invoice: CreateUpdatePurchaseInvoiceDto = new CreateUpdatePurchaseInvoiceDto();

  medicineOptions: MedicineMasterDto[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _invoiceService: PurchaseInvoiceServiceProxy,
    private _medicineService: MedicineMasterServiceProxy
  ) {
    super(injector);
    this.invoice.items = [];
  }

  ngOnInit(): void {
    this.loadMedicines();
  }

  loadMedicines() {
    this._medicineService.getAllListOfMedicine().subscribe(res => {
      this.medicineOptions = res;
    });
  }

  addItem() {
    const item = new CreateUpdatePurchaseInvoiceItemDto();
    item.tenantId = abp.session.tenantId;
    item.quantity = 1;
    item.purchasePrice = 0;
    item.sellingPrice = 0;
    this.invoice.items.push(item);
  }

  removeItem(index: number) {
    this.invoice.items.splice(index, 1);
  }

 get isFormValid(): boolean {
  if (!(this.purchaseInvoiceForm?.form?.valid && this.invoice.items.length > 0)) {
    return false;
  }

  // Custom validations
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
  if (!item.expiryDate) {
    return true;
  }

  // Agar moment object hai to toDate() se Date banalo
  let exp: Date;
  if (moment.isMoment(item.expiryDate)) {
    exp = item.expiryDate.toDate();
  } else {
    exp = new Date(item.expiryDate as any);
  }

  return exp < this.today;
}
// Per item total calculate karke return karo (DTO me property nahi hai)
calculateItemTotal(item: CreateUpdatePurchaseInvoiceItemDto): number {
  return (item.quantity || 0) * (item.purchasePrice || 0);
}

// Grand total calculate karke invoice.TotalAmount set karo
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

  // Grand total calculate before sending
  this.updateInvoiceTotal();
debugger
  this._invoiceService.create(this.invoice).subscribe({
    next: () => {
      this.notify.info(this.l('SavedSuccessfully'));
      this.bsModalRef.hide();
      this.onSave.emit();
    },
    error: () => this.saving = false
  });
}

}
