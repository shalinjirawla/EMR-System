import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { TableModule } from 'primeng/table';
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
import moment from 'moment';

@Component({
  selector: 'app-edit-purchase-invoice',
  standalone: true,
  imports: [FormsModule, CommonModule, ButtonModule, DropdownModule, InputTextModule, InputNumberModule,
    CalendarModule, TableModule, AbpModalHeaderComponent, AbpModalFooterComponent],
  providers: [PurchaseInvoiceServiceProxy, MedicineMasterServiceProxy],
  templateUrl: './edit-purchase-invoice.component.html'
})
export class EditPurchaseInvoiceComponent extends AppComponentBase implements OnInit {
  @ViewChild('purchaseInvoiceForm', { static: true }) purchaseInvoiceForm: NgForm;
  @ViewChild('itemForm') itemForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  loading = false;
  today: Date = new Date();
  invoiceId?: number;
  invoice: CreateUpdatePurchaseInvoiceDto = new CreateUpdatePurchaseInvoiceDto();
  medicineOptions: MedicineMasterDto[] = [];

  currentItem: CreateUpdatePurchaseInvoiceItemDto = new CreateUpdatePurchaseInvoiceItemDto();
  editIndex: number = -1;
  isEditing = false;
  uiInvoiceDate: Date | null = null;
  uiCurrentExpiry: Date | null = null;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _invoiceService: PurchaseInvoiceServiceProxy,
    private _medicineService: MedicineMasterServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector);
    this.invoice.items = [];
    if (this.bsModalRef.content?.invoiceId) {
      this.invoiceId = this.bsModalRef.content.invoiceId;
    }
  }

  ngOnInit(): void {
    this.loadMedicines();
    if (this.invoiceId) {
      this.loadInvoice(this.invoiceId);
    }
  }

  loadMedicines(): void {
    this._medicineService.getAllListOfMedicine().subscribe({
      next: (res) => {
        this.medicineOptions = res && Array.isArray(res) ? res : [];
      },
      error: (error) => {
        console.error('Error loading medicines:', error);
        this.notify.error('Failed to load medicines');
      }
    });
  }

  loadInvoice(id: number): void {
    this.loading = true;
    this._invoiceService.get(id).subscribe({
      next: (res) => {
        this.invoice = res;
        this.uiInvoiceDate = res.invoiceDate ? this.convertToDate(res.invoiceDate) : null;
        this.invoice.items = res.items || [];

        this.invoice.items.forEach(item => {
          if (item.expiryDate) {
            item.expiryDate = this.convertToDate(item.expiryDate) as any;
          }
        });

        this.updateInvoiceTotal();
        this.loading = false;
        this.cd.detectChanges();
      },
      error: (error) => {
        console.error('Error loading invoice:', error);
        this.notify.error('Failed to load invoice');
        this.loading = false;
      }
    });
  }

  private convertToDate(date: any): Date | null {
    if (!date) {
      return null;
    }

    if (date instanceof Date) {
      return date;
    }

    if (moment.isMoment(date)) {
      return (date as any).toDate();
    }

    if (typeof date === 'string') {
      const parsedMoment = moment(date, ['YYYY-MM-DD', 'DD-MM-YYYY', moment.ISO_8601]);
      if (parsedMoment.isValid()) {
        return parsedMoment.toDate();
      }
    }

    return null;
  }

  onInvoiceDateChange(date: Date): void {
    if (date) {
      this.invoice.invoiceDate = moment(date) as any;
    } else {
      this.invoice.invoiceDate = null;
    }
    this.cd.detectChanges();
  }

  onCurrentExpiryChange(date: Date): void {
    if (date) {
      this.currentItem.expiryDate = moment(date) as any;
    } else {
      this.currentItem.expiryDate = null;
    }
    this.cd.detectChanges();
  }

  getMedicineName(id: number): string {
    if (!id || !this.medicineOptions || this.medicineOptions.length === 0) {
      return '-';
    }
    const medicine = this.medicineOptions.find(x => x.id === id);
    return medicine?.name || '-';
  }

  updateTempTotal(): void {
  }

  saveItem(): void {
    // Validate item form is valid
    if (!this.itemForm.valid) {
      this.message.warn('Please fill all item fields.');
      return;
    }

    if (!this.currentItem.medicineMasterId) {
      this.message.warn('Please select a medicine.');
      return;
    }

    if (!this.currentItem.batchNo || this.currentItem.batchNo.trim() === '') {
      this.message.warn('Please enter batch number.');
      return;
    }

    if (!this.currentItem.expiryDate) {
      this.message.warn('Please select expiry date.');
      return;
    }

    if (!this.currentItem.quantity || this.currentItem.quantity < 1) {
      this.message.warn('Quantity must be at least 1.');
      return;
    }

    if (this.currentItem.purchasePrice === null || this.currentItem.purchasePrice === undefined || this.currentItem.purchasePrice < 0) {
      this.message.warn('Please enter a valid purchase price.');
      return;
    }

    if (this.currentItem.sellingPrice === null || this.currentItem.sellingPrice === undefined || this.currentItem.sellingPrice < 0) {
      this.message.warn('Please enter a valid selling price.');
      return;
    }

    if (this.currentItem.sellingPrice < this.currentItem.purchasePrice) {
      this.message.warn('Selling price cannot be less than purchase price.');
      return;
    }

    if (this.isEditing && this.editIndex > -1) {
      const updatedItem = new CreateUpdatePurchaseInvoiceItemDto();
      Object.assign(updatedItem, this.currentItem);
      this.invoice.items[this.editIndex] = updatedItem;
      this.message.success('Item updated successfully');
      this.isEditing = false;
      this.editIndex = -1;
    } else {
      const newItem = new CreateUpdatePurchaseInvoiceItemDto();
      Object.assign(newItem, this.currentItem);
      this.invoice.items.push(newItem);
      this.message.success('Item added successfully');
    }

    this.resetItemForm();
    this.updateInvoiceTotal();
    this.cd.detectChanges();
  }

  private resetItemForm(): void {
    this.currentItem = new CreateUpdatePurchaseInvoiceItemDto();
    this.uiCurrentExpiry = null;

    if (this.itemForm) {
      this.itemForm.resetForm();

      setTimeout(() => {
        if (this.itemForm && this.itemForm.form) {
          this.itemForm.form.markAsPristine();
          this.itemForm.form.markAsUntouched();
          this.itemForm.form.reset();
        }
        this.cd.detectChanges();
      }, 0);
    }
  }

  editItem(index: number): void {
    if (index < 0 || index >= this.invoice.items.length) {
      this.message.error('Invalid item index');
      return;
    }

    this.currentItem = new CreateUpdatePurchaseInvoiceItemDto();
    Object.assign(this.currentItem, this.invoice.items[index]);

    // Convert date to Date object for the datepicker component
    if (this.currentItem.expiryDate) {
      this.uiCurrentExpiry = this.convertToDate(this.currentItem.expiryDate);
    }

    this.isEditing = true;
    this.editIndex = index;
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.editIndex = -1;
    this.resetItemForm();
  }

  removeItem(index: number): void {
    if (index < 0 || index >= this.invoice.items.length) {
      this.message.error('Invalid item index');
      return;
    }

    this.invoice.items.splice(index, 1);
    this.updateInvoiceTotal();
    this.message.success('Item removed successfully');
    this.cd.detectChanges();
  }

  calculateItemTotal(item: CreateUpdatePurchaseInvoiceItemDto): number {
    if (!item || !item.quantity || !item.purchasePrice) {
      return 0;
    }
    return item.quantity * item.purchasePrice;
  }

  updateInvoiceTotal(): void {
    this.invoice.totalAmount = this.invoice.items.reduce(
      (sum, item) => sum + this.calculateItemTotal(item),
      0
    );
  }
  get isFormValid(): boolean {
    if (!this.purchaseInvoiceForm?.form?.valid) {
      return false;
    }

    if (!this.invoice.items || this.invoice.items.length === 0) {
      return false;
    }

    for (let i = 0; i < this.invoice.items.length; i++) {
      const item = this.invoice.items[i];

      if (!item.quantity || item.quantity < 1) {
        return false;
      }

      if (item.purchasePrice === null || item.purchasePrice === undefined || item.purchasePrice < 0) {
        return false;
      }

      if (item.sellingPrice === null || item.sellingPrice === undefined || item.sellingPrice < 0) {
        return false;
      }

      if (item.sellingPrice < item.purchasePrice) {
        return false;
      }
    }

    return true;
  }
  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please complete the form properly. Ensure all items have valid prices and selling price >= purchase price.');
      return;
    }
    this.invoice.items.forEach(item => {
      if (item.expiryDate) {
        item.expiryDate = this.convertToDate(item.expiryDate) as any;
      }
    });
    if (this.uiInvoiceDate) {
      this.invoice.invoiceDate = moment(this.uiInvoiceDate) as any;
    }

    this.saving = true;
    this.updateInvoiceTotal();
    this._invoiceService.update(this.invoice).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (error) => {
        console.error('Error updating invoice:', error);
        this.notify.error('Failed to save purchase invoice. Please try again.');
        this.saving = false;
      }
    });
  }
}