import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { BadgeModule } from 'primeng/badge';
import { AvatarModule } from 'primeng/avatar';
import { Component, Injector, ChangeDetectorRef, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table, TableModule } from 'primeng/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
import { CollectionStatus, CreatePrescriptionLabTestsServiceProxy, DischargeStatus, DischargeSummaryDto, EmergencyProcedureStatus, InvoiceDto, LabTestStatus, PatientDischargeServiceProxy, PrescriptionDto, PrescriptionLabTestDto, PrescriptionLabTestServiceProxy, PrescriptionServiceProxy, SelectedEmergencyProcedures, SelectedEmergencyProceduresDto, SelectedEmergencyProceduresServiceProxy, VitalDto } from '@shared/service-proxies/service-proxies';
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
  imports: [StepperModule, FormsModule, EditorModule, RouterLink, AccordionModule, DialogModule, StepsModule, TextareaModule, InputGroupModule, InputGroupAddonModule, CommonModule, TableModule, AvatarModule, BadgeModule, TabsModule, PaginatorModule, CheckboxModule,
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
  collectionStatusOptions = [
    { label: 'Not PickedUp', value: CollectionStatus._0 },
    { label: 'Picked Up', value: CollectionStatus._1 },
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

  // ----listing----
  GetSummaryDetails() {
    this._summaryService.patientDischargeSummary(this.patientId).subscribe({
      next: (res) => {
        this.data = res;
        console.log("1111111111:", res)
        this.currentStep = this.getStepFromStatus(this.data?.patientDischarge?.dischargeStatus);
        this.cd.detectChanges();
      }, error: (err) => {

      }
    })
  }

  // ---- post -----
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

    this._summaryService.finalApproval(rawSummary, this.patientId, 1).subscribe({
      next: (res) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Discharge Approved',
          detail: 'The patient has been successfully approved for discharge.'
        });
        this.GetSummaryDetails();
        this.cd.detectChanges();
      }, error: (err) => {

      }
    })
  }
  Discharge() {
    this._summaryService.finalDischarge(this.patientId).subscribe({
      next: (res) => {
        this.router.navigate(['app/patient-discharge/list'],);
        this.cd.detectChanges();
      }, error: (err) => {

      }
    })
  }
  ChangeStatus(status: DischargeStatus) {
    this._summaryService.dischargeStatusChange(this.patientId, status)
      .subscribe({
        next: () => {
          this.data.patientDischarge.dischargeStatus = status;
          this.GetSummaryDetails();
          this.cd.detectChanges();
        }
      });
  }

  // ---- view/create -----
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
    createPrescriptionDialog.onHide.subscribe(() => {
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
  DisplayVitalDetails(recordId: any) {
    this.visible = true;
    this.showVitalDetails = this.data.vitals.find(x => x.id == recordId);
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
  getStepFromStatus(status: DischargeStatus): number {
    switch (status) {
      case DischargeStatus._0:
        return 1; // Step 1 active
      case DischargeStatus._1:
        return 2; // Step 1 completed, move to 2
      case DischargeStatus._2:
        return 2; // Step 2 active
      case DischargeStatus._3:
        return 3; // Step 2 completed, move to 3
      case DischargeStatus._4:
        return 3; // Step 3 active
      case DischargeStatus._5:
        return 4; // Step 3 completed, move to 4
      case DischargeStatus._6:
        return 4; // Step 4 active
      case DischargeStatus._7:
        return 5; // Step 4 completed, move to 5
      case DischargeStatus._8:
        return 5; // Step 5 active
      case DischargeStatus._9:
        return 6; // Step 5 completed, optionally disable next
      default:
        return 1;
    }
  }

  /// ------display ---
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
  getShortName(fullName: string): string {
    if (!fullName) return '';
    const words = fullName.trim().split(' ');
    const firstInitial = words[0].charAt(0).toUpperCase();
    const lastInitial = words.length > 1 ? words[words.length - 1].charAt(0).toUpperCase() : '';
    return firstInitial + lastInitial;
  }
  getCollectionStatusLabel(value: number): string {
    const status = this.collectionStatusOptions.find(s => s.value === value);
    const dataa = status ? status.label : '';
    return dataa;
  }
  getCollectionStatusSeverity(value: number): 'info' | 'warn' | 'success' {
    switch (value) {
      case CollectionStatus._0: return 'warn';        // Pending
      case CollectionStatus._1: return 'success';   // In_review
      default: return 'info';
    }
  }
  /// check for validation
  checkForDoctorNotify(): boolean {
    const pendingPrescriptions = this.data?.prescriptions
      ?.filter(x => x.collectionStatus === CollectionStatus._0) ?? []
    if (pendingPrescriptions.length > 0) {
      return true;
    }
    const pendingProcedure = this.data?.selectedEmergencyProcedures
      ?.filter(x => x.status === EmergencyProcedureStatus._0) ?? [];

    if (pendingProcedure.length > 0) {
      return true;
    }
    return false;
  }
  checkForLabTechnicianNotify(): boolean {
    const pendingTests = this.data.prescriptionLabTests?.filter(x => x.testStatus !== LabTestStatus._2) || [];
    return pendingTests.length > 0;
  }
  checkForBillStaffNotify(): boolean {
    return !(this.data.invoices && this.data.invoices.length > 0);
  }
}