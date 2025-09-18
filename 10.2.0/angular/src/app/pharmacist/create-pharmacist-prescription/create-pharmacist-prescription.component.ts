import { Component, OnInit, ViewChild, EventEmitter, Output, Injector, ChangeDetectorRef } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import {
  CollectionStatus,
  CreatePharmacistPrescriptionsWithItemDto,
  CreateUpdatePharmacistPrescriptionsDto,
  OrderStatus,
  PatientServiceProxy,
  PaymentMethod,
  PharmacistInventoryServiceProxy,
  PharmacistPrescriptionItemWithUnitPriceDto,
  PharmacistPrescriptionsServiceProxy,
  PrescriptionItemDto,
  PrescriptionItemsServiceProxy,
  PrescriptionServiceProxy,
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
  selector: 'app-create-pharmacist-prescription',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, ToastModule, AbpModalFooterComponent, MultiSelectModule, FormsModule, SelectModule,
    CommonModule, ButtonModule, TextareaModule, InputNumberModule, TableModule, DropdownModule, InputTextModule
  ],
  templateUrl: './create-pharmacist-prescription.component.html',
  styleUrl: './create-pharmacist-prescription.component.css',
  providers: [PatientServiceProxy, PrescriptionItemsServiceProxy, PharmacistInventoryServiceProxy, MessageService, PrescriptionServiceProxy, PharmacistPrescriptionsServiceProxy]
})
export class CreatePharmacistPrescriptionComponent extends AppComponentBase implements OnInit {
  @ViewChild('createPharmacistPrescriptionForm', { static: true }) createPharmacistPrescriptionForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  patients: any[] = [];
  prescriptions: any[] = [];
  selectedPrescription: any = null;
  paymentMethod: PaymentMethod = PaymentMethod._0;
  isSaving = false;
  selectedPatient: any;
  PaymentMethod = PaymentMethod;
  selectedPrescriptionItem: PharmacistPrescriptionItemWithUnitPriceDto[] = [];
  newPrescriptionItem!: PharmacistPrescriptionItemWithUnitPriceDto;
  selectedPrescriptionID!: number;
  _pharmacyNotes!: string;
  total: number = 0;
  newUnitPrice!: any;
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};
  selectedMedicineUnits: { [medicineName: string]: string } = {};
  _medicinePrice!: any;
  medicineStocks: { [medicineId: number]: number } = {}; // stock map
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
    private patientService: PatientServiceProxy,
    private prescriptionService: PrescriptionServiceProxy,
    private PrescriptionItemsService: PrescriptionItemsServiceProxy,
    private pharmacistPrescriptionService: PharmacistPrescriptionsServiceProxy,
    private messageService: MessageService,
    private _pharmacistInventoryService: PharmacistInventoryServiceProxy,
  ) {
    super(injector);
  }
  ngOnInit() {
    this.loadPatients();
  }
  loadPatients() {
    this.patientService.getOpdPatients().subscribe({
      next: (res) => {
        this.patients = res;
      }, error: (err) => { }
    });
  }
  onPatientChange() {
    if (this.selectedPatient) {
      this.selectedPrescriptionItem = [];
      this.loadPrescriptions(this.selectedPatient);
    } else {
      this.prescriptions = [];
      this.selectedPrescription = null;
    }
  }
  loadPrescriptions(patientId: number) {
    this.prescriptionService.getPrescriptionsByPatient(patientId).subscribe({
      next: (res) => {
        this.prescriptions = res.items.map(p => ({
          ...p,
          prescriptionName: `${p.patient?.fullName || ''} - ${p.issueDate.toDate().toLocaleDateString()} - ${p.id}`
        }));
      }, error: () => { }
    });
  }

  onPrescriptionChange(prescriptionId: number) {
    if (!prescriptionId) {
      this.selectedPrescriptionItem = [];
      return;
    }
    this.selectedPrescriptionID = prescriptionId;

    // take items from selected prescription; ensure stable row ids and qty >=1
    const items = this.prescriptions.find(x => x.id === prescriptionId)?.pharmacistPrescription || [];
    const items1 = items.filter(x => x.isPrescribe == true)
    this.selectedPrescriptionItem = (items1 || []).map((it: any) => {
      if (!it.qty || it.qty < 1) it.qty = 1;
      it._rowId = this.nextRowId();
      return it;
    });

    // clamp known stocks immediately and notify
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

    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
    this.total = this.getPrescriptionTotal();
    this.cdRef.detectChanges();
  }
  setPaymentMethod(method: PaymentMethod) {
    this.paymentMethod = method;
  }

  save() {
    if (!this.createPharmacistPrescriptionForm.valid) return;
    this.isSaving = true;
    const input = new CreateUpdatePharmacistPrescriptionsDto();
    input.tenantId = abp.session.tenantId;
    input.prescriptionId = this.selectedPrescriptionID;
    input.issueDate = moment();
    input.pharmacyNotes = this._pharmacyNotes;
    input.collectionStatus = CollectionStatus._1;
    input.paymentMethod= this.paymentMethod;
    input.isPaid=false;
    input.pickedUpByPatient=this.selectedPatient;
    input.grandTotal = this.getPrescriptionTotal();
    const resBody: any = {
      pharmacistPrescriptionsDto: input,
      pharmacistPrescriptionsListOfItem: this.selectedPrescriptionItem,
    }
    this.pharmacistPrescriptionService.handlePharmacistPrescriptionPayment(resBody).subscribe({
    next: (result: any) => {
      if (this.paymentMethod === PaymentMethod._0) {
        // ✅ Cash → normal success
        this.notify.success('Created successfully!');
        this.onSave.emit();
        this.bsModalRef.hide();
      } else if (this.paymentMethod === PaymentMethod._1 && result) {
        // ✅ Card → redirect to Stripe checkout
        window.location.href = result;
      }
    },
    error: () => this.isSaving = false,
    complete: () => this.isSaving = false
  });
}

  onQtyChange(itm: any, newQty: number) {
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

      setTimeout(() => {
        itm.qty = availableStock;
        this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
        this.cdRef.detectChanges();
        this.total = this.getPrescriptionTotal();
      }, 0);

      return;
    }

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
        error: () => { }
      });
      return;
    }

    // normal path
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
    this.total = this.getPrescriptionTotal();
  }

  getPrescriptionTotal(): number {
    if (!this.selectedPrescriptionItem?.length) return 0;
    return this.selectedPrescriptionItem
      .map(itm => (itm.unitPrice || 0) * (itm.qty || 0))
      .reduce((sum, val) => sum + val, 0);
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

          res.items.forEach((medicine: any) => {
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
      prescriptionId: null,
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
    this.loadMedicines();
    this.total = this.getPrescriptionTotal();
  }

  GetPriceOfMedicine(_medicineId: number) {
    this._pharmacistInventoryService.get(_medicineId).subscribe({
      next: (res) => {
        if (this.newPrescriptionItem) this.newPrescriptionItem.unitPrice = res.sellingPrice;
        this.medicineStocks[_medicineId] = res.stock || 0;
      }, error: () => { }
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
        this.newPrescriptionItem.qty = stock;
      }

      this.newPrescriptionItem.duration = `${(this.newPrescriptionItem as any).durationValue} ${(this.newPrescriptionItem as any).durationUnit}`;
      (this.newPrescriptionItem as any)._rowId = this.nextRowId();
      this.selectedPrescriptionItem.push(this.newPrescriptionItem);
      this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
      this.newPrescriptionItem = null;
      this.total = this.getPrescriptionTotal();
    }
  }
  // helper: returns stock for display (if known)
  getStockForItem(itm: any) {
    const mid = itm.medicineId || itm.id;
    if (!mid) return '-';
    return this.medicineStocks[mid] !== undefined ? this.medicineStocks[mid] : '-';
  }

  // optional method kept if you still want to use server-side check
  checkForMedicine(medicineId: any, qty: any) {
    this._pharmacistInventoryService.getMedicineStatus(medicineId, qty)
      .subscribe({
        next: (res) => {
          // you can show toast instead of alert
          this.messageService.add({ severity: 'info', summary: 'Stock Status', detail: String(res) });
        }, error: () => { }
      });
  }
}