import { Component, OnInit, ViewChild, EventEmitter, Output, Injector, ChangeDetectorRef } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import {
  CollectionStatus,
  CreateUpdatePharmacistPrescriptionsDto, PatientServiceProxy, PaymentMethod, PharmacistInventoryServiceProxy, PharmacistPrescriptionItemWithUnitPriceDto,
  PharmacistPrescriptionsServiceProxy, PrescriptionItemsServiceProxy, PrescriptionServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { MessageService } from 'primeng/api';
import { AppComponentBase } from '@shared/app-component-base';
import { MultiSelectModule } from 'primeng/multiselect';
import { SelectModule } from 'primeng/select';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TextareaModule } from 'primeng/textarea';
import moment from 'moment';
import { InputNumberModule } from 'primeng/inputnumber';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
@Component({
  selector: 'app-edit-pharmacist-prescription',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, ToastModule, AbpModalFooterComponent, InputTextModule, FormsModule,
    CommonModule, ButtonModule, TextareaModule, InputNumberModule, TableModule, DropdownModule
  ],
  templateUrl: './edit-pharmacist-prescription.component.html',
  styleUrl: './edit-pharmacist-prescription.component.css',
  providers: [PrescriptionItemsServiceProxy, PharmacistInventoryServiceProxy, MessageService, PrescriptionServiceProxy, PharmacistPrescriptionsServiceProxy]
})
export class EditPharmacistPrescriptionComponent extends AppComponentBase implements OnInit {
  @ViewChild('createPharmacistPrescriptionForm', { static: true }) createPharmacistPrescriptionForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  id!: number
  selectedPrescription: any = null;
  paymentMethod: PaymentMethod = PaymentMethod._0;
  isSaving = false;
  PaymentMethod = PaymentMethod;
  // arrays / models
  selectedPrescriptionItem: PharmacistPrescriptionItemWithUnitPriceDto[] = [];
  newPrescriptionItem!: PharmacistPrescriptionItemWithUnitPriceDto;
  selectedPrescriptionID!: number;
  _pharmacyNotes!: string;
  selectedPrescriptionName!: string;
  total: number = 0;
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};
  selectedMedicineUnits: { [medicineName: string]: string } = {};
  medicineStocks: { [medicineId: number]: number } = {};
  frequencyOptions = [
    { label: 'Once a day', value: 'Once a day' },
    { label: 'Twice a day', value: 'Twice a day' },
    { label: 'Three times a day', value: 'Three times a day' },
    { label: 'Four times a day', value: 'Four times a day' },
    { label: 'Every 6 hours', value: 'Every 6 hours' },
    { label: 'Every 8 hours', value: 'Every 8 hours' },
    { label: 'Every 12 hours', value: 'Every 12 hours' },
    { label: 'As needed', value: 'As needed' }
  ];
  durationUnits = [
    { label: 'Days', value: 'Days' },
    { label: 'Weeks', value: 'Weeks' },
    { label: 'Months', value: 'Months' }
  ];
  // row id generator for stable dataKey
  private _rowCounter = 1;
  private nextRowId(): number { return this._rowCounter++; }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cdRef: ChangeDetectorRef,
    private PrescriptionItemsService: PrescriptionItemsServiceProxy,
    private _pharmacistInventoryService: PharmacistInventoryServiceProxy,
    private prescriptionService: PrescriptionServiceProxy,
    private pharmacistPrescriptionService: PharmacistPrescriptionsServiceProxy,
    private messageService: MessageService,
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.id) {
      this.GetDetailsByID();
    }
  }
  GetDetailsByID() {
    this.pharmacistPrescriptionService.getPharmacistPrescriptionsById(this.id).subscribe({
      next: (res) => {
        setTimeout(() => {
          if (res.prescriptionId) {
            this.selectedPrescriptionID = res.prescriptionId;
            this._pharmacyNotes = res.pharmacyNotes;
            // const filterdList=res.prescriptionItem.filter(x=>x.pharmacistPrescriptionId==res.)
            this.selectedPrescriptionItem = (res.prescriptionItem || []).map((it: any) => {
              // ensure qty and stable row id
              if (!it.qty || it.qty < 1) it.qty = 1;
              it._rowId = this.nextRowId();
              return it;
            });

            this.selectedPrescriptionName = `${res.patientName || ''} - ${res.issueDate.toDate().toLocaleDateString()} - ${res.prescriptionId}`;

            // clamp quantities immediately if we know stock
            this.selectedPrescriptionItem.forEach(itm => {
              const mid = itm.medicineId || (itm as any).id;
              const stock = (mid ? this.medicineStocks[mid] : undefined);
              if (stock !== undefined && itm.qty > stock) {
                itm.qty = stock;
                this.messageService.add({
                  severity: 'warn',
                  summary: 'Stock Adjusted',
                  detail: `${itm.medicineName} qty adjusted to available stock (${stock})`
                });
              }
            });
            // replace array reference to force table render
            this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
            this.total = this.getPrescriptionTotal();
          }
          this.cdRef.detectChanges();
        });
      },
      error: (err) => {
        // handle error optionally
      }
    });
  }

  setPaymentMethod(method: PaymentMethod) {
    this.paymentMethod = method;
  }
  save() {
    if (!this.createPharmacistPrescriptionForm.valid) return;

    this.isSaving = true;
    const input = new CreateUpdatePharmacistPrescriptionsDto();
    input.id = this.id;
    input.tenantId = abp.session.tenantId;
    input.prescriptionId = this.selectedPrescriptionID;
    input.issueDate = moment();
    input.pharmacyNotes = this._pharmacyNotes;
    input.collectionStatus = CollectionStatus._0;
    input.grandTotal = this.getPrescriptionTotal();
    const resBody: any = {
      pharmacistPrescriptionsDto: input,
      pharmacistPrescriptionsListOfItem: this.selectedPrescriptionItem,
    }
    if (this.paymentMethod === PaymentMethod._0) {

      // Cash -> direct create
      this.pharmacistPrescriptionService.createPharmacistPrescriptionsWithItem(resBody).subscribe({
        next: () => {
          this.notify.success('Created successfully!');
          this.onSave.emit();
          this.bsModalRef.hide();
        },
        error: () => this.isSaving = false
      });
    } else {
      // card flow (if implemented)
      this.isSaving = false;
    }
  }

  getPrescriptionTotal(): number {
    if (!this.selectedPrescriptionItem || !this.selectedPrescriptionItem.length) return 0;
    return this.selectedPrescriptionItem
      .map(itm => (itm.unitPrice || 0) * (itm.qty || 0))
      .reduce((sum, val) => sum + val, 0);
  }

  /**
   * onQtyChange: receives the newQty from ngModelChange.
   * clamps qty to available stock and forces table re-render reliably.
   */
  onQtyChange(itm: any, newQty: number) {
    // assign requested value first
    itm.qty = Number(newQty) || 0;
    if (itm.qty < 1) itm.qty = 1;

    const medicineId = itm.medicineId || itm.id;
    const availableStock = (medicineId ? this.medicineStocks[medicineId] : undefined);

    if (availableStock !== undefined && availableStock >= 0 && itm.qty > availableStock) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Stock Limit',
        detail: `Only ${availableStock} units available for ${itm.medicineName}`
      });

      // update after current event loop so p-inputNumber internal state finishes
      setTimeout(() => {
        itm.qty = availableStock;
        // replace array to force p-table re-render of the row (dataKey uses _rowId)
        this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
        this.cdRef.detectChanges();
        this.total = this.getPrescriptionTotal();
      }, 0);

      return;
    }

    // if stock unknown, fetch and then validate
    if ((availableStock === undefined || availableStock === null) && medicineId) {
      this._pharmacistInventoryService.get(medicineId).subscribe({
        next: (mres) => {
          const s = mres.stock || 0;
          this.medicineStocks[medicineId] = s;
          if (itm.qty > s) {
            this.messageService.add({
              severity: 'warn',
              summary: 'Stock Limit',
              detail: `Only ${s} units available for ${itm.medicineName}`
            });
            setTimeout(() => {
              itm.qty = s;
              this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
              this.cdRef.detectChanges();
              this.total = this.getPrescriptionTotal();
            }, 0);
          } else {
            this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
            this.cdRef.detectChanges();
            this.total = this.getPrescriptionTotal();
          }
        },
        error: () => {
          // ignore fetch error
        }
      });
      return;
    }

    // normal path
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
    this.total = this.getPrescriptionTotal();
  }

  loadMedicines(afterLoadedCallback?: () => void) {
    this._pharmacistInventoryService.getAll(
      undefined, undefined, undefined, undefined, undefined, true, undefined, undefined
    ).subscribe({
      next: (res) => {
        if (res.items && res.items.length > 0) {
          const selectedIds = this.selectedPrescriptionItem?.map(itm => itm.medicineId) || [];
          this.medicineOptions = res.items.map(item => ({
            label: item.medicineName,
            value: item.id,
            name: item.medicineName,
            disabled: selectedIds.includes(item.id)
          })); 

          res.items.forEach(medicine => {
            const units = medicine.unit ? medicine.unit.split(',').map((u: string) => u.trim()) : [];
            this.medicineDosageOptions[medicine.medicineName] = units;
            this.selectedMedicineUnits[medicine.medicineName] = units.length ? units[0] : '';
            this.medicineStocks[medicine.id] = medicine.stock || 0;
          });
        }
        if (afterLoadedCallback) afterLoadedCallback();
      },
      error: (err) => {
        this.notify.error('Could not load medicines');
        if (afterLoadedCallback) afterLoadedCallback();
      }
    });
  }

  getDosageOptions(medicineName: string): any[] {
    if (!medicineName || !this.medicineDosageOptions[medicineName]) return [];
    return this.medicineDosageOptions[medicineName].map(unit => ({ label: unit, value: unit }));
  }

  onMedicineChange(item: any) {
    const selected = this.medicineOptions.find(m => m.value === item.medicineId);
    if (selected) {
      item.medicineName = selected.name;
      if (this.medicineDosageOptions[selected.name]) {
        item.dosage = this.selectedMedicineUnits[selected.name];
      } else {
        item.dosage = '';
      }
    }

    if (item.medicineId > 0) {
      this.GetPriceOfMedicine(item.medicineId);
    }
  }
  NewItem(): void {
    this.loadMedicines();
    const item = new PharmacistPrescriptionItemWithUnitPriceDto();
    item.init({
      prescriptionId: this.selectedPrescriptionID,
      medicineId: 0,
      medicineName: '',
      dosage: '',
      frequency: '',
      duration: '',
      instructions: '',
      qty: 1,
      unitPrice: 0,
      totalPayableAmount: 0,
      isPrescribe: false,
    });
    (item as any).durationValue = 1;
    (item as any).durationUnit = 'Days';
    (item as any)._isNew = true;
    (item as any)._rowId = this.nextRowId();
    this.newPrescriptionItem = item;
  }

  removeItem(index: number): void {
    this.selectedPrescriptionItem.splice(index, 1);
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
    this.total = this.getPrescriptionTotal();
     this.loadMedicines();
  }

  GetPriceOfMedicine(_medicineId: number) {
    this._pharmacistInventoryService.get(_medicineId).subscribe({
      next: (res) => {
        if (this.newPrescriptionItem) this.newPrescriptionItem.unitPrice = res.sellingPrice;
        this.medicineStocks[_medicineId] = res.stock || 0;
      },
      error: () => { }
    });
  }

  addItem() {
    if (this.newPrescriptionItem) {
      const medicineId = this.newPrescriptionItem.medicineId;
      const stock = (medicineId ? this.medicineStocks[medicineId] : undefined);
      if (stock !== undefined && stock >= 0 && this.newPrescriptionItem.qty > stock) {
        this.messageService.add({
          severity: 'warn',
          summary: 'Stock Limit',
          detail: `Only ${stock} units available for ${this.newPrescriptionItem.medicineName}`
        });
        // clamp
        this.newPrescriptionItem.qty = stock;
      }

      this.newPrescriptionItem.duration =
        `${(this.newPrescriptionItem as any).durationValue} ${(this.newPrescriptionItem as any).durationUnit}`;

      // ensure new item has stable row id
      (this.newPrescriptionItem as any)._rowId = this.nextRowId();

      this.selectedPrescriptionItem.push(this.newPrescriptionItem);
      this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
      this.newPrescriptionItem = null;
      this.total = this.getPrescriptionTotal();
    }
  }
}