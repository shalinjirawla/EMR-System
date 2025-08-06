import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
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
import { LabReportDetailDto, LabReportResultItemDto, LabReportsTypeServiceProxy, 
  LabReportTemplateItemServiceProxy, PatientServiceProxy, PaymentMethod, 
  PrescriptionLabTestsServiceProxy, CreateUpdatePrescriptionLabTestDto, 
  LabTestStatus,
  LabTestCreationResultDto} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-lab-report',
  imports: [AbpModalFooterComponent, AbpModalHeaderComponent,DropdownModule,SelectModule,ToggleButtonModule , ButtonModule,CheckboxModule,FormsModule,CommonModule,TagModule ],
  providers: [PatientServiceProxy,LabReportsTypeServiceProxy,LabReportTemplateItemServiceProxy,PrescriptionLabTestsServiceProxy],
  templateUrl: './create-lab-report.component.html',
  styleUrl: './create-lab-report.component.css'
})
export class CreateLabReportComponent implements OnInit {
  patients = [];
  labTests = [];
  selectedPatient: any = null;
  selectedPatientId: number | null = null;
  selectedLabTestId: number | null = null;
  isAdmitted = false;
  paymentMode: PaymentMethod = PaymentMethod._0;
  selectedLabTest: LabReportDetailDto;
  items: any[] = [];
  saving = false;

  constructor(
    private _patientService: PatientServiceProxy,
    private _labService: LabReportsTypeServiceProxy,
    private _prescriptionLabTestsService: PrescriptionLabTestsServiceProxy,
    public bsModalRef: BsModalRef,
    public cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadPatients();
    this.loadLabTests();
  }

  loadPatients() {
    this._patientService.patientDropDown().subscribe((res) => {
      this.patients = res;
    });
  }

  loadLabTests() {
    this._labService.getAllTestByTenantID(abp.session.tenantId).subscribe((res) => {
      this.labTests = res.items;
    });
  }

  onPatientChange(patientId: number) {
    this.selectedPatientId = patientId;
    this.selectedPatient = this.patients.find(p => p.id === patientId);
    this.isAdmitted = !!this.selectedPatient?.isAdmitted;
  }

  onLabTestChange(labTestId: number) {
    this.selectedLabTestId = labTestId;

    this._labService.getReportDetailsWithTests(labTestId).subscribe((res: LabReportDetailDto) => {
      this.selectedLabTest = res;
      this.items = res.labTests.map(test => ({
        ...test,
        result: '',
        flag: null
      }));
      this.cdr.detectChanges();
    });
  }

  onResultChange(item: any) {
    const parsedResult = parseFloat(item.result);
    if (isNaN(parsedResult) || item.minRange == null || item.maxRange == null) {
      item.flag = undefined;
      return;
    }
    if (parsedResult < item.minRange) item.flag = 'Low';
    else if (parsedResult > item.maxRange) item.flag = 'High';
    else item.flag = 'Normal';
  }

 
  

  save() {
    if (!this.selectedPatientId || !this.selectedLabTestId || this.items.length === 0) return;

    const input = new CreateUpdatePrescriptionLabTestDto();
    input.tenantId = abp.session.tenantId;
    input.patientId = this.selectedPatientId;
    input.labReportsTypeId = this.selectedLabTestId;
    input.testStatus = LabTestStatus._1; // Completed
    input.isPaid = !this.isAdmitted;
    input.paymentMethod = this.isAdmitted ? null : this.paymentMode;

    input.resultItems = this.items.map(i => {
      const item = new LabReportResultItemDto();
      item.test = i.labTestName;
      item.result = i.result;
      item.minValue = i.minRange;
      item.maxValue = i.maxRange;
      item.unit = i.measureUnitName;
      item.flag = i.flag;
      return item;
    });

    this.saving = true;
debugger
    this._prescriptionLabTestsService.createLabTest(input).subscribe({
      next: (result: LabTestCreationResultDto) => {
       if (result.isStripeRedirect && result.stripeSessionUrl) {
        // Redirect to Stripe Checkout
        window.location.href = result.stripeSessionUrl;
      } else {
        // OPD cash or IPD path
        abp.notify.success('Lab report created successfully!');
        this.bsModalRef.hide();
      }
      },
      error: () => this.saving = false,
      complete: () => this.saving = false
    });
  }
  getSeverity(flag: string): string {
    switch (flag) {
      case 'High': return 'danger';
      case 'Low': return 'warn';
      case 'Normal': return 'success';
      default: return 'info';
    }
  }
  
  isAbnormal(item): boolean {
    const value = parseFloat(item.result);
    return !isNaN(value) && (value < item.minRange || value > item.maxRange);
  }
  
    
}
