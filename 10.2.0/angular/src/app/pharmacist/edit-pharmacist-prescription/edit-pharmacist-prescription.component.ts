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
@Component({
  selector: 'app-edit-pharmacist-prescription',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, AbpModalFooterComponent, InputTextModule, MultiSelectModule, FormsModule, SelectModule,
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
  selectedPrescriptionItem!: PharmacistPrescriptionItemWithUnitPriceDto[];
  newPrescriptionItem!: PharmacistPrescriptionItemWithUnitPriceDto;
  selectedPrescriptionID!: number;
  _pharmacyNotes!: string;
  selectedPrescriptionName!: string;
  total: number = 0;
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};
  selectedMedicineUnits: { [medicineName: string]: string } = {};
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
            this.selectedPrescriptionItem = res.prescriptionItem;
            this.selectedPrescriptionName = `${res.patientName || ''} - ${res.issueDate.toDate().toLocaleDateString()} - ${res.prescriptionId}`
            this.selectedPrescriptionItem.forEach(itm => {
              if (!itm.qty || itm.qty < 1) {
                itm.qty = 1;
              }
            });
            // calculate total immediately on load
            this.total = this.getPrescriptionTotal();
          }
          this.cdRef.detectChanges();
        });
      }, error: (err) => {

      }
    })
  }
  setPaymentMethod(method: PaymentMethod) {
    this.paymentMethod = method;
  }
  save() {
    if (!this.createPharmacistPrescriptionForm.valid) {
      return;
    }

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
      // // Card -> create Stripe Checkout Session
      // this.labReceiptService.createStripeCheckoutSession(input).subscribe({

      //   next: (sessionUrl) => {
      //     window.location.href = sessionUrl; // redirect to Stripe
      //   },
      //   error: () => this.isSaving = false
      // });
    }
  }
  getPrescriptionTotal(): number {
    if (!this.selectedPrescriptionItem || !this.selectedPrescriptionItem.length) {
      return 0;
    }
    return this.selectedPrescriptionItem
      .map(itm => itm.unitPrice * itm.qty)
      .reduce((sum, val) => sum + val, 0);
  }

  /// add new
  onQtyChange(itm: any) {
    if (itm.qty < 1) {
      itm.qty = 1; // enforce minimum
    }
    this.total = this.getPrescriptionTotal(); // recalc every time qty changes
  }
  loadMedicines() {
    // Call getAll() with default parameters to get all available medicines
    this._pharmacistInventoryService.getAll(
      undefined,  // keyword
      undefined,  // sorting
      undefined,  // minStock
      undefined,  // maxStock
      undefined,  // fromExpiryDate
      true,       // isAvailable (only get available medicines)
      undefined,  // skipCount
      undefined   // maxResultCount
    ).subscribe({
      next: (res) => {
        if (res.items && res.items.length > 0) {
          const selectedIds = this.selectedPrescriptionItem?.map(itm => itm.medicineId) || [];
          this.medicineOptions = res.items.map(item => ({
            label: item.medicineName,
            value: item.id, // Use medicineId as value
            name: item.medicineName, // Store name separately
            disabled: selectedIds.includes(item.id)
          }));


          // Prepare dosage options for each medicine
          res.items.forEach(medicine => {
            const unit = medicine.unit;
            if (unit) {
              // Split units if they are comma separated (e.g., "200 mg, 500 mg")
              const units = unit.split(',').map(u => u.trim());
              this.medicineDosageOptions[medicine.medicineName] = units;
              this.selectedMedicineUnits[medicine.medicineName] = units[0];
            }
          });
        }
      },
      error: (err) => {
        this.notify.error('Could not load medicines');
        console.error('Error loading medicines:', err);
      }
    });
  }
  getDosageOptions(medicineName: string): any[] {
    if (!medicineName || !this.medicineDosageOptions[medicineName]) return [];

    return this.medicineDosageOptions[medicineName].map(unit => ({
      label: unit,
      value: unit
    }));
  }
  onMedicineChange(item: any) {
    const selected = this.medicineOptions.find(m => m.value === item.medicineId);
    if (selected) {
      item.medicineName = selected.name;

      // Set default dosage
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
    this.newPrescriptionItem = item;
  }
  removeItem(index: number): void {
    this.selectedPrescriptionItem.splice(index, 1);
    this.loadMedicines();
  }
  GetPriceOfMedicine(_medicineId: number) {
    this._pharmacistInventoryService.get(_medicineId).subscribe({
      next: (res) => {
        this.newPrescriptionItem.unitPrice = res.sellingPrice
      }, error: (err) => { }
    });
  }
  addItem() {
    if (this.newPrescriptionItem) {
      this.newPrescriptionItem.duration = `${(this.newPrescriptionItem as any).durationValue} ${(this.newPrescriptionItem as any).durationUnit}`;
      this.selectedPrescriptionItem.push(this.newPrescriptionItem);
      this.newPrescriptionItem = null;
      this.getPrescriptionTotal();
    }
  }
  // AddPrescriptionItems() {
  //   this.PrescriptionItemsService.createPrescriptionItemList(this.selectedPrescriptionItem).subscribe({
  //     next: (res) => {
  //       this.save();
  //     }, error: (err) => {
  //     }
  //   })
  // }
  validateMedicineStock(itm: any) {
    this._pharmacistInventoryService
      .getMedicineStatus(itm.medicineId, itm.qty)
      .subscribe(result => {
        if (!result.isValid) {
          // Rollback to previous value
          itm.qty = itm.qty - 1;
          this.messageService.add({ severity: 'warn', summary: 'Stock Check', detail: result.message });
        } else if (result.message.includes("Warning")) {
          this.messageService.add({ severity: 'info', summary: 'Stock Warning', detail: result.message });
        }
        // Update total anyway
        this.total = this.getPrescriptionTotal();
      });
  }
}
