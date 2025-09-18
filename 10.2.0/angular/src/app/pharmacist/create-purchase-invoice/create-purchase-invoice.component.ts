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
    return this.purchaseInvoiceForm?.form?.valid && this.invoice.items.length > 0;
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn("Please complete the form properly and add at least one item.");
      return;
    }

    this.saving = true;
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
