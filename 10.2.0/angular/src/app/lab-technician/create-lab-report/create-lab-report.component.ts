import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { SelectModule } from "primeng/select";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@node_modules/@angular/common';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import {
  LabReportDetailDto,
  LabReportResultItemDto,
  LabReportsTypeServiceProxy,
  LabReportTemplateItemServiceProxy,
  PatientServiceProxy,
  PaymentMethod,
  CreateUpdatePrescriptionLabTestDto,
  LabTestStatus,
  CreatePrescriptionLabTestsServiceProxy,
  PackageSuggestionDto,
  LabTestSource,
  PrescriptionServiceProxy,
  CreateLabTestReceiptDto,
  LabTestReceiptServiceProxy
} from '@shared/service-proxies/service-proxies';
import { MultiSelectModule } from 'primeng/multiselect';
import { MessageService } from 'primeng/api';

interface CombinedTestOrPackage {
  id: number;
  name: string;
  price: number;
  type: 'Test' | 'Package';
  packageTestIds?: number[]; // present only for packages
}

@Component({
  selector: 'app-create-lab-report',
  imports: [DropdownModule, SelectModule, ToggleButtonModule, ButtonModule, CheckboxModule, FormsModule, CommonModule, TagModule, MultiSelectModule],
  providers: [PatientServiceProxy,LabTestReceiptServiceProxy,MessageService, LabReportsTypeServiceProxy, PrescriptionServiceProxy, LabReportTemplateItemServiceProxy, CreatePrescriptionLabTestsServiceProxy],
  templateUrl: './create-lab-report.component.html',
  styleUrl: './create-lab-report.component.css'
})
export class CreateLabReportComponent implements OnInit {
  patients: any[] = [];
  labTests: any[] = [];
  suggestions: any[] = [];
  prescriptions: any[] = [];
  labTestSource: LabTestSource = LabTestSource._1; // default or null
  selectedPrescription: any = null;
  paymentMethod: PaymentMethod = PaymentMethod._0; // Default to Cash
  isSaving = false;
  LabTestSource = LabTestSource;
  selectedPatient: any;
  selectedLabTests: number[] = [];

  paymentMethods = [
  { label: 'Cash', value: PaymentMethod._0 },
  { label: 'Card', value: PaymentMethod._1 }
];

  constructor(
    private patientService: PatientServiceProxy,
    private labTestService: LabReportsTypeServiceProxy,
    private prescriptionService: PrescriptionServiceProxy,
    private _labReceiptService:LabTestReceiptServiceProxy,
    private _messageService: MessageService
  ) { }

  ngOnInit() {
    this.loadPatients();
    this.loadLabTests();
  }

  loadPatients() {
    this.patientService.getOpdPatients().subscribe(res => {
      this.patients = res;
    });
  }

  loadLabTests() {
    this.labTestService.getAllTestsAndPackagesByTenantId(abp.session.tenantId).subscribe(res => {
      this.labTests = res.items // Only tests
    });
  }
  onLabTestSourceChange() {
    if (this.labTestSource === LabTestSource._0 && this.selectedPatient) {
      this.loadPrescriptions(this.selectedPatient);
    } else {
      this.prescriptions = [];
      this.selectedPrescription = null;
    }
  }
  onPatientChange() {
    if (this.labTestSource === LabTestSource._0 && this.selectedPatient) {
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
      this.selectedLabTests = [];
      return;
    }

    const selected = this.prescriptions.find(p => p.id === prescriptionId);
    if (selected && selected.labTestIds) {
      this.selectedLabTests = [...selected.labTestIds];
    } else {
      this.selectedLabTests = [];
    }

    this.onLabTestsChange();
  }


  onLabTestsChange() {
    // 🔹 Sabhi tests ko by default enable
    this.labTests.forEach(t => t.disabled = false);

    // 🔹 Agar koi package select hai to uske andar ke tests disable kar do
    const selectedPackages = this.labTests.filter(
      t => this.selectedLabTests.includes(t.id) && t.type === 'Package'
    );

    selectedPackages.forEach(pkg => {
      pkg.packageTestIds?.forEach(testId => {
        const testItem = this.labTests.find(t => t.id === testId);
        if (testItem) {
          testItem.disabled = true;
        }
      });
    });
    this.selectedLabTests = this.selectedLabTests.filter(id => {
      const item = this.labTests.find(t => t.id === id);
      return !(item?.disabled && item.type === 'Test');
    });

    // 🔹 Suggestions ke liye API call (agar koi package select nahi hai)
    const onlyTestsSelected = this.selectedLabTests.filter(id => {
      const item = this.labTests.find(t => t.id === id);
      return item && item.type === 'Test';
    });

    if (onlyTestsSelected.length >= 2) {
      this.labTestService.getPackageSuggestions(onlyTestsSelected)
        .subscribe(res => {
          this.suggestions = res;
        });
    } else {
      this.suggestions = [];
    }
  }

