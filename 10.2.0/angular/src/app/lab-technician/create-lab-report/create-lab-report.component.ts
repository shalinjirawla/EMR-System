import { Component, OnInit, ViewChild, EventEmitter, Output, Injector } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import {
  LabReportsTypeServiceProxy,
  PatientServiceProxy,
  PaymentMethod,
  LabTestSource,
  PrescriptionServiceProxy,
  CreateLabTestReceiptDto,
  LabTestReceiptServiceProxy
} from '@shared/service-proxies/service-proxies';
import { MessageService } from 'primeng/api';
import { AppComponentBase } from '@shared/app-component-base';
import { MultiSelectModule } from 'primeng/multiselect';
import { SelectModule } from 'primeng/select';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-create-lab-report',
  standalone: true,
  imports: [
    AbpModalHeaderComponent, AbpModalFooterComponent,MultiSelectModule,FormsModule,SelectModule,CommonModule,ButtonModule
  ],
  templateUrl: './create-lab-report.component.html',
  styleUrls: ['./create-lab-report.component.css'],
  providers: [PatientServiceProxy, LabTestReceiptServiceProxy, MessageService, LabReportsTypeServiceProxy, PrescriptionServiceProxy]
})
export class CreateLabReportComponent extends AppComponentBase implements OnInit {
  @ViewChild('createLabReportForm', { static: true }) createLabReportForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  patients: any[] = [];
  labTests: any[] = [];
  suggestions: any[] = [];
  prescriptions: any[] = [];
  labTestSource: LabTestSource = LabTestSource._1;
  selectedPrescription: any = null;
  paymentMethod: PaymentMethod = PaymentMethod._0;
  isSaving = false;
  selectedPatient: any;
  selectedLabTests: string[] = [];

  LabTestSource = LabTestSource;
  PaymentMethod = PaymentMethod;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private patientService: PatientServiceProxy,
    private labTestService: LabReportsTypeServiceProxy,
    private prescriptionService: PrescriptionServiceProxy,
    private labReceiptService: LabTestReceiptServiceProxy,
    private messageService: MessageService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.loadPatients();
    this.loadLabTests();
  }

  loadPatients() {
    this.patientService.getOpdPatients().subscribe(res => this.patients = res);
  }

  loadLabTests() {
    this.labTestService.getAllTestsAndPackagesByTenantId(abp.session.tenantId).subscribe(res => {
      this.labTests = res.items.map(t => ({
        ...t,
        uniqueId: `${t.type}-${t.id}`
      }));
    });
  }

  onPatientChange() {
    if (this.labTestSource === LabTestSource._0 && this.selectedPatient) {
      this.loadPrescriptions(this.selectedPatient);
    } else {
      this.prescriptions = [];
      this.selectedPrescription = null;
    }
  }

  onLabTestSourceChange() {
    if (this.labTestSource === LabTestSource._0 && this.selectedPatient) {
      this.loadPrescriptions(this.selectedPatient);
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
      this.selectedLabTests = [];
      return;
    }

    const selected = this.prescriptions.find(p => p.id === prescriptionId);
    if (selected && selected.labTestIds) {
      this.selectedLabTests = selected.labTestIds
        .map(testId => {
          const testItem = this.labTests.find(t => t.id === testId && t.type === 'Test');
          return testItem ? testItem.uniqueId : null;
        })
        .filter(x => x != null);
    } else {
      this.selectedLabTests = [];
    }

    this.onLabTestsChange();
  }

  onLabTestsChange() {
    // disable tests included in packages
    this.labTests.forEach(t => t.disabled = false);
    const selectedPackages = this.labTests.filter(
      t => this.selectedLabTests.includes(t.uniqueId) && t.type === 'Package'
    );
    selectedPackages.forEach(pkg => {
      pkg.packageTestIds?.forEach(testId => {
        const testItem = this.labTests.find(t => t.id === testId && t.type === 'Test');
        if (testItem) testItem.disabled = true;
      });
    });
    this.selectedLabTests = this.selectedLabTests.filter(uid => {
      const item = this.labTests.find(t => t.uniqueId === uid);
      return !(item?.disabled && item.type === 'Test');
    });
    const onlyTestsSelected = this.selectedLabTests
      .map(uid => this.labTests.find(t => t.uniqueId === uid))
      .filter(t => t?.type === 'Test')
      .map(t => t.id);
    if (onlyTestsSelected.length >= 2) {
      this.labTestService.getPackageSuggestions(onlyTestsSelected).subscribe(res => this.suggestions = res);
    } else {
      this.suggestions = [];
    }
  }

  replaceTests(suggestion: any) {
    this.selectedLabTests = this.selectedLabTests.filter(uid => {
      const item = this.labTests.find(t => t.uniqueId === uid);
      return !suggestion.includedTests.includes(item?.id);
    });
    const packageItem = this.labTests.find(x => x.id === suggestion.packageId && x.type === 'Package');
    if (packageItem) {
      this.selectedLabTests.push(packageItem.uniqueId);
      packageItem.packageTestIds?.forEach(testId => {
        const testItem = this.labTests.find(t => t.id === testId && t.type === 'Test');
        if (testItem) testItem.disabled = true;
      });
    }
    this.suggestions = [];
  }

  get selectedTestsTotalPrice(): number {
    return this.selectedLabTests.reduce((total, uid) => {
      const test = this.labTests.find(t => t.uniqueId === uid);
      return total + (test?.price || 0);
    }, 0);
  }

  get selectedTestDetails() {
    return this.selectedLabTests.map(uid => this.labTests.find(t => t.uniqueId === uid)).filter(t => t);
  }

  setPaymentMethod(method: PaymentMethod) {
    this.paymentMethod = method;
  }

save() {
  if (!this.createLabReportForm.valid || !this.selectedLabTests.length) {
    this.messageService.add({ severity: 'warn', summary: 'Warning', detail: 'Please complete all required fields.' });
    return;
  }

  this.isSaving = true;

  const input = new CreateLabTestReceiptDto({
    tenantId:abp.session.tenantId,
    patientId: this.selectedPatient,
    labTestSource: this.labTestSource,
    prescriptionId: this.labTestSource === LabTestSource._0 ? this.selectedPrescription : undefined,
    selectedTestIds: this.getNonPackageTestIds(),
    selectedPackageIds: this.getPackageIds(),
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


  private getNonPackageTestIds(): number[] {
    return this.selectedLabTests
      .map(uid => this.labTests.find(t => t.uniqueId === uid))
      .filter(t => t?.type === 'Test')
      .map(t => t.id);
  }

  private getPackageIds(): number[] {
    return this.selectedLabTests
      .map(uid => this.labTests.find(t => t.uniqueId === uid))
      .filter(t => t?.type === 'Package')
      .map(t => t.id);
  }
}
