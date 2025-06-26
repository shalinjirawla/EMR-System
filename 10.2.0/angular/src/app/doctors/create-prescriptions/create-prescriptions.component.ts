import { ChangeDetectorRef, Component, Injector, OnInit, Output, ViewChild,EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppointmentServiceProxy, CreateUpdatePrescriptionItemDto, LabReportsTypeServiceProxy, PrescriptionItemDto, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { AppointmentDto, CreateUpdatePrescriptionDto, DoctorDto, DoctorServiceProxy, PatientDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { TextareaModule } from 'primeng/textarea';
import { AppSessionService } from '@shared/session/app-session.service';
import { MultiSelectModule } from 'primeng/multiselect';

@Component({
  selector: 'app-create-prescriptions',
  standalone: true,
  imports: [
    FormsModule, CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent,MultiSelectModule
  ],
  templateUrl: './create-prescriptions.component.html',
  styleUrls: ['./create-prescriptions.component.css'],
  providers: [DoctorServiceProxy, PatientServiceProxy,LabReportsTypeServiceProxy, AppointmentServiceProxy, AppSessionService, PrescriptionServiceProxy]
})
export class CreatePrescriptionsComponent extends AppComponentBase implements OnInit {
  @ViewChild('prescriptionForm', { static: true }) prescriptionForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  patients!: PatientDto[];
  appointments!: AppointmentDto[];
  labTests: any[] = [];
  selectedLabTests: any[] = []; 
  doctorID!: number;
  prescription: CreateUpdatePrescriptionDto = {
    id: 0,
    tenantId: abp.session.tenantId,
    diagnosis: '',
    notes: '',
    issueDate: moment(),
    isFollowUpRequired: false,
    appointmentId: 0,
    doctorId: 0,
    patientId: 0,
    items: [],
    labTestIds:[],
    init: function (_data?: any): void {
      throw new Error('Function not implemented.');
    },
    toJSON: function (data?: any) {
      throw new Error('Function not implemented.');
    },
    clone: function (): CreateUpdatePrescriptionDto {
      throw new Error('Function not implemented.');
    }
  };
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _doctorService: DoctorServiceProxy,
    private _patientService: PatientServiceProxy,
    private _appointmentService: AppointmentServiceProxy,
    private _sessionService: AppSessionService,
    private _prescriptionService: PrescriptionServiceProxy,
    private _labService:LabReportsTypeServiceProxy
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.FetchDoctorID();
    this.LoadPatients();
     this.LoadLabReports();
  }
  LoadPatients() {
    this._patientService.getAllPatientByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.patients = res.items;
      }, error: (err) => {
      }
    })
  }
  LoadAppoinments() {
    const patientId = this.prescription.patientId;
    const doctorId = this.doctorID;
    if (!patientId) return;
    if (!doctorId) return;
    this._appointmentService.getPatientAppointment(patientId, doctorId).subscribe({
      next: (res) => {
        this.appointments = res.items;
        this.appointments.forEach(app => {
          app['title'] = `${app.startTime} - ${app.endTime} -${app.patient?.fullName}`;
        });
      }, error: (err) => {
      }
    })
  }
  LoadLabReports() {
    this._labService.getAllTestByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.labTests = res.items.map(item => ({
          id: item.id,
          name: item.reportType // 'ReportType' column ka data
        }));
      },
      error: (err) => {
        this.notify.error('Could not load lab tests');
      }
    });
  }
  addItem(): void {
    const item = new CreateUpdatePrescriptionItemDto();
    item.id = 0;
    item.tenantId = abp.session.tenantId;
    item.medicineName = '';
    item.dosage = '';
    item.frequency = '';
    item.duration = '';
    item.instructions = '';
    item.prescriptionId = 0;

    if (!this.prescription.items) {
      this.prescription.items = [];
    }

    this.prescription.items.push(item);
  }
  removeItem(index: number): void {
    this.prescription.items.splice(index, 1);
  }
  FetchDoctorID() {
    this._doctorService.getDoctorDetailsByAbpUserID(abp.session.userId).subscribe({
      next: (res) => {
        this.doctorID = res.id;
      }, error: (err) => {

      }
    })
  }
  isSaveDisabled(): boolean {
    if (!this.prescriptionForm.valid || this.saving) {
      return true;
    }
    if (!this.prescription.items || this.prescription.items.length === 0) {
      return true;
    }

    return this.prescription.items.some(item =>
      !item.medicineName?.trim() ||
      !item.dosage?.trim() ||
      !item.frequency?.trim() ||
      !item.duration?.trim() ||
      !item.instructions?.trim()
    );
  }
  save(): void {
    const input = new CreateUpdatePrescriptionDto();
    input.tenantId = this.prescription.tenantId;
    input.diagnosis = this.prescription.diagnosis;
    input.notes = this.prescription.notes;
    input.issueDate = this.prescription.issueDate;
    input.isFollowUpRequired = this.prescription.isFollowUpRequired;
    input.appointmentId = this.prescription.appointmentId;
    input.doctorId = this.doctorID;
    input.patientId = this.prescription.patientId;
    input.items = this.prescription.items;
    input.labTestIds = this.selectedLabTests.map(test => test.id || test);
    debugger
    this._prescriptionService.createPrescriptionWithItem(input).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (err) => {
        this.saving = false;
      }
    });
  }
}
