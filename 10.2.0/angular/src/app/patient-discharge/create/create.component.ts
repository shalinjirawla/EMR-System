import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { BadgeModule } from 'primeng/badge';
import { AvatarModule } from 'primeng/avatar';
import { Component, Injector, ChangeDetectorRef, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table, TableModule } from 'primeng/table';
import { ActivatedRoute, Router } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { StepperModule } from 'primeng/stepper';
import { StepsModule } from 'primeng/steps';
import { CreatePrescriptionLabTestsServiceProxy, DischargeSummaryDto, LabTestStatus, PatientDetailsFordischargeSummaryDto, PatientDischargeServiceProxy, PrescriptionDto, PrescriptionLabTestDto, PrescriptionLabTestServiceProxy, PrescriptionServiceProxy, VitalDto } from '@shared/service-proxies/service-proxies';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { TextareaModule } from 'primeng/textarea';
import { AccordionModule } from 'primeng/accordion';
import { DialogModule } from 'primeng/dialog';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ViewPharmacistPrescriptionComponent } from '@app/pharmacist/view-pharmacist-prescription/view-pharmacist-prescription.component';
import { ViewLabReportComponent } from '@app/lab-technician/view-lab-report/view-lab-report.component';
@Component({
  selector: 'app-create',
  animations: [appModuleAnimation()],
  templateUrl: './create.component.html',
  styleUrl: './create.component.css',
  providers: [PatientDischargeServiceProxy, PrescriptionServiceProxy, CreatePrescriptionLabTestsServiceProxy],
  imports: [StepperModule, FormsModule, AccordionModule, DialogModule, StepsModule, TextareaModule, InputGroupModule, InputGroupAddonModule, CommonModule, TableModule, AvatarModule, BadgeModule, TabsModule, PaginatorModule, CheckboxModule,
    BreadcrumbModule, TooltipModule, CardModule, TagModule, SelectModule, InputTextModule, MenuModule, ButtonModule],
})
export class CreateComponent implements OnInit {
  patientId: number;
  items: MenuItem[] | undefined;
  activeStep: number = 1;
  value: any;
  data: DischargeSummaryDto = null;
  prescriptionData: PrescriptionDto[];
  prescriptionLabTest: PrescriptionLabTestDto[];
  visible = false;
  showVitalDetails: VitalDto;
  vitalLenght = 0;
  testStatus = [
    { label: 'Pending', value: LabTestStatus._0 },
    { label: 'In Progress', value: LabTestStatus._1 },
    { label: 'Completed', value: LabTestStatus._2 },
  ];
  constructor(
    private _activatedRoute: ActivatedRoute,
    private cd: ChangeDetectorRef, private router: Router,
    private _summaryService: PatientDischargeServiceProxy,
    private _prescriptionService: PrescriptionServiceProxy,
    private _modalService: BsModalService,
    private _prescriptionLabTestService: CreatePrescriptionLabTestsServiceProxy,
  ) {
    this.patientId = Number(this._activatedRoute.snapshot.paramMap.get('id'));
  }
  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Create Discharge summary' },
    ];
    this.GetSummaryDetails();
    this.GetPrescriptionsByPatient();
    this.GetPrescriptionLabTest();
  }
  gotList() {
    this.router.navigate(['app/doctors/patients'],);
  }
  calculateAge(dob: string | Date): number {
    const birthDate = new Date(dob);
    const today = new Date();

    let age = today.getFullYear() - birthDate.getFullYear();

    const hasBirthdayPassedThisYear =
      today.getMonth() > birthDate.getMonth() ||
      (today.getMonth() === birthDate.getMonth() && today.getDate() >= birthDate.getDate());

    if (!hasBirthdayPassedThisYear) {
      age--;
    }

    return age;
  }
  GetSummaryDetails() {
    this._summaryService.patientDischargeSummary(this.patientId).subscribe({
      next: (res) => {
        this.data = res;
        if (res.vitals) {
          this.vitalLenght = res.vitals.length;
        }
        this.cd.detectChanges();
      }, error: (err) => {

      }
    })
  }
  DisplayVitalDetails(recordId: any) {
    this.visible = true;
    this.showVitalDetails = this.data.vitals.find(x => x.id == recordId);
  }
  onclose() {
    this.visible = false;
    this.showVitalDetails = null
  }
  GetPrescriptionsByPatient() {
    this._prescriptionService.getPrescriptionsByPatient(this.patientId).subscribe({
      next: (res) => {
        this.prescriptionData = res.items;
        this.cd.detectChanges();
      }, error: (err) => {

      }
    });
  }
  ViewPharmacistPrescriptions(prescription: any) {
    let createDialog: BsModalRef;
    createDialog = this._modalService.show(ViewPharmacistPrescriptionComponent, {
      class: 'modal-lg',
      initialState: {
        _prescriptionId: prescription.id,
        _pharmacistPrescriptionId: prescription.pharmacistPrescription[0].pharmacistPrescriptionId
      },
    });
  }
  GetPrescriptionLabTest() {
    this._prescriptionLabTestService.getPrescriptionLabTestByPatientId(this.patientId).subscribe({
      next: (res) => {
        this.prescriptionLabTest = res;
        this.cd.detectChanges();
      }, error: (err) => {

      }
    });
  }
  getStatusLabel(value: number): string {
    const status = this.testStatus.find(s => s.value === value);
    return status ? status.label : '';
  }
  getStatusSeverity(value: number): 'info' | 'warn' | 'success' | 'danger' | 'secondary' | 'contrast' {
    switch (value) {
      case LabTestStatus._0: return 'info';        // Pending
      case LabTestStatus._1: return 'secondary';   // In Progress
      case LabTestStatus._2: return 'success';     // Completed
      default: return 'contrast';
    }
  }
  ViewLabReport(id?: number) {
    let viewReportDialog: BsModalRef;
    viewReportDialog = this._modalService.show(ViewLabReportComponent, {
      class: 'modal-xl',
      initialState: {
        id: id,
      },
    });
  }
}