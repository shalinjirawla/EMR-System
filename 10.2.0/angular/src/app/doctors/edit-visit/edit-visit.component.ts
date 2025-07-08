import { ChangeDetectorRef, Component, Injector, OnInit, Output, ViewChild, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PatientDropDownDto, PaymentMode, DepartmentServiceProxy, DepartmentListDto, NurseDto, NurseServiceProxy, DoctorServiceProxy, CreateUpdateVisitDto, VisitServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { AppointmentDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import { TextareaModule } from 'primeng/textarea';
import { AppSessionService } from '@shared/session/app-session.service';
import { MultiSelectModule } from 'primeng/multiselect';
import { PermissionCheckerService } from '@node_modules/abp-ng2-module';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { DatePickerModule } from 'primeng/datepicker';
import moment from 'moment';
@Component({
  selector: 'app-edit-visit',
  imports: [
    FormsModule, DatePickerModule, CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent, MultiSelectModule
  ],
  templateUrl: './edit-visit.component.html',
  styleUrl: './edit-visit.component.css',
  providers: [VisitServiceProxy, DepartmentServiceProxy, DoctorServiceProxy, NurseServiceProxy, PatientServiceProxy, AppSessionService]
})
export class EditVisitComponent extends AppComponentBase implements OnInit {
  id!: number;
  @ViewChild('editVisitForm', { static: true }) createVisitForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  patients!: PatientDropDownDto[];
  appointments!: AppointmentDto[];
  doctorID!: number;
  showAddPatientButton = false;
  tomorrow!: Date;
  visit: any = {
    id: 0,
    tenantId: abp.session.tenantId,
    patientId: 0,
    departmentId: 0,
    nurseId: 0,
    dateOfVisit: null,
    timeOfVisit: '',
    reasonForVisit: '',
    paymentMode: PaymentMode._0,
    consultationFee: 0,
    doctorId: 0,
  };
  departments!: DepartmentListDto[];
  nurse!: NurseDto[];
  selectedPaymentMethod: string;
  minVisitDate: Date;
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _nurseService: NurseServiceProxy,
    private _patientService: PatientServiceProxy,
    private permissionChecker: PermissionCheckerService,
    private _modalService: BsModalService,
    private _departmentService: DepartmentServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _visitService: VisitServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.FillForm();
    this.showAddPatientButton = this.permissionChecker.isGranted('Pages.Users');
    this.FetchDoctorID();
    this.LoadPatients();
    this.LoadDepartMent();
    this.LoadNurse();
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    this.minVisitDate = today;
  }
  FillForm() {
    this._visitService.get(this.id).subscribe((result) => {
      this.visit.id = result.id;
      this.visit.tenantId = abp.session.tenantId;
      this.visit.patientId = result.patient.id;
      this.visit.departmentId = result.department.id;
      this.visit.doctorId = result.doctor.id;
      this.visit.nurseId = result.nurse.id;
      this.visit.dateOfVisit = result.dateOfVisit ? moment(result.dateOfVisit).toDate() : null;
      this.visit.timeOfVisit = result.timeOfVisit ? this.convertTimeToDate(result.timeOfVisit) : null;
      this.visit.reasonForVisit = result.reasonForVisit;
      this.visit.paymentMode = result.paymentMode;
      this.visit.consultationFee = result.consultationFee;
      this.cd.detectChanges();
    });
  }
  convertTimeToDate(timeStr: string): Date {
    const [hours, minutes, seconds] = timeStr.split(':').map(Number);
    const now = new Date();
    now.setHours(hours, minutes, seconds || 0, 0);
    return now;
  }
  LoadPatients() {
    this._patientService.patientDropDown().subscribe({
      next: (res) => {
        this.patients = res;
        this.cd.detectChanges();
      }, error: (err) => {
      }
    });
  }
  LoadDepartMent() {
    this._departmentService.getAllDepartmentByTenantID().subscribe(res => {
      this.departments = res.items;
      this.cd.detectChanges();
    })
  }
  LoadNurse() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.nurse = res.items;
        this.cd.detectChanges();
      }, error: (err) => {
      }
    })
  }
  FetchDoctorID() {
    this._doctorService.getDoctorDetailsByAbpUserID(abp.session.userId).subscribe({
      next: (res) => {
        this.doctorID = res.id;
        this.cd.detectChanges();
      }, error: (err) => {
      }
    });
  }
  isSaveDisabled(): boolean {
    if (!this.createVisitForm.valid || this.saving) {
      return true;
    }
  }
  save(): void {

    this.saving = true;
    // Create a proper DTO instance for the visit
    const input = new CreateUpdateVisitDto();
    input.id = this.visit.id;
    input.tenantId = abp.session.tenantId;
    input.patientId = this.visit.patientId;
    input.departmentId = this.visit.departmentId;
    input.doctorId = this.doctorID;
    input.nurseId = this.visit.nurseId;
    input.dateOfVisit = this.visit.dateOfVisit;
    input.timeOfVisit = this.dateToTimeString(this.visit.timeOfVisit);
    input.reasonForVisit = this.visit.reasonForVisit;
    input.paymentMode = this.visit.paymentMode;
    input.consultationFee = this.visit.consultationFee;
    this._visitService.update(input).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (err) => {
        this.saving = false;
        this.notify.error('Could not save visit');
      },
      complete: () => {
        this.saving = false;
      }
    });
  }
  dateToTimeString(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const seconds = date.getSeconds().toString().padStart(2, '0');
    return `${hours}:${minutes}:${seconds}`;
  }
  showCreatePatientDialog(id?: number): void {
    let createOrEditPatientDialog: BsModalRef;
    createOrEditPatientDialog = this._modalService.show(CreateUserDialogComponent, {
      class: 'modal-lg',
      initialState: {
        defaultRole: 'Patient',
        disableRoleSelection: true
      }
    });
    createOrEditPatientDialog.content.onSave.subscribe(() => {
      this.LoadPatients();
    });
  }
  togglePaymentMethod(method: string) {
    this.selectedPaymentMethod = method;
  }
}
