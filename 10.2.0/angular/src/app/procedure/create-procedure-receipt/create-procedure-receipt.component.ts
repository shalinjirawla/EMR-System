import { Component, OnInit, ViewChild, EventEmitter, Output, Injector } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import { 
  PatientServiceProxy, 
  PrescriptionServiceProxy,
  PaymentMethod,
  CreateUpdateProcedureReceiptDto,
  ProcedureReceiptServiceProxy,
  CreateProcedureReceiptWithIdsDto
} from '@shared/service-proxies/service-proxies';
import { MessageService } from 'primeng/api';
import { AppComponentBase } from '@shared/app-component-base';
import { SelectModule } from 'primeng/select';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import moment from 'moment';


@Component({
  selector: 'app-create-procedure-receipt',
  imports: [AbpModalHeaderComponent, AbpModalFooterComponent, FormsModule, SelectModule, CommonModule, ButtonModule
  ],
  providers: [PatientServiceProxy, PrescriptionServiceProxy, ProcedureReceiptServiceProxy, MessageService],
  templateUrl: './create-procedure-receipt.component.html',
  styleUrl: './create-procedure-receipt.component.css'
})
export class CreateProcedureReceiptComponent extends AppComponentBase implements OnInit {
  @ViewChild('createProcedureForm', { static: true }) createProcedureForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  patients: any[] = [];
  prescriptions: any[] = [];
  selectedPatient: any;
  selectedPrescription: any;
  selectedProcedures: any[] = [];
  paymentMethod: PaymentMethod = PaymentMethod._0;
  isSaving = false;

  PaymentMethod = PaymentMethod;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private patientService: PatientServiceProxy,
    private prescriptionService: PrescriptionServiceProxy,
    private procedureReceiptService: ProcedureReceiptServiceProxy,
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
      this.prescriptionService.getPrescriptionsByPatient(this.selectedPatient).subscribe(res => {
        this.prescriptions = res.items.map(p => ({
          ...p,
          prescriptionName: `${p.patient?.fullName || ''} - ${p.issueDate.toDate().toLocaleDateString()} - ${p.id}`
        }));
      });
    } else {
      this.prescriptions = [];
      this.selectedPrescription = null;
      this.selectedProcedures = [];
    }
  }

onPrescriptionChange(prescriptionId: number) {
  const selected = this.prescriptions.find(p => p.id === prescriptionId);
  this.selectedProcedures = selected?.procedures.map(proc => ({
    id: proc.id,
    name: proc.procedureName,
    price: proc.emergencyProcedures?.defaultCharge || 0
  })) || [];
}

  get totalAmount(): number {
    return this.selectedProcedures.reduce((sum, p) => sum + (p.price || 0), 0);
  }

  setPaymentMethod(method: PaymentMethod) {
    this.paymentMethod = method;
  }

save() {
  if (!this.createProcedureForm.valid || !this.selectedProcedures.length) {
    this.messageService.add({ severity: 'warn', summary: 'Warning', detail: 'Please complete all required fields.' });
    return;
  }

  this.isSaving = true;

  const input = new CreateUpdateProcedureReceiptDto({
    id: 0,
    tenantId: abp.session.tenantId,
    patientId: this.selectedPatient,
    paymentMethod: this.paymentMethod,
    totalFee: this.totalAmount,
    paymentDate: moment(),
    status: 0,
    paymentIntentId: ''
  });

  const dto = new CreateProcedureReceiptWithIdsDto({
    input,
    selectedProcedureIds: this.selectedProcedures.map(p => p.id)
  });

  if (this.paymentMethod === PaymentMethod._0) {
    // Cash
    debugger
    this.procedureReceiptService.createProcedureReceipt(dto).subscribe({
      next: () => {
        this.notify.success('Procedure receipt created successfully!');
        this.onSave.emit();
        this.bsModalRef.hide();
      },
      error: () => this.isSaving = false
    });
  } else {
    // Card -> Stripe
    this.procedureReceiptService.createStripeCheckoutSession(dto).subscribe({
      next: (sessionUrl) => window.location.href = sessionUrl,
      error: () => this.isSaving = false
    });
  }
}



}
