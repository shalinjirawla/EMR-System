import { Component, OnInit, ViewChild, EventEmitter, Output, Injector } from '@angular/core';
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
@Component({
  selector: 'app-create-pharmacist-prescription',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, AbpModalFooterComponent, MultiSelectModule, FormsModule, SelectModule,
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
  selectedPrescriptionItem!: PharmacistPrescriptionItemWithUnitPriceDto[];
  newPrescriptionItem!: PharmacistPrescriptionItemWithUnitPriceDto;
  selectedPrescriptionID!: number;
  _pharmacyNotes!: string;
  total: number = 0;
  newUnitPrice!: any;
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};
  selectedMedicineUnits: { [medicineName: string]: string } = {};
  _medicinePrice!: any;
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
        this.patients = res
      }, error: (err) => {

      }
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
    this.prescriptionService.getPrescriptionsByPatient(patientId).subscribe(res => {
      this.prescriptions = res.items.map(p => ({
        ...p,
        prescriptionName: `${p.patient?.fullName || ''} - ${p.issueDate.toDate().toLocaleDateString()} - ${p.id}`
      }));
    });
  }
  onPrescriptionChange(prescriptionId: number) {
    if (!prescriptionId) {
      this.selectedPrescriptionItem = [];
      return;
    }
    this.selectedPrescriptionID = prescriptionId;
    const filterdList = this.prescriptions.find(x => x.id === prescriptionId)?.pharmacistPrescription;
    this.selectedPrescriptionItem=filterdList.filter(x=>x.pharmacistPrescriptionId==null)
    this.selectedPrescriptionItem.forEach(itm => {
      if (!itm.qty || itm.qty < 1) {
        itm.qty = 1;
      }
    });
    // calculate total immediately on load
    this.total = this.getPrescriptionTotal();
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
    input.tenantId = abp.session.tenantId;
    input.prescriptionId = this.selectedPrescriptionID;
    input.issueDate = moment();
    input.pharmacyNotes = this._pharmacyNotes;
    input.collectionStatus = CollectionStatus._1;
    input.pickedUpByPatient=this.selectedPatient;
    input.grandTotal = this.getPrescriptionTotal();
    const resBody: any = {
      pharmacistPrescriptionsDto: input,
      pharmacistPrescriptionsListOfItem: this.selectedPrescriptionItem,
    }
    if (this.paymentMethod === PaymentMethod._0) {
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
  onQtyChange(itm: any) {
    const request = {
      items: [{ medicineId: itm.medicineId, requestedQty: itm.qty }]
    };
    if (itm.qty < 1) {
      itm.qty = 1; // enforce minimum
    }
    this.total = this.getPrescriptionTotal(); // recalc every time qty changes
  }
  getPrescriptionTotal(): number {
    if (!this.selectedPrescriptionItem?.length) {
      return 0;
    }
    return this.selectedPrescriptionItem
      .map(itm => (itm.unitPrice || 0) * (itm.qty || 1))
      .reduce((sum, val) => sum + val, 0);
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
  checkForMedicine(medicineId: any, qty: any) {
    this._pharmacistInventoryService.getMedicineStatus(medicineId, qty)
      .subscribe({
        next: (res) => {
          alert(res)
        }, error: (err) => {

        }
      })
  }
}
