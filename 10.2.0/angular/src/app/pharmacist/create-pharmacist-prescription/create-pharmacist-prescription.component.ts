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

/**
 * Local UI item type — use Partial so plain server objects or newly created objects both can be assigned.
 * Add UI-only props here (durationValue/durationUnit/_rowId/_isNew/batchId etc).
 */
interface LocalPharmacistItem extends Partial<PharmacistPrescriptionItemWithUnitPriceDto> {
  _rowId?: number;
  _isNew?: boolean;
  batchId?: number;
  batchNo?: string;
  expiryDate?: Date | null;
  durationValue?: number;
  durationUnit?: string;
}

@Component({
  selector: 'app-create-pharmacist-prescription',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, ToastModule, AbpModalFooterComponent, MultiSelectModule, FormsModule, SelectModule,
    CommonModule, ButtonModule, TextareaModule, InputNumberModule, TableModule, DropdownModule, InputTextModule
  ],
  templateUrl: './create-pharmacist-prescription.component.html',
  styleUrl: './create-pharmacist-prescription.component.css',
  providers: [PatientServiceProxy, MedicineMasterServiceProxy, MedicineFormMasterServiceProxy, PrescriptionItemsServiceProxy, PharmacistInventoryServiceProxy, MessageService, PrescriptionServiceProxy, PharmacistPrescriptionsServiceProxy]
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

  // Use LocalPharmacistItem
  selectedPrescriptionItem: LocalPharmacistItem[] = [];
  newPrescriptionItem!: LocalPharmacistItem | null;
  selectedPrescriptionID!: number;
  _pharmacyNotes!: string;
  total: number = 0;

  // caches & helpers
  medicineTypes: any[] = [];
  medicinesByType: any[] = [];
  medicineBatches: { [medicineId: number]: any[] } = {}; // cached batches from getMedicineWithStockById
  medicineStocks: { [medicineId: number]: number } = {}; // total available count
  batchAllocations: { [medicineId: number]: { [batchId: number]: number } } = {}; // allocated qty per batch

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
    private _medicineFormService: MedicineFormMasterServiceProxy,
    private _medicineMasterService: MedicineMasterServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.loadPatients();
    this.loadMedicineTypes();
  }

  /* ========== Medicine types & meds ========== */
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
    }
  }

  /**
   * Fetch and cache batch list and total for a medicine using MedicineMaster service
   */
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
        // sort using daysToExpire ascending
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
  /* ========== allocation helpers ========== */
  private getAllocatedForBatch(medicineId: number, batchId: number): number {
    return (this.batchAllocations[medicineId] && this.batchAllocations[medicineId][batchId]) || 0;
  }

  private changeAllocatedForBatch(medicineId: number, batchId: number, delta: number) {
    if (!this.batchAllocations[medicineId]) this.batchAllocations[medicineId] = {};
    const cur = this.batchAllocations[medicineId][batchId] || 0;
    const next = cur + delta;
    if (next <= 0) {
      delete this.batchAllocations[medicineId][batchId];
    } else {
      this.batchAllocations[medicineId][batchId] = next;
    }
  }

  /**
   * Allocate requestedQty across available batches and create rows for UI.
   * Returns array of LocalPharmacistItem rows.
   */
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

    // Sort batches by expiry (FIFO)
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

    // If we couldn't allocate full quantity, show warning
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

  /* ========== New item flow ========== */
  NewItem(): void {
    const dto = new PharmacistPrescriptionItemWithUnitPriceDto();
    dto.init({
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
    const item: LocalPharmacistItem = dto as LocalPharmacistItem;
    item.durationValue = 1;
    item.durationUnit = 'Days';
    item._isNew = true;
    item._rowId = this.nextRowId();
    this.newPrescriptionItem = item;
  }

  addItem() {
    if (!this.newPrescriptionItem) return;
    const medId = this.newPrescriptionItem.medicineId as number;
    if (!medId) {
      this.messageService.add({ severity: 'warn', summary: 'Select medicine', detail: 'Please select a medicine.' });
      return;
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
        createdRows.forEach(r => this.selectedPrescriptionItem.push(r));
        this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
        this.total = this.getPrescriptionTotal();
      }

      this.newPrescriptionItem = null;
      this.cdRef.detectChanges();
    });
  }


  getRowMaxQty(itm: LocalPharmacistItem): number {
    const currentQty = Number(itm?.qty || 0);

    if (!itm || !itm.medicineId) return currentQty;

    const medId = itm.medicineId as number;

    // If this row already has a batch assigned -> compute batch-limited max
    if (itm.batchId) {
      const batch = (this.medicineBatches[medId] || []).find((b: any) => b.id === itm.batchId);
      const batchQty = batch ? (batch.quantity || 0) : 0;

      // allocated for this batch includes this row's qty, so subtract currentQty to get "other allocated"
      const allocatedIncludingThis = this.getAllocatedForBatch(medId, itm.batchId as number);
      const otherAllocated = Math.max(0, allocatedIncludingThis - currentQty);

      // available in this batch for increasing THIS row:
      const availableInThisBatch = Math.max(0, batchQty - otherAllocated);

      // absolute max for this row = currentQty + availableInThisBatch
      return currentQty + availableInThisBatch;
    }

    // If row has no batchId (template/unassigned), limit by global remaining
    const total = this.medicineStocks[medId] || 0;
    const allocated = Object.values(this.batchAllocations[medId] || {}).reduce((s, v) => s + v, 0);
    const remainingGlobal = Math.max(0, total - allocated);

    return currentQty + remainingGlobal;
  }
  private getMaxPossibleQtyForCurrentBatch(itm: LocalPharmacistItem, medId: number): number {
  if (!itm.batchId) return 0;

  const batch = this.medicineBatches[medId]?.find((b: any) => b.id === itm.batchId);
  if (!batch) return 0;

  const batchTotalQuantity = batch.quantity || 0;
  
  // Calculate allocated quantity for this batch (including current item's quantity)
  const allocatedForThisBatch = this.getAllocatedForBatch(medId, itm.batchId) || 0;
  
  // Available quantity in this batch = total - allocated
  const availableInBatch = Math.max(0, batchTotalQuantity - allocatedForThisBatch);
  
  // Maximum possible = current quantity + available in batch
  const maxPossible = (itm.qty || 0) + availableInBatch;
  
  
  return maxPossible;
}
 splitToNewBatch(itm: LocalPharmacistItem) {
  const medId = itm.medicineId as number;
  if (!medId) return;

  // Ensure batches are loaded
  if (!this.medicineBatches[medId]) {
    this.fetchBatchesForMedicine(medId, () => {
      this.splitToNewBatch(itm);
    });
    return;
  }

  // Find available batches (excluding current batch)
  const availableBatches = (this.medicineBatches[medId] || [])
    .filter(b => {
      const isDifferentBatch = b.id !== itm.batchId;
      const hasStock = (b.quantity || 0) > 0;
      const allocated = this.getAllocatedForBatch(medId, b.id) || 0;
      const remaining = Math.max(0, (b.quantity || 0) - allocated);
      return isDifferentBatch && hasStock && remaining > 0;
    })
    .sort((a, b) => (a.daysToExpire ?? Number.MAX_SAFE_INTEGER) - (b.daysToExpire ?? Number.MAX_SAFE_INTEGER));

  if (availableBatches.length === 0) {
    this.messageService.add({
      severity: 'warn',
      summary: 'No Batches Available',
      detail: 'No other batches available with stock for this medicine.'
    });
    return;
  }

  // Pick the best batch (earliest expiry)
  const bestBatch = availableBatches[0];
  const allocated = this.getAllocatedForBatch(medId, bestBatch.id) || 0;
  const availableInBatch = Math.max(0, (bestBatch.quantity || 0) - allocated);

  if (availableInBatch <= 0) return;

  // Create new item for the split
  const newItem: LocalPharmacistItem = {
    prescriptionId: itm.prescriptionId,
    pharmacistPrescriptionId: itm.pharmacistPrescriptionId,
    medicineId: itm.medicineId,
    medicineName: itm.medicineName,
    dosage: itm.dosage,
    frequency: itm.frequency,
    duration: itm.duration,
    instructions: itm.instructions,
    isPrescribe: itm.isPrescribe ?? false,
    medicineFormId: itm.medicineFormId,
    
    qty: 1,
    unitPrice: bestBatch.sellingPrice ?? (itm.unitPrice ?? 0),
    totalPayableAmount: (bestBatch.sellingPrice ?? (itm.unitPrice ?? 0)) * 1,
    batchId: bestBatch.id,
    batchNo: bestBatch.batchNo,
    expiryDate: bestBatch.expiryDate ? new Date(bestBatch.expiryDate) : null,
    
    _isNew: true,
    _rowId: this.nextRowId()
  };

  // Allocate the quantity to the new batch
  this.changeAllocatedForBatch(medId, bestBatch.id, 1);

  // Add the new item to the list
  this.selectedPrescriptionItem.push(newItem);
  this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];

  this.messageService.add({
    severity: 'success',
    summary: 'Batch Added',
    detail: `1 unit added from batch ${bestBatch.batchNo}`
  });

  this.updateUI();
}

  private autoSplitToNewBatch(itm: LocalPharmacistItem) {
    const medId = itm.medicineId as number;
    const currentQty = itm.qty || 0;

    // Find the best batch (earliest expiry)
    const bestBatch = this.pickBestBatch(
      (this.medicineBatches[medId] || [])
        .filter(b => b.id !== itm.batchId && (b.quantity || 0) > 0)
    );

    if (!bestBatch) {
      this.messageService.add({
        severity: 'warn',
        summary: 'No Stock Available',
        detail: 'No additional stock available in other batches.'
      });
      return;
    }

    const batchAvailable = Math.max(0, (bestBatch.quantity || 0) -
      (this.getAllocatedForBatch(medId, bestBatch.id) || 0));

    if (batchAvailable <= 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Batch Full',
        detail: 'No more stock available for this medicine.'
      });
      return;
    }

    // Create new row for the split quantity
    const splitQty = Math.min(1, batchAvailable); // Start with 1 unit in new batch

    const newRow: LocalPharmacistItem = {
      ...this.createItemTemplate(itm),
      qty: splitQty,
      unitPrice: bestBatch.sellingPrice ?? (itm.unitPrice ?? 0),
      batchId: bestBatch.id,
      batchNo: bestBatch.batchNo,
      expiryDate: bestBatch.expiryDate ? new Date(bestBatch.expiryDate) : null,
      _isNew: true,
      _rowId: this.nextRowId()
    };

    // Update allocations
    this.changeAllocatedForBatch(medId, bestBatch.id, splitQty);

    // Add new row
    this.selectedPrescriptionItem.push(newRow);
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];

    this.messageService.add({
      severity: 'success',
      summary: 'Batch Split',
      detail: `1 unit moved to batch ${bestBatch.batchNo}`
    });

    this.updateUI();
  }
  canSplitToNewBatch(itm: LocalPharmacistItem): boolean {
  try {
    const medId = itm.medicineId as number;
    if (!medId || !itm.batchId) return false;

    // Get the current batch
    const currentBatch = this.medicineBatches[medId]?.find((b: any) => b.id === itm.batchId);
    if (!currentBatch) return false;

    const batchTotalQty = currentBatch.quantity || 0;
    const currentQty = itm.qty || 0;

    // Check if current quantity has reached or exceeded batch total
    const isAtBatchLimit = currentQty >= batchTotalQty;
    
    if (!isAtBatchLimit) {
      return false;
    }

    // Check if other batches are available
    const otherBatches = (this.medicineBatches[medId] || [])
      .filter(b => {
        if (b.id === itm.batchId) return false; // Exclude current batch
        if ((b.quantity || 0) <= 0) return false; // Exclude empty batches
        
        // Check if batch has available stock considering allocations
        const allocated = this.getAllocatedForBatch(medId, b.id) || 0;
        const remaining = Math.max(0, (b.quantity || 0) - allocated);
        return remaining > 0;
      });

    console.log('📦 Other batches available:', otherBatches.length);

    const result = otherBatches.length > 0;
    console.log('🎯 Split button should show:', result);
    
    return result;
  } catch (error) {
    console.error('Error in canSplitToNewBatch:', error);
    return false;
  }
}
getStockForItem(itm: LocalPharmacistItem) {
  const mid = itm.medicineId as number;
  if (!mid) return '-';
  
  const total = this.medicineStocks[mid];
  if (total === undefined || total === null) return '-';
  
  const allocated = Object.values(this.batchAllocations[mid] || {}).reduce((a, v) => a + v, 0);
  const remaining = Math.max(0, total - allocated);
  
  // Show current batch info if available
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
  onQtyChange(itm: LocalPharmacistItem, newQty: any) {
  console.log('🚀 onQtyChange called', { 
    medicine: itm.medicineName, 
    oldQty: itm.qty, 
    newQty: newQty,
    batchId: itm.batchId 
  });
  
  // Convert and validate new quantity
  newQty = Number(newQty) || 1;
  if (newQty < 1) newQty = 1;

  const medId = itm.medicineId as number;
  const oldQty = Number(itm.qty) || 1;
  
  // ✅ No actual change
  if (newQty === oldQty) {
    console.log('✅ No change in quantity');
    return;
  }

  // 🚨 IMMEDIATE VALIDATION: Check if decrease
  if (newQty < oldQty) {
    console.log('🔻 Decreasing quantity from', oldQty, 'to', newQty);
    itm.qty = newQty; // Immediate UI update
    this.handleQtyDecrease(itm, oldQty, newQty);
    return;
  }

  // 🚨 For increase: First check current batch limits
  const currentBatchMaxQty = this.getMaxPossibleQtyForCurrentBatch(itm, medId);
  
  // ✅ If within current batch limits - allow immediately
  if (newQty <= currentBatchMaxQty) {
    console.log('✅ Within current batch limits');
    itm.qty = newQty;
    this.handleQtyIncreaseWithinBatch(itm, oldQty, newQty);
    return;
  }

  // ❌ If exceeds current batch limits
  console.log('❌ Exceeds current batch limits');
  
  // Show warning message
  this.messageService.add({
    severity: 'warn',
    summary: 'Batch Limit',
    detail: `Batch ${itm.batchNo} has only ${currentBatchMaxQty} units available.`
  });
  
  // Reset to current batch maximum
  itm.qty = currentBatchMaxQty;
  this.updateUI();
}
private handleQtyIncreaseWithinBatch(itm: LocalPharmacistItem, oldQty: number, newQty: number) {
  const delta = newQty - oldQty;
  const medId = itm.medicineId as number;
  
  console.log('📈 Handling quantity increase within batch:', {
    medicine: itm.medicineName,
    delta: delta
  });

  if (itm.batchId) {
    this.changeAllocatedForBatch(medId, itm.batchId, delta);
    console.log('📊 Increased allocation for batch:', itm.batchId);
  }
  
  // Force UI update to show/hide split button
  this.updateUI();
}

  private handleQtyDecrease(itm: LocalPharmacistItem, oldQty: number, newQty: number) {
  const decreaseAmount = oldQty - newQty;
  const medId = itm.medicineId as number;

  console.log('🔻 Handling quantity decrease:', {
    medicine: itm.medicineName,
    oldQty: oldQty,
    newQty: newQty,
    decreaseAmount: decreaseAmount
  });

  if (itm.batchId) {
    // Decrease allocation for the batch
    this.changeAllocatedForBatch(medId, itm.batchId, -decreaseAmount);
    console.log('📉 Decreased allocation for batch:', itm.batchId);
  }

  // Remove item if quantity becomes zero
  if (newQty <= 0) {
    const idx = this.selectedPrescriptionItem.findIndex(x => x._rowId === itm._rowId);
    if (idx >= 0) {
      this.selectedPrescriptionItem.splice(idx, 1);
      console.log('🗑️ Item removed due to zero quantity');
    }
  }

  // Force UI update to hide/show split button
  this.updateUI();
  
  console.log('✅ Quantity decrease handled');
}



  private handleQtyIncrease(itm: LocalPharmacistItem, oldQty: number, newQty: number) {
    const medId = itm.medicineId as number;
    const delta = newQty - oldQty;


    // Check if batches are loaded
    if (!this.medicineBatches[medId]) {
      this.fetchBatchesForMedicine(medId, () => {
        this.handleQtyIncrease(itm, oldQty, newQty);
      });
      return;
    }

    // Calculate MAXIMUM POSSIBLE quantity for current batch
    const currentBatchMaxQty = this.getMaxPossibleQtyForCurrentBatch(itm, medId);
    const totalAvailable = this.medicineStocks[medId] || 0;
    const alreadyAllocatedTotalForMed = Object.values(this.batchAllocations[medId] || {}).reduce((s, v) => s + v, 0);
    const remainingGlobal = Math.max(0, totalAvailable - alreadyAllocatedTotalForMed);


    // 🚨 STRICT VALIDATION: Check if requested quantity exceeds current batch limit
    if (itm.batchId && newQty > currentBatchMaxQty) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Batch Limit',
        detail: `Batch ${itm.batchNo} has only ${currentBatchMaxQty - oldQty} units available. You can add ${currentBatchMaxQty - oldQty} more units to this batch.`
      });

      // Reset to current batch maximum
      itm.qty = currentBatchMaxQty;
      this.updateUI();
      return;
    }

    // 🚨 STRICT VALIDATION: Check if requested quantity exceeds total available stock
    if (newQty > (oldQty + remainingGlobal)) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Stock Limit',
        detail: `Only ${remainingGlobal} units available for ${itm.medicineName}. Maximum possible: ${oldQty + remainingGlobal}`
      });

      // Reset to maximum available quantity
      const maxPossibleQty = oldQty + remainingGlobal;
      itm.qty = maxPossibleQty;
      this.updateUI();
      return;
    }

    // Allocate the increased quantity
    this.allocateIncreasedQuantity(itm, oldQty, newQty, delta, medId);
  }



  private allocateIncreasedQuantity(itm: LocalPharmacistItem, oldQty: number, newQty: number, delta: number, medId: number) {
    let remainingIncrease = delta;

    // ✅ FIRST: Try to allocate to current batch (within its limits)
    if (itm.batchId) {
      const batch = this.medicineBatches[medId].find((b: any) => b.id === itm.batchId);
      if (batch) {
        const allocatedForThisBatch = this.getAllocatedForBatch(medId, itm.batchId);
        const batchAvailable = Math.max(0, (batch.quantity || 0) - allocatedForThisBatch);


        if (batchAvailable > 0) {
          const take = Math.min(batchAvailable, remainingIncrease);

          // 🚨 IMPORTANT: Only allocate what's actually available in current batch
          if (take > 0) {


            // Update allocation for current batch
            this.changeAllocatedForBatch(medId, itm.batchId, take);
            remainingIncrease -= take;
          }
        } else {
        }
      }
    }

    // ✅ SECOND: Allocate remaining quantity to new batches (create new rows)
    if (remainingIncrease > 0) {

      const newRows = this.createNewRowsForRemainingQuantity(itm, remainingIncrease, medId);
      if (newRows.length > 0) {
        // Add new rows to the main array
        newRows.forEach(row => {
          this.selectedPrescriptionItem.push(row);

        });



        // Show success message for batch allocation
        this.messageService.add({
          severity: 'success',
          summary: 'Batch Allocation',
          detail: `Quantity distributed into ${newRows.length} additional batch(es) for ${itm.medicineName}.`
        });
      } else {


        // 🚨 REVERT if no new batches available
        const actuallyAllocated = delta - remainingIncrease;
        itm.qty = oldQty + actuallyAllocated;

        this.messageService.add({
          severity: 'warn',
          summary: 'Partial Allocation',
          detail: `Only ${actuallyAllocated} additional units could be allocated for ${itm.medicineName}.`
        });
      }
    } else {

    }

    // Final update
    this.updateUI();
  }

  // ---------- more robust createNewRowsForRemainingQuantity (with logs and guaranteed _rowId) ----------
  private createNewRowsForRemainingQuantity(itm: LocalPharmacistItem, remainingQty: number, medId: number): LocalPharmacistItem[] {

    const newRows: LocalPharmacistItem[] = [];
    if (remainingQty <= 0) return newRows;

    const batches = this.medicineBatches[medId] || [];
    // sort by earliest expiry first
    const availableBatches = batches
      .filter(b => b.id !== itm.batchId) // exclude current batch (we already used it)
      .sort((a, b) => (a.daysToExpire ?? Number.MAX_SAFE_INTEGER) - (b.daysToExpire ?? Number.MAX_SAFE_INTEGER));

    let remaining = remainingQty;

    for (const batch of availableBatches) {
      if (remaining <= 0) break;

      const batchId = batch.id;
      const allocatedForBatch = this.getAllocatedForBatch(medId, batchId);
      const batchAvailable = Math.max(0, (batch.quantity || 0) - allocatedForBatch);



      if (batchAvailable <= 0) continue;

      const take = Math.min(batchAvailable, remaining);

      const row: LocalPharmacistItem = {
        prescriptionId: itm.prescriptionId,
        pharmacistPrescriptionId: itm.pharmacistPrescriptionId,
        medicineId: itm.medicineId,
        medicineName: itm.medicineName,
        dosage: itm.dosage,
        frequency: itm.frequency,
        duration: itm.duration,
        instructions: itm.instructions,
        qty: take,
        unitPrice: batch.sellingPrice ?? (itm.unitPrice ?? 0),
        totalPayableAmount: ((batch.sellingPrice ?? (itm.unitPrice ?? 0)) * take),
        isPrescribe: itm.isPrescribe ?? false,
        medicineFormId: itm.medicineFormId,
        batchId: batchId,
        batchNo: batch.batchNo,
        expiryDate: batch.expiryDate ? new Date(batch.expiryDate) : null,
        _isNew: true,
        _rowId: this.nextRowId()
      };

      // update allocation
      this.changeAllocatedForBatch(medId, batchId, take);

      newRows.push(row);
      remaining -= take;

    }

    if (newRows.length === 0) {

    }

    return newRows;
  }

  private createItemTemplate(itm: LocalPharmacistItem): LocalPharmacistItem {
    return {
      prescriptionId: itm.prescriptionId,
      pharmacistPrescriptionId: itm.pharmacistPrescriptionId,
      medicineId: itm.medicineId,
      medicineName: itm.medicineName,
      dosage: itm.dosage,
      frequency: itm.frequency,
      duration: itm.duration,
      instructions: itm.instructions,
      qty: 0,
      unitPrice: itm.unitPrice ?? 0,
      totalPayableAmount: itm.totalPayableAmount ?? 0,
      isPrescribe: itm.isPrescribe ?? false,
      medicineFormId: itm.medicineFormId,
      _isNew: true
    };
  }
  // --- new helper: remaining capacity in a specific batch for this row (excluding this row's current qty) ---
  private getBatchAvailableForRow(medId: number, batchId: number, currentRow?: LocalPharmacistItem): number {
    if (!medId || !batchId) return 0;
    const batch = (this.medicineBatches[medId] || []).find((b: any) => b.id === batchId);
    if (!batch) return 0;
    // allocated includes currentRow.qty; subtract currentRow.qty so we get "other allocated" amount
    const allocatedIncludingThis = this.getAllocatedForBatch(medId, batchId);
    const excludingThis = allocatedIncludingThis - (currentRow?.qty || 0);
    const available = Math.max(0, (batch.quantity || 0) - excludingThis);
    return available;
  }
