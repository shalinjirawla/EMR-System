import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { PatientServiceProxy, LabReportsTypeServiceProxy, PrescriptionLabTestsServiceProxy, LabReportTemplateItemServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { SelectModule } from "primeng/select";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@node_modules/@angular/common';
import { ToggleButtonModule } from 'primeng/togglebutton';

@Component({
  selector: 'app-create-lab-report',
  imports: [AbpModalFooterComponent, AbpModalHeaderComponent, SelectModule,ToggleButtonModule , ButtonModule,CheckboxModule,FormsModule,CommonModule],
  providers: [PatientServiceProxy,LabReportsTypeServiceProxy,LabReportTemplateItemServiceProxy],
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
  paymentMode: 'Cash' | 'Card';

  selectedLabTest: any = null;
  items: any[] = [];

  saveTemplate = false;
  saving = false;

  constructor(
    private _patientService: PatientServiceProxy,
    private _labService: LabReportsTypeServiceProxy,
    private _labReportTemplateItemService:LabReportTemplateItemServiceProxy,
    public bsModalRef: BsModalRef,
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
    this.selectedLabTest = this.labTests.find(test => test.id === labTestId);
  }
  

  cloneTestItems() {
    debugger
    if (!this.selectedLabTest) return;
    this.items = [];
    this._labReportTemplateItemService.getByLabReportsTypeId(this.selectedLabTestId).subscribe((res) => {
      this.items = res;
      console.log(this.items);
    });
    // Imagine testItems is part of labTest, here you’d map actual items
    // For this demo, I'll just make one item with current test’s info:
    this.items.push({
      test: this.selectedLabTest.reportType,
      result: '',
      minValue: this.selectedLabTest.minValue,
      maxValue: this.selectedLabTest.maxValue,
      unit: this.selectedLabTest.unit,
      flag: undefined // will be set after entering result
    });
  }

  addItem() {
    this.items.push({ test: '', result: '', min: '', max: '', unit: '', flag: undefined });
  }

  removeItem(i: number) {
    this.items.splice(i, 1);
  }

  onResultChange(item: any) {
    if (!item.result || !item.min || !item.max) {
      item.flag = undefined;
      return;
    }
    item.flag = this.getFlag(item.result, `${item.min}-${item.max}`);
  }

  getFlag(result: string, reference: string): string {
    const parsedResult = parseFloat(result);
    const [min, max] = reference.split('-').map(v => parseFloat(v));
    if (isNaN(parsedResult) || isNaN(min) || isNaN(max)) return 'Unknown';
    if (parsedResult < min) return 'Low';
    if (parsedResult > max) return 'High';
    return 'Normal';
  }

  getSeverity(flag: string): string {
    switch (flag) {
      case 'Normal': return 'success';
      case 'High': return 'danger';
      case 'Low': return 'warn';
      default: return 'secondary';
    }
  }

  save() {
    this.saving = true;
    // Prepare DTO, send API, etc.
    // On success:
    this.bsModalRef.hide();
  }
}
