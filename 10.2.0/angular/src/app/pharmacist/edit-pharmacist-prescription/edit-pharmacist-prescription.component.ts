import { Component, OnInit, ViewChild, EventEmitter, Output, Injector, ChangeDetectorRef } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import {
  CollectionStatus,
  CreateUpdatePharmacistPrescriptionsDto,
  MedicineFormMasterServiceProxy,
  MedicineMasterServiceProxy,
  PatientServiceProxy,
  PaymentMethod,
  PharmacistInventoryServiceProxy,
  PharmacistPrescriptionItemWithUnitPriceDto,
  PharmacistPrescriptionsServiceProxy,
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

interface LocalPharmacistItem extends Partial<PharmacistPrescriptionItemWithUnitPriceDto> {
  _rowId?: number;
  _isNew?: boolean;
  batchId?: number;
  batchNo?: string;
  expiryDate?: Date | null;
  durationValue?: number;
  durationUnit?: string;
}

interface MedicineBatch {
  id: number;
  batchNo: string;
  expiryDate: Date | null;
  quantity: number;
  sellingPrice: number;
  daysToExpire?: number;
  isExpire?: boolean;
}

@Component({
  selector: 'app-edit-pharmacist-prescription',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, ToastModule, AbpModalFooterComponent, InputTextModule, FormsModule,
    CommonModule, ButtonModule, TextareaModule, InputNumberModule, TableModule, DropdownModule
  ],
  templateUrl: './edit-pharmacist-prescription.component.html',
  styleUrl: './edit-pharmacist-prescription.component.css',
  providers: [PrescriptionItemsServiceProxy, MedicineMasterServiceProxy, MedicineFormMasterServiceProxy,
    PharmacistInventoryServiceProxy, MessageService, PrescriptionServiceProxy, PharmacistPrescriptionsServiceProxy]
})
export class EditPharmacistPrescriptionComponent extends AppComponentBase implements OnInit {
  @ViewChild('createPharmacistPrescriptionForm', { static: true }) createPharmacistPrescriptionForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  id!: number;
  selectedPrescription: any = null;
  paymentMethod: PaymentMethod = PaymentMethod._0;
  isSaving = false;
  PaymentMethod = PaymentMethod;

  // Arrays and models
  selectedPrescriptionItem: LocalPharmacistItem[] = [];
  newPrescriptionItem!: LocalPharmacistItem | null;
  selectedPrescriptionID!: number;
  _pharmacyNotes!: string;
  ispaid: any;
  receiptNumber: string;
  selectedPrescriptionName!: string;
  total: number = 0;