private updateUI() {
  console.log('🔄 Updating UI...');

  // Force Angular to detect changes by creating new array reference
  this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
  this.total = this.getPrescriptionTotal();

  console.log('✅ UI updated', {
    itemsCount: this.selectedPrescriptionItem.length,
    total: this.total
  });

  // Force change detection
  this.cdRef.detectChanges();
  console.log('🎊 Change detection completed');
}
  /* ========== UI helpers ========== */
  getPrescriptionTotal(): number {
    if (!this.selectedPrescriptionItem?.length) return 0;
    return this.selectedPrescriptionItem
      .map(itm => (itm.unitPrice || 0) * (itm.qty || 0))
      .reduce((sum, val) => sum + val, 0);
  }

  getRemainingStock(itm: LocalPharmacistItem): number {
    const mid = itm.medicineId as number;
    if (!mid) return 0;

    const total = this.medicineStocks[mid] || 0;
    const allocated = Object.values(this.batchAllocations[mid] || {}).reduce((a, v) => a + v, 0);
    return Math.max(0, total - allocated);
  }
  /* ========== other existing methods ========== */
  loadPatients() {
    this.patientService.getOpdPatients().subscribe({
      next: (res) => this.patients = res,
      error: () => { }
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

    const prescription = this.prescriptions.find(x => x.id === prescriptionId);
    const items = prescription?.pharmacistPrescription || [];
    const items1 = items.filter(x => x.isPrescribe == true);

    // Clear existing items and allocations
    this.selectedPrescriptionItem = [];
    this.batchAllocations = {};

    // Process each prescription item
    items1.forEach((it: any) => {
      if (!it.qty || it.qty < 1) it.qty = 1;



      this.fetchBatchesForMedicine(it.medicineId, () => {
        this.createItemsForPrescription(it);
      });
    });
  }

  private createItemsForPrescription(prescriptionItem: any) {
    const medId = prescriptionItem.medicineId;
    const requestedQty = prescriptionItem.qty || 1;

    if (!medId) return;

    const totalAvailable = this.medicineStocks[medId] || 0;

    if (totalAvailable <= 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Out of Stock',
        detail: `No stock available for ${prescriptionItem.medicineName}`
      });
      return;
    }

    const template: LocalPharmacistItem = {
      ...prescriptionItem,
      _isNew: false,
      _rowId: this.nextRowId()
    };

    const createdRows = this.allocateAcrossBatchesAndCreateRows(template, requestedQty);

    if (createdRows.length > 0) {
      createdRows.forEach(row => {
        this.selectedPrescriptionItem.push(row);
      });

      this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
      this.total = this.getPrescriptionTotal();

      // Show toast if quantity was split into multiple batches
      if (createdRows.length > 1) {
        this.messageService.add({
          severity: 'info',
          summary: 'Batch Allocation',
          detail: `${prescriptionItem.medicineName} quantity distributed across ${createdRows.length} batches`
        });
      }
    }

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
    input.paymentMethod = this.paymentMethod;
    input.isPaid = false;
    input.pickedUpByPatient = this.selectedPatient;
    input.grandTotal = this.getPrescriptionTotal();

    // If backend expects DTOs without UI props, strip UI-only props here before sending.
    const itemsToSend = this.selectedPrescriptionItem.map(itm => {
      const copy = { ...itm } as any;
      // remove UI-only fields:
      delete copy._rowId; delete copy._isNew; delete copy.batchNo; delete copy.expiryDate; delete copy.durationValue; delete copy.durationUnit;
      return copy;
    });

    const resBody: any = {
      pharmacistPrescriptionsDto: input,
      pharmacistPrescriptionsListOfItem: itemsToSend,
    };

    this.pharmacistPrescriptionService.handlePharmacistPrescriptionPayment(resBody).subscribe({
      next: (result: any) => {
        if (this.paymentMethod === PaymentMethod._0) {
          this.notify.success('Created successfully!');
          this.onSave.emit();
          this.bsModalRef.hide();
        } else if (this.paymentMethod === PaymentMethod._1 && result) {
          window.location.href = result;
        }
      },
      error: () => this.isSaving = false,
      complete: () => this.isSaving = false
    });
  }

  removeItem(index: number): void {
    const itm = this.selectedPrescriptionItem[index];
    if (itm && itm.batchId) {
      this.changeAllocatedForBatch(itm.medicineId as number, itm.batchId as number, -(itm.qty || 0));
    }
    this.selectedPrescriptionItem.splice(index, 1);
    this.selectedPrescriptionItem = [...this.selectedPrescriptionItem];
    this.total = this.getPrescriptionTotal();
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
}