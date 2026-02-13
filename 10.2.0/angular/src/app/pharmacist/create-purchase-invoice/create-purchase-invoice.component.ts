import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { TableModule } from 'primeng/table';
import { DatePickerModule } from 'primeng/datepicker';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import {
  PurchaseInvoiceServiceProxy, CreateUpdatePurchaseInvoiceDto, CreateUpdatePurchaseInvoiceItemDto,
  MedicineMasterServiceProxy, MedicineMasterDto
} from '@shared/service-proxies/service-proxies';
import moment from 'moment';

@Component({
  selector: 'app-create-purchase-invoice',
  standalone: true,
  imports: [FormsModule, CommonModule, ButtonModule, DropdownModule, InputTextModule, InputNumberModule,
    CalendarModule, DatePickerModule, TableModule, AbpModalHeaderComponent, AbpModalFooterComponent],
  providers: [PurchaseInvoiceServiceProxy, MedicineMasterServiceProxy],
  templateUrl: './create-purchase-invoice.component.html',
  styleUrls: ['./create-purchase-invoice.component.css']
})
export class CreatePurchaseInvoiceComponent extends AppComponentBase implements OnInit {
  @ViewChild('purchaseInvoiceForm', { static: true }) purchaseInvoiceForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  @ViewChild('itemForm') itemForm: NgForm;

  saving = false;
  today: Date = new Date();
  invoice: CreateUpdatePurchaseInvoiceDto = new CreateUpdatePurchaseInvoiceDto();
  medicineOptions: MedicineMasterDto[] = [];

  currentItem: CreateUpdatePurchaseInvoiceItemDto = new CreateUpdatePurchaseInvoiceItemDto();
  editIndex: number = -1;
  isEditing = false;
  medicinesLoading = false;

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
  loadMedicines(): void {
    this.medicinesLoading = true;
    this._medicineService.getAllListOfMedicine().subscribe({
      next: (res) => {
        this.medicineOptions = res && Array.isArray(res) ? res : [];
        this.medicinesLoading = false;
        console.log('Medicines loaded successfully:', this.medicineOptions.length);
      },
      error: (error) => {
        console.error('Error loading medicines:', error);
        this.medicineOptions = [];
        this.medicinesLoading = false;
        this.notify.error('Failed to load medicines. Please try again.');
      }
    });
  }


  getMedicineName(id: number): string {
    if (!id || !this.medicineOptions || this.medicineOptions.length === 0) {
      return '-';
    }
    const medicine = this.medicineOptions.find(x => x.id === id);
    return medicine?.name || '-';
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


  updateTempTotal(): void {
  }
  saveItem(): void {
    if (!this.itemForm.valid) {
      this.message.warn('Please fill all required item fields.');
      return;
    }

    if (this.currentItem.sellingPrice < this.currentItem.purchasePrice) {
      this.message.warn('Selling price cannot be less than purchase price.');
      return;
    }

    if (!this.currentItem.quantity || this.currentItem.quantity < 1) {
      this.message.warn('Quantity must be at least 1.');
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
  }
  private resetItemForm(): void {
    this.currentItem = new CreateUpdatePurchaseInvoiceItemDto();

    if (this.itemForm) {
      this.itemForm.resetForm();

      setTimeout(() => {
        if (this.itemForm && this.itemForm.form) {
          this.itemForm.form.markAsPristine();
          this.itemForm.form.markAsUntouched();
          this.itemForm.form.reset();
        }
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

    if (this.currentItem.expiryDate) {
      this.currentItem.expiryDate = this.convertToDate(this.currentItem.expiryDate) as any;
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
    if (!this.purchaseInvoiceForm?.form) {
      return false;
    }

    const mainFormValid = this.purchaseInvoiceForm.form.valid;
    const hasItems = this.invoice.items && this.invoice.items.length > 0;

    return mainFormValid && hasItems;
  }
  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please complete the form properly and add at least one valid item.');
      return;
    }

    if (!this.invoice.items || this.invoice.items.length === 0) {
      this.message.warn('Please add at least one item to the invoice.');
      return;
    }

    this.invoice.items.forEach(item => {
      if (item.expiryDate) {
        item.expiryDate = this.convertToDate(item.expiryDate) as any;
      }
    });

    this.saving = true;
    this.updateInvoiceTotal();

    this._invoiceService.create(this.invoice).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (error) => {
        console.error('Error saving invoice:', error);
        this.notify.error('Failed to save purchase invoice. Please try again.');
        this.saving = false;
      }
    });
  }
}