  // Medicine and batch management
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};
  selectedMedicineUnits: { [medicineName: string]: string } = {};
  medicineTypes: any[] = [];
  medicinesByType: any[] = [];
  medicineBatches: { [medicineId: number]: any[] } = {};
  medicineStocks: { [medicineId: number]: number } = {};
  batchAllocations: { [medicineId: number]: { [batchId: number]: number } } = {};
  disabledMedicineIds: Set<number> = new Set<number>();

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

  // Row id generator for stable dataKey
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
    private _medicineFormService: MedicineFormMasterServiceProxy,
    private messageService: MessageService,
    private _medicineMasterService: MedicineMasterServiceProxy

  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.id) {
      this.GetDetailsByID();
    }
    this.loadMedicineTypes();
  }
  loadMedicineTypes() {
    this._medicineForm_service_getAll();
  }
  private _medicineForm_service_getAll() {
    this._medicineFormService.getAlldicineFormByTenantId(abp.session.tenantId).subscribe({
      next: (res: any) => this.medicineTypes = (res.items || []).map((x: any) => ({ id: x.id, name: x.name })),
      error: () => this.notify.error('Could not load medicine types')
    });
  }
  onMedicineTypeSelect(item: LocalPharmacistItem) {
    if (!item || !item.medicineFormId) {
      this.medicinesByType = [];
      if (item) { item.medicineId = null; item.dosage = ''; }
      return;
    }
    this._medicineMasterService.getMedicinesByFormId(item.medicineFormId).subscribe({
      next: (res: any) => {
        this.medicinesByType = (res || []).map((x: any) => ({ id: x.id, medicineName: x.medicineName, dosageOption: x.dosageOption }));
        item.medicineId = null;
        item.dosage = '';
      },
      error: (err) => {
        this.notify.error('Could not load medicines for selected type');
        this.medicinesByType = [];
        item.medicineId = null;
        item.dosage = '';
      }
    });
  }
  onMedicineSelect(item: LocalPharmacistItem) {
    if (item.medicineId && this.isMedicineDisabled({ id: item.medicineId })) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Medicine Already Added',
        detail: 'This medicine is already included from the selected prescription. Please select another medicine.'
      });
      item.medicineId = null;
      item.medicineName = '';
      item.dosage = '';
      item.unitPrice = 0;
      this.cdRef.detectChanges();
      return;
    }
    const selected = this.medicinesByType.find(m => m.id === item.medicineId);
    if (selected) {
      item.medicineName = selected.medicineName;
      item.dosage = selected.dosageOption;
      this.fetchBatchesForMedicine(item.medicineId as number, () => {
        const best = this.pickBestBatch(this.medicineBatches[item.medicineId as number] || []);
        if (best) item.unitPrice = best.sellingPrice ?? item.unitPrice ?? 0;
      });
    } else {
      item.medicineName = '';
      item.dosage = '';
      item.unitPrice = 0;
    }
  }
  isMedicineDisabled(medicine: any): boolean {
    return this.disabledMedicineIds.has(medicine.id);
  }
  private pickBestBatch(stocks: any[]): any | null {
    if (!stocks || !stocks.length) return null;
    const available = stocks.filter(s => (s.quantity || 0) > 0 && !s.isExpire);
    if (available.length) {
      available.sort((a, b) => (a.daysToExpire ?? Number.MAX_SAFE_INTEGER) - (b.daysToExpire ?? Number.MAX_SAFE_INTEGER));
      return available[0];
    }
    const anyAvailable = stocks.filter(s => (s.quantity || 0) > 0);
    if (anyAvailable.length) {
      anyAvailable.sort((a, b) => (a.daysToExpire ?? Number.MAX_SAFE_INTEGER) - (b.daysToExpire ?? Number.MAX_SAFE_INTEGER));
      return anyAvailable[0];
    }
    return null;
  }
  private fetchBatchesForMedicine(medicineMasterId: number, done?: () => void) {
    if (!medicineMasterId) { if (done) done(); return; }
    if (this.medicineBatches[medicineMasterId]) { if (done) done(); return; }

    this._medicineMasterService.getMedicineWithStockById(medicineMasterId).subscribe({
      next: (res: any) => {
        const dto = (res && res.result) ? res.result : res;
        const stocks = (dto?.stocks || []).map((s: any) => ({
          ...s,
          expiryDateObj: s.expiryDate ? new Date(s.expiryDate) : null
        }));

        stocks.sort((a: any, b: any) => (a.daysToExpire ?? Number.MAX_SAFE_INTEGER) - (b.daysToExpire ?? Number.MAX_SAFE_INTEGER));
        this.medicineBatches[medicineMasterId] = stocks;
        const totalAvailable = stocks.reduce((sum: number, s: any) => sum + (s.quantity || 0), 0);
        this.medicineStocks[medicineMasterId] = totalAvailable;
        if (done) done();
      },
      error: (err) => {
        this._pharmacistInventoryService.get(medicineMasterId).subscribe({
          next: (mres: any) => {
            const pseudo = [{
              id: -medicineMasterId,
              batchNo: 'N/A',
              expiryDate: null,
              quantity: mres.stock || 0,
              sellingPrice: mres.sellingPrice || 0,
              isExpire: false,
              daysToExpire: Number.MAX_SAFE_INTEGER
            }];
            this.medicineBatches[medicineMasterId] = pseudo;
            this.medicineStocks[medicineMasterId] = mres.stock || 0;
            if (done) done();
          },
          error: () => {
            this.medicineBatches[medicineMasterId] = [];
            this.medicineStocks[medicineMasterId] = 0;
            if (done) done();
          }
        });
      }
    });
  }

  private allocateAcrossBatches(medicineId: number, requestedQty: number): LocalPharmacistItem[] {
    const allocatedItems: LocalPharmacistItem[] = [];
    let remainingQty = requestedQty;

    const batches = this.medicineBatches[medicineId] || [];

    for (const batch of batches) {
      if (remainingQty <= 0) break;

      // Skip expired batches
      if (batch.isExpire) continue;

      const allocatedForBatch = this.getAllocatedForBatch(medicineId, batch.id);
      const availableInBatch = Math.max(0, (batch.quantity || 0) - allocatedForBatch);

      if (availableInBatch <= 0) continue;

      const take = Math.min(availableInBatch, remainingQty);

      allocatedItems.push({
        batchId: batch.id,
        batchNo: batch.batchNo,
        expiryDate: batch.expiryDate,
        qty: take,
        unitPrice: batch.sellingPrice
      } as LocalPharmacistItem);

      this.changeAllocatedForBatch(medicineId, batch.id, take);
      remainingQty -= take;
    }

    return allocatedItems;
  }

  private getAllocatedForBatch(medicineId: number, batchId: number): number {
    return (this.batchAllocations[medicineId] && this.batchAllocations[medicineId][batchId]) || 0;
  }

  private changeAllocatedForBatch(medicineId: number, batchId: number, delta: number): void {
    if (!this.batchAllocations[medicineId]) {
      this.batchAllocations[medicineId] = {};
    }

    const current = this.getAllocatedForBatch(medicineId, batchId);
    const newValue = current + delta;

    if (newValue <= 0) {
      delete this.batchAllocations[medicineId][batchId];
    } else {
      this.batchAllocations[medicineId][batchId] = newValue;
    }
  }
  private getMaxPossibleQtyForCurrentBatch(itm: LocalPharmacistItem, medId: number): number {
    if (!itm.batchId) return 0;

    const batch = this.medicineBatches[medId]?.find((b: any) => b.id === itm.batchId);
    if (!batch) return 0;

    const batchTotalQuantity = batch.quantity || 0;
    const allocatedForThisBatch = this.getAllocatedForBatch(medId, itm.batchId) || 0;
    const availableInBatch = Math.max(0, batchTotalQuantity - allocatedForThisBatch);
    const maxPossible = (itm.qty || 0) + availableInBatch;
    return maxPossible;
  }
  canSplitToNewBatch(item: LocalPharmacistItem): boolean {
    if (!item.medicineId || !item.batchId) return false;

    const currentBatch = this.medicineBatches[item.medicineId]?.find(b => b.id === item.batchId);
    if (!currentBatch) return false;

    // Check if current batch is at maximum capacity
    const currentQty = item.qty || 0;
    const batchTotalQty = currentBatch.quantity || 0;
    const isAtBatchLimit = currentQty >= batchTotalQty;

    if (!isAtBatchLimit) return false;

    // Check if other batches are available
    const otherBatches = (this.medicineBatches[item.medicineId] || [])
      .filter(b => {
        if (b.id === item.batchId) return false;
        if (b.isExpire) return false;

        const allocated = this.getAllocatedForBatch(item.medicineId, b.id);
        const remaining = Math.max(0, (b.quantity || 0) - allocated);
        return remaining > 0;
      });

    return otherBatches.length > 0;
  }

  splitToNewBatch(item: LocalPharmacistItem): void {
    if (!item.medicineId) return;

    const otherBatches = (this.medicineBatches[item.medicineId] || [])
      .filter(b => {
        if (b.id === item.batchId) return false;
        if (b.isExpire) return false;

        const allocated = this.getAllocatedForBatch(item.medicineId, b.id);
        const remaining = Math.max(0, (b.quantity || 0) - allocated);
        return remaining > 0;
      })
      .sort((a, b) => a.daysToExpire - b.daysToExpire);

    if (otherBatches.length === 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'No Batches Available',
        detail: 'No other batches available with stock'
      });
      return;
    }

    const bestBatch = otherBatches[0];
    const allocated = this.getAllocatedForBatch(item.medicineId, bestBatch.id);
    const availableInBatch = Math.max(0, (bestBatch.quantity || 0) - allocated);

    if (availableInBatch <= 0) return;

    const newItem: LocalPharmacistItem = {
      ...this.createItemTemplate(item),
      qty: 1,
      unitPrice: bestBatch.sellingPrice,
      batchId: bestBatch.id,
      batchNo: bestBatch.batchNo,
      expiryDate: bestBatch.expiryDate,
      _isNew: true,
      _rowId: this.nextRowId()
    };

    this.changeAllocatedForBatch(item.medicineId, bestBatch.id, 1);
    this.selectedPrescriptionItem.push(newItem);
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];

    this.messageService.add({
      severity: 'success',
      summary: 'Batch Added',
      detail: `1 unit added from batch ${bestBatch.batchNo}`
    });

    this.updateUI();
  }

  getStockForItem(itm: LocalPharmacistItem) {
    const mid = itm.medicineId as number;
    if (!mid) return '-';

    const total = this.medicineStocks[mid];
    if (total === undefined || total === null) return '-';

    const allocated = Object.values(this.batchAllocations[mid] || {}).reduce((a, v) => a + v, 0);
    const remaining = Math.max(0, total - allocated);

    if (itm.batchId) {
      const batch = this.medicineBatches[mid]?.find(b => b.id === itm.batchId);
      if (batch) {
        const batchAllocated = this.getAllocatedForBatch(mid, itm.batchId);
        const batchRemaining = Math.max(0, (batch.quantity || 0) - batchAllocated);
        return `Batch Stock: ${batchRemaining} | Total: ${remaining}/${total}`;
      }
    }

    return `${remaining} / ${total}`;
  }

  getRemainingStock(itm: LocalPharmacistItem): number {
    const mid = itm.medicineId as number;
    if (!mid) return 0;
    const total = this.medicineStocks[mid] || 0;
    const allocated = Object.values(this.batchAllocations[mid] || {}).reduce((a, v) => a + v, 0);
    return Math.max(0, total - allocated);
  }

  getAvailableStockForMedicine(medicineId: number): number {
    if (!medicineId) return 0;
    return this.medicineStocks[medicineId] || 0;
  }


  GetDetailsByID() {
    this.pharmacistPrescriptionService.getPharmacistPrescriptionsById(this.id).subscribe({
      next: (res) => {
        setTimeout(() => {
          if (res.prescriptionId) {
            this.selectedPrescriptionID = res.prescriptionId;
            this._pharmacyNotes = res.pharmacyNotes;
            this.ispaid = res.isPaid;
            this.receiptNumber = res.receiptNumber;

            // Initialize batch allocations
            this.batchAllocations = {};

            this.selectedPrescriptionItem = (res.prescriptionItem || []).map((it: any) => {
              if (!it.qty || it.qty < 1) it.qty = 1;
              it._rowId = this.nextRowId();
              res.prescriptionItem.forEach(item => {
                if (item.medicineId) {
                  this.disabledMedicineIds.add(item.medicineId);
                }
              });

              // Fetch batches for each medicine
              if (it.medicineId) {
                this.fetchBatchesForMedicine(it.medicineId, () => {
                  // Allocate quantity to batches
                  if (it.batchId) {
                    this.changeAllocatedForBatch(it.medicineId, it.batchId, it.qty);
                  } else {
                    // Auto-allocate if no batch specified
                    this.autoAllocateToBatches(it);
                  }
                });
              }

              return it;
            });

            this.selectedPrescriptionName = `${res.patientName || ''} - ${res.issueDate.toDate().toLocaleDateString()} - ${res.prescriptionId}`;
            this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
            this.total = this.getPrescriptionTotal();
          }
          this.cdRef.detectChanges();
        });
      },
      error: (err) => {
        this.notify.error('Could not load prescription details');
      }
    });
  }

  /**
   * Auto-allocate item to batches using FIFO
   */
  private autoAllocateToBatches(item: LocalPharmacistItem): void {
    if (!item.medicineId) return;

    const allocatedBatches = this.allocateAcrossBatches(item.medicineId, item.qty || 1);

    if (allocatedBatches.length > 0) {
      const firstBatch = allocatedBatches[0];
      item.batchId = firstBatch.batchId;
      item.batchNo = firstBatch.batchNo;
      item.expiryDate = firstBatch.expiryDate;
      item.unitPrice = firstBatch.unitPrice;

      if (allocatedBatches.length > 1) {
        // Create additional rows for other batches
        for (let i = 1; i < allocatedBatches.length; i++) {
          const batch = allocatedBatches[i];
          const newItem: LocalPharmacistItem = {
            ...this.createItemTemplate(item),
            qty: batch.qty,
            unitPrice: batch.unitPrice,
            batchId: batch.batchId,
            batchNo: batch.batchNo,
            expiryDate: batch.expiryDate,
            _isNew: false,
            _rowId: this.nextRowId()
          };
          this.selectedPrescriptionItem.push(newItem);
        }
      }
    }
  }

  onQtyChange(itm: LocalPharmacistItem, newQty: any) {
    newQty = Number(newQty) || 1;
    if (newQty < 1) newQty = 1;
    const medId = itm.medicineId as number;
    const oldQty = Number(itm.qty) || 1;
    if (newQty === oldQty) {
      return;
    }
    if (newQty < oldQty) {
      itm.qty = newQty;
      this.handleQtyDecrease(itm, oldQty, newQty);
      return;
    }
    const currentBatchMaxQty = this.getMaxPossibleQtyForCurrentBatch(itm, medId);
    if (newQty <= currentBatchMaxQty) {

      itm.qty = newQty;
      this.handleQtyIncreaseWithinBatch(itm, oldQty, newQty);
      return;
    }

    this.messageService.add({
      severity: 'warn',
      summary: 'Batch Limit',
      detail: `Batch ${itm.batchNo} has only ${currentBatchMaxQty} units available.`
    });
    itm.qty = currentBatchMaxQty;
    this.updateUI();
  }
  private handleQtyIncreaseWithinBatch(itm: LocalPharmacistItem, oldQty: number, newQty: number) {
    const delta = newQty - oldQty;
    const medId = itm.medicineId as number;

    if (itm.batchId) {
      this.changeAllocatedForBatch(medId, itm.batchId, delta);

    }
    this.updateUI();
  }

  private handleQtyDecrease(itm: LocalPharmacistItem, oldQty: number, newQty: number) {
    const decreaseAmount = oldQty - newQty;
    const medId = itm.medicineId as number;


    if (itm.batchId) {
      this.changeAllocatedForBatch(medId, itm.batchId, -decreaseAmount);
    }
    if (newQty <= 0) {
      const idx = this.selectedPrescriptionItem.findIndex(x => x._rowId === itm._rowId);
      if (idx >= 0) {
        this.selectedPrescriptionItem.splice(idx, 1);
      }
    }
    this.updateUI();

  }

  NewItem(): void {
    this.loadMedicines();
    const item: LocalPharmacistItem = {
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
      durationValue: 1,
      durationUnit: 'Days',
      _isNew: true,
      _rowId: this.nextRowId()
    };
    this.newPrescriptionItem = item;
  }

  addItem() {
    if (!this.newPrescriptionItem) return;
    const medId = this.newPrescriptionItem.medicineId as number;

    if (medId && this.isMedicineDisabled({ id: medId })) {
      this.messageService.add({
        severity: 'error',
        summary: 'Invalid Selection',
        detail: 'Cannot add medicine that is already in prescription.'
      });
      return;
    }
    if (!medId) {
      this.messageService.add({ severity: 'warn', summary: 'Select medicine', detail: 'Please select a medicine.' });
      return;
    }
    if (this.newPrescriptionItem.durationValue && this.newPrescriptionItem.durationUnit) {
      this.newPrescriptionItem.duration = `${this.newPrescriptionItem.durationValue} ${this.newPrescriptionItem.durationUnit}`;
    }
    this.fetchBatchesForMedicine(medId, () => {
      const totalAvailable = this.medicineStocks[medId] || 0;
      let requested = Number(this.newPrescriptionItem!.qty) || 1;
      if (requested < 1) requested = 1;

      const alreadyAllocatedTotalForMed = Object.values(this.batchAllocations[medId] || {}).reduce((s, v) => s + v, 0);
      const remainingGlobal = Math.max(0, totalAvailable - alreadyAllocatedTotalForMed);
      if (remainingGlobal <= 0) {
        this.messageService.add({ severity: 'warn', summary: 'Out of stock', detail: `No stock left for ${this.newPrescriptionItem!.medicineName}` });
        return;
      }
      if (requested > remainingGlobal) {
        this.messageService.add({ severity: 'warn', summary: 'Stock Limit', detail: `Only ${remainingGlobal} units available for ${this.newPrescriptionItem!.medicineName}.` });
        requested = remainingGlobal;
      }

      const createdRows = this.allocateAcrossBatchesAndCreateRows(this.newPrescriptionItem!, requested);
      if (createdRows.length) {
        this.disabledMedicineIds.add(medId);
        createdRows.forEach(r => this.selectedPrescriptionItem.push(r));
        this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
        this.total = this.getPrescriptionTotal();

        this.messageService.add({
          severity: 'success',
          summary: 'Medicine Added',
          detail: `${this.newPrescriptionItem.medicineName} added successfully and disabled for future selection`
        });
      }

      this.newPrescriptionItem = null;
      this.cdRef.detectChanges();
    });
  }

  private allocateAcrossBatchesAndCreateRows(templateItem: LocalPharmacistItem, requestedQty: number): LocalPharmacistItem[] {
    const rows: LocalPharmacistItem[] = [];
    const medId = templateItem.medicineId as number;

    if (!medId || requestedQty <= 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Invalid Request',
        detail: 'Medicine not selected or invalid quantity'
      });
      return rows;
    }

    const batches = this.medicineBatches[medId] || [];
    let remaining = requestedQty;


    const sortedBatches = [...batches].sort((a, b) => {
      const aExpiry = a.daysToExpire ?? Number.MAX_SAFE_INTEGER;
      const bExpiry = b.daysToExpire ?? Number.MAX_SAFE_INTEGER;
      return aExpiry - bExpiry;
    });

    for (const batch of sortedBatches) {
      if (remaining <= 0) break;

      const batchId = batch.id;
      const allocatedForBatch = this.getAllocatedForBatch(medId, batchId);
      const batchAvailable = Math.max(0, (batch.quantity || 0) - allocatedForBatch);

      if (batchAvailable <= 0) continue;

      const take = Math.min(batchAvailable, remaining);

      const row: LocalPharmacistItem = {
        ...this.createItemTemplate(templateItem),
        qty: take,
        unitPrice: batch.sellingPrice ?? (templateItem.unitPrice ?? 0),
        batchId: batchId,
        batchNo: batch.batchNo,
        expiryDate: batch.expiryDate ? new Date(batch.expiryDate) : null,
        _isNew: true,
        _rowId: this.nextRowId()
      };

      rows.push(row);
      this.changeAllocatedForBatch(medId, batchId, take);
      remaining -= take;
    }


    if (remaining > 0) {
      const allocated = requestedQty - remaining;
      this.messageService.add({
        severity: 'warn',
        summary: 'Insufficient Stock',
        detail: `Only ${allocated} out of ${requestedQty} units allocated for ${templateItem.medicineName}.`
      });
    }

    return rows;
  }

  removeItem(index: number): void {
    const itm = this.selectedPrescriptionItem[index];
    if (itm) {
      const medicineId = itm.medicineId as number;
      if (medicineId) {
        const otherItemsWithSameMedicine = this.selectedPrescriptionItem.filter(
          (item, idx) => idx !== index && item.medicineId === medicineId
        );
        if (otherItemsWithSameMedicine.length === 0) {
          this.disabledMedicineIds.delete(medicineId);
        }
      }
      if (itm.batchId) {
        this.changeAllocatedForBatch(medicineId, itm.batchId as number, -(itm.qty || 0));
      }
    }
    this.selectedPrescriptionItem.splice(index, 1);
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
    this.total = this.getPrescriptionTotal();
  }

  // HELPER METHODS

  private createItemTemplate(item: LocalPharmacistItem): LocalPharmacistItem {
    return {
      prescriptionId: null,
      pharmacistPrescriptionId: null,
      medicineId: item.medicineId,
      medicineName: item.medicineName,
      dosage: item.dosage,
      frequency: item.frequency,
      duration: item.duration,
      instructions: item.instructions,
      isPrescribe: item.isPrescribe,
      durationValue: item.durationValue,
      durationUnit: item.durationUnit,
      _isNew: true
    } as LocalPharmacistItem;
  }

  private updateUI(): void {
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
    this.total = this.getPrescriptionTotal();
    this.cdRef.detectChanges();
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

          // Pre-fetch batches for all medicines
          res.items.forEach(medicine => {
            const units = medicine.unit ? medicine.unit.split(',').map((u: string) => u.trim()) : [];
            this.medicineDosageOptions[medicine.medicineName] = units;
            this.selectedMedicineUnits[medicine.medicineName] = units.length ? units[0] : '';

            // Fetch batches for this medicine
            this.fetchBatchesForMedicine(medicine.id);
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

  onMedicineChange(item: LocalPharmacistItem) {
    const selected = this.medicineOptions.find(m => m.value === item.medicineId);
    if (selected) {
      item.medicineName = selected.name;
      if (this.medicineDosageOptions[selected.name]) {
        item.dosage = this.selectedMedicineUnits[selected.name];
      } else {
        item.dosage = '';
      }
    }

    if (item.medicineId && item.medicineId > 0) {
      this.GetPriceOfMedicine(item.medicineId);

      // Fetch batches for the selected medicine
      this.fetchBatchesForMedicine(item.medicineId, () => {
        // Auto-select the best batch price
        const batches = this.medicineBatches[item.medicineId!] || [];
        const bestBatch = batches.find(b => !b.isExpire) || batches[0];
        if (bestBatch) {
          item.unitPrice = bestBatch.sellingPrice;
        }
      });
    }
  }

  GetPriceOfMedicine(_medicineId: number) {
    this._pharmacistInventoryService.get(_medicineId).subscribe({
      next: (res) => {
        if (this.newPrescriptionItem) {
          this.newPrescriptionItem.unitPrice = res.sellingPrice;
        }
        this.medicineStocks[_medicineId] = res.stock || 0;
      },
      error: () => { }
    });
  }

  getPrescriptionTotal(): number {
    if (!this.selectedPrescriptionItem || !this.selectedPrescriptionItem.length) return 0;
    return this.selectedPrescriptionItem
      .map(itm => (itm.unitPrice || 0) * (itm.qty || 0))
      .reduce((sum, val) => sum + val, 0);
  }

  save() {
    if (!this.createPharmacistPrescriptionForm.valid) return;

    // Validate stock before saving
    const stockIssues = this.selectedPrescriptionItem.filter(item => {
      const availableStock = this.getAvailableStockForMedicine(item.medicineId!);
      return (item.qty || 0) > availableStock;
    });

    if (stockIssues.length > 0) {
      this.messageService.add({
        severity: 'error',
        summary: 'Stock Issues',
        detail: 'Some items have insufficient stock. Please adjust quantities.'
      });
      return;
    }

    this.isSaving = true;
    const input = new CreateUpdatePharmacistPrescriptionsDto();
    input.id = this.id;
    input.tenantId = abp.session.tenantId;
    input.prescriptionId = this.selectedPrescriptionID;
    input.issueDate = moment();
    input.pharmacyNotes = this._pharmacyNotes;
    input.isPaid = this.ispaid;
    input.receiptNumber = this.receiptNumber;
    input.collectionStatus = CollectionStatus._0;
    input.grandTotal = this.getPrescriptionTotal();

    const resBody: any = {
      pharmacistPrescriptionsDto: input,
      pharmacistPrescriptionsListOfItem: this.selectedPrescriptionItem.map(item => {
        const copy = { ...item } as any;
        delete copy._rowId;
        delete copy._isNew;
        delete copy.durationValue;
        delete copy.durationUnit;
        return copy;
      }),
    }
    debugger
    this.pharmacistPrescriptionService.createPharmacistPrescriptionsWithItem(resBody).subscribe({
      next: () => {
        this.notify.success('Updated successfully!');
        this.onSave.emit();
        this.bsModalRef.hide();
      },
      error: () => this.isSaving = false
    });
  }
}