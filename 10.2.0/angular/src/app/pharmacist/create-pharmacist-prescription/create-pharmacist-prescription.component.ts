import { Component, OnInit, ViewChild, EventEmitter, Output, Injector } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import {
  PatientServiceProxy,
  PaymentMethod,
  PharmacistPrescriptionItemWithUnitPriceDto,
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
@Component({
  selector: 'app-create-pharmacist-prescription',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, AbpModalFooterComponent, MultiSelectModule, FormsModule, SelectModule,
    CommonModule, ButtonModule, TextareaModule
  ],
  templateUrl: './create-pharmacist-prescription.component.html',
  styleUrl: './create-pharmacist-prescription.component.css',
  providers: [PatientServiceProxy, MessageService, PrescriptionServiceProxy]
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
  pharmacyNotes!: string;
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private patientService: PatientServiceProxy,
    private prescriptionService: PrescriptionServiceProxy,
    private messageService: MessageService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.loadPatients();
  }

  loadPatients() {
    this.patientService.getOpdPatients().subscribe(res => this.patients = res);
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
    this.selectedPrescriptionItem = this.prescriptions.find(x => x.id === prescriptionId)?.pharmacistPrescription;
  }

  setPaymentMethod(method: PaymentMethod) {
    this.paymentMethod = method;
  }

  save() {
    if (!this.createPharmacistPrescriptionForm.valid) {
      return;
    }

    this.isSaving = true;

    const input = new CreateLabTestReceiptDto({
      tenantId: abp.session.tenantId,
      patientId: this.selectedPatient,
      labTestSource: this.labTestSource,
      prescriptionId: this.labTestSource === LabTestSource._0 ? this.selectedPrescription : undefined,
      paymentMethod: this.paymentMethod,
      totalAmount: this.selectedTestsTotalPrice
    });

    if (this.paymentMethod === PaymentMethod._0) {
      // Cash -> direct create
      this.labReceiptService.createLabTestReceipt(input).subscribe({
        next: () => {
          this.notify.success('Lab receipt created successfully!');
          this.onSave.emit();
          this.bsModalRef.hide();
        },
        error: () => this.isSaving = false
      });
    } else {
      // Card -> create Stripe Checkout Session
      this.labReceiptService.createStripeCheckoutSession(input).subscribe({

        next: (sessionUrl) => {
          debugger
          window.location.href = sessionUrl; // redirect to Stripe
        },
        error: () => this.isSaving = false
      });
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
}
