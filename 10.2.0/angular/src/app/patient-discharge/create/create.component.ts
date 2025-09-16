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
import { CreatePrescriptionLabTestsServiceProxy, DischargeStatus, DischargeSummaryDto, EmergencyProcedureStatus, InvoiceDto, LabTestStatus, PatientDischargeServiceProxy, PrescriptionDto, PrescriptionLabTestDto, PrescriptionLabTestServiceProxy, PrescriptionServiceProxy, SelectedEmergencyProcedures, SelectedEmergencyProceduresDto, SelectedEmergencyProceduresServiceProxy, VitalDto } from '@shared/service-proxies/service-proxies';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { TextareaModule } from 'primeng/textarea';
import { AccordionModule } from 'primeng/accordion';
import { DialogModule } from 'primeng/dialog';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { EditorModule } from 'primeng/editor';
import { ViewLabReportComponent } from '@app/lab-technician/view-lab-report/view-lab-report.component';
import { ViewPrescriptionComponent } from '@app/doctors/view-prescription/view-prescription.component';
import { CreatePrescriptionsComponent } from '@app/doctors/create-prescriptions/create-prescriptions.component';
import { ViewInvoiceComponent } from '@app/billing-staff/view-invoice/view-invoice.component';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
@Component({
  selector: 'app-create',
  animations: [appModuleAnimation()],
  templateUrl: './create.component.html',
  styleUrl: './create.component.css',
  providers: [PatientDischargeServiceProxy, PrescriptionServiceProxy, CreatePrescriptionLabTestsServiceProxy, MessageService],
  imports: [StepperModule, FormsModule, EditorModule, AccordionModule, DialogModule, StepsModule, TextareaModule, InputGroupModule, InputGroupAddonModule, CommonModule, TableModule, AvatarModule, BadgeModule, TabsModule, PaginatorModule, CheckboxModule,
    BreadcrumbModule, TooltipModule, ToastModule, CardModule, TagModule, SelectModule, InputTextModule, MenuModule, ButtonModule],
})
export class CreateComponent implements OnInit {
  patientId: number;
  items: MenuItem[] | undefined;
  activeStep: number = 1;
  value: any;
  data: DischargeSummaryDto = null;
  visible = false;
  showVitalDetails: VitalDto;
  currentStep!: number;
  testStatus = [
    { label: 'Pending', value: LabTestStatus._0 },
    { label: 'In Progress', value: LabTestStatus._1 },
    { label: 'Completed', value: LabTestStatus._2 },
  ];
  procedureStatus = [
    { label: 'Pending', value: EmergencyProcedureStatus._0 },
    { label: 'Completed', value: EmergencyProcedureStatus._1 },
  ];
  constructor(
    private _activatedRoute: ActivatedRoute,
    private cd: ChangeDetectorRef, private router: Router,
    private _summaryService: PatientDischargeServiceProxy,
    private _prescriptionService: PrescriptionServiceProxy,
    private _modalService: BsModalService,
    private messageService: MessageService,
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
        if (this.data.patientDischarge.dischargeStatus < 5) {
          this.currentStep = this.data.patientDischarge.dischargeStatus + 1;
        }
        else {
          this.currentStep = 5;
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
  showViewPrescriptionDialog(id: number): void {
    const viewPrescriptionDialog: BsModalRef = this._modalService.show(
      ViewPrescriptionComponent,
      {
        class: 'modal-lg',
        initialState: {
          id: id
        }
      }
    );
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
      class: 'modal-lg',
      initialState: {
        id: id,
      },
    });
  }
  showCreatePrescriptionDialog() {
    let createPrescriptionDialog: BsModalRef;
    createPrescriptionDialog = this._modalService.show(CreatePrescriptionsComponent, {
      class: 'modal-xl',
      initialState: {
        dischargePatientID: this.patientId,
      }
    });

    createPrescriptionDialog.content.onSave.subscribe(() => {
      this.GetSummaryDetails();
    });
  }
  viewInvoice(invoiceId: number): void {
    let viewInvoiceDialog: BsModalRef;

    viewInvoiceDialog = this._modalService.show(ViewInvoiceComponent, {
      class: 'modal-xl',
      initialState: {
        id: invoiceId,   // Pass invoice id to modal
      },
    });
  }
  getStatusLabelForProcedure(value: number): string {
    const status = this.procedureStatus.find((s) => s.value === value);
    return status ? status.label : '';
  }
  getStatusSeverityForProcedure(value: number): 'info' | 'success' | 'danger' | 'secondary' {
    switch (value) {
      case EmergencyProcedureStatus._0:
        return 'info'; // Pending
      case EmergencyProcedureStatus._1:
        return 'success'; // Completed
      default:
        return 'secondary';
    }
  }
  markAsComplete(status: DischargeStatus) {
    this._summaryService.dischargeStatusChange(this.patientId, status)
      .subscribe({
        next: () => {
          this.data.patientDischarge.dischargeStatus = status;
        }
      });
  }
  checkForLabTechnicianNotify(): boolean {
    const filteredList = this.data.prescriptionLabTests.filter(x => x.testStatus != 2)
    if (filteredList.length > 0) {
      return true;
    }
  }
  checkForBillStaffNotify(): boolean {
    if (this.data.invoices.length <= 0) {
      return true;
    }
  }
  FinalApproval() {
    const rawSummary = this.data.patientDischarge.dischargeSummary || '';
    const plainText = rawSummary
      .replace(/<[^>]*>/g, '')  // remove HTML tags
      .replace(/&nbsp;/g, '')   // remove non-breaking spaces
      .trim();

    if (!plainText) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Validation Error',
        detail: 'Discharge summary cannot be empty.'
      });
      return;
    }

    this._summaryService.finalApproval(plainText, this.patientId, 1).subscribe({
      next: (res) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Discharge Approved',
          detail: 'The patient has been successfully approved for discharge.'
        });
        this.GetSummaryDetails();
      }, error: (err) => {

      }
    })
  }
  Discharge() {
    this._summaryService.finalDischarge(this.patientId).subscribe({
      next: (res) => {
        this.router.navigate(['app/doctors/patients'],);
      }, error: (err) => {

      }
    })
  }
}