  replaceTests(suggestion: any) {
    // 1️⃣ Remove only the tests that are in the includedTests list
    this.selectedLabTests = this.selectedLabTests.filter(
      id => !suggestion.includedTests.includes(id)
    );

    // 2️⃣ Add the package itself to the selection
    const packageItem = this.labTests.find(
      x => x.id === suggestion.packageId && x.type === 'Package'
    );
    if (packageItem) {
      this.selectedLabTests.push(packageItem.id);

      // 3️⃣ Disable its tests in the dropdown
      packageItem.packageTestIds?.forEach(testId => {
        const testItem = this.labTests.find(t => t.id === testId);
        if (testItem) {
          testItem.disabled = true; // disable flag
        }
      });
    }

    // 4️⃣ Remove disabled items from selection (in case they were selected before)
    this.selectedLabTests = this.selectedLabTests.filter(id => {
      const item = this.labTests.find(t => t.id === id);
      return !(item?.disabled && item.type === 'Test');
    });

    // 5️⃣ Clear suggestions
    this.suggestions = [];
  }

  get selectedTestsTotalPrice(): number {
    if (!this.selectedLabTests || this.selectedLabTests.length === 0) {
      return 0;
    }

    return this.selectedLabTests.reduce((total, testId) => {
      const test = this.labTests.find(t => t.id === testId);
      return test ? total + (test.price || 0) : total;
    }, 0);
  }
  // Add this getter inside your component class
get selectedTestDetails() {
  if (!this.selectedLabTests || this.selectedLabTests.length === 0) {
    return [];
  }
  return this.selectedLabTests
    .map(id => this.labTests.find(t => t.id === id))
    .filter(t => t != null); // filter out nulls just in case
}
save() {
    if (!this.validateBeforeSave()) return;

    this.isSaving = true;

    const input = new CreateLabTestReceiptDto({
      patientId: this.selectedPatient,
      labTestSource: this.labTestSource,
      prescriptionId: this.labTestSource === LabTestSource._0 ? this.selectedPrescription : undefined,
      selectedTestIds: this.getNonPackageTestIds(),
      selectedPackageIds: this.getPackageIds(),
      paymentMethod: this.paymentMethod,
      totalAmount: this.selectedTestsTotalPrice
    });
debugger
    this._labReceiptService.createLabTestReceipt(input).subscribe({
      next: (receiptId) => {
        this._messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Lab receipt created successfully!'
        });
        // Reset form or close modal
        this.resetForm();
      },
      error: (err) => {
        this.isSaving = false;
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to create lab receipt'
        });
      }
    });
  }

  // Helper methods
  private getNonPackageTestIds(): number[] {
    return this.selectedLabTests.filter(id => {
      const item = this.labTests.find(t => t.id === id);
      return item && item.type === 'Test';
    });
  }

  private getPackageIds(): number[] {
    return this.selectedLabTests.filter(id => {
      const item = this.labTests.find(t => t.id === id);
      return item && item.type === 'Package';
    });
  }

  private validateBeforeSave(): boolean {
    if (!this.selectedPatient) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Warning',
        detail: 'Please select a patient'
      });
      return false;
    }

    if (this.selectedLabTests.length === 0) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Warning',
        detail: 'Please select at least one test'
      });
      return false;
    }

    if (this.labTestSource === LabTestSource._0 && !this.selectedPrescription) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Warning',
        detail: 'Please select a prescription for OPD cases'
      });
      return false;
    }

    return true;
  }

  private resetForm() {
    this.selectedPatient = null;
    this.selectedPrescription = null;
    this.selectedLabTests = [];
    this.paymentMethod = PaymentMethod._0;
    this.isSaving = false;
  }
}