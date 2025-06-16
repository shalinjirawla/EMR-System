import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppointmentServiceProxy, PrescriptionItemDto } from '@shared/service-proxies/service-proxies';
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
@Component({
  selector: 'app-create-prescriptions',
  standalone: true,
  imports: [
    FormsModule, CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent
  ],
  templateUrl: './create-prescriptions.component.html',
  styleUrls: ['./create-prescriptions.component.css'],
  providers: [DoctorServiceProxy, PatientServiceProxy, AppointmentServiceProxy, AppSessionService]
})
export class CreatePrescriptionsComponent extends AppComponentBase implements OnInit {
  @ViewChild('prescriptionForm', { static: true }) prescriptionForm: NgForm;
  saving = false;
  patients!: PatientDto[];
  appointments!: AppointmentDto[];
  // prescription: CreateUpdatePrescriptionDto = new CreateUpdatePrescriptionDto();
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
  ) {
    super(injector);
  }

  ngOnInit(): void {
    // this.LoadAppoinments();
    this.LoadPatients();
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
    if (!patientId) return;

    // this._appointmentService.getAllAppointmentsByPatientID(patientId, abp.session.tenantId).subscribe({
    //   next: (res) => {
    //     debugger
    //     this.appointments = res.items;
    //   }, error: (err) => {
    //     debugger
    //   }
    // })
  }

  addItem(): void {
    const item = new PrescriptionItemDto();
    item.id = 0;
    item.tenantId = abp.session.tenantId;
    item.medicineName = '';
    item.dosage = '';
    item.frequency = '';
    item.duration = '';
    item.instructions = '';
    item.prescription = undefined;

    if (!this.prescription.items) {
      this.prescription.items = [];
    }

    this.prescription.items.push(item);
  }

  removeItem(index: number): void {
    this.prescription.items.splice(index, 1);
  }

  save(): void {
    debugger
    if (!this.prescriptionForm.valid) {
      this.notify.warn('Please fill all required fields');
      return;
    }
    const input = {
      ...this.prescription,
      issueDate: this.prescription.issueDate?.toISOString()
    };
    setTimeout(() => {
      this.notify.success('Prescription saved successfully!');
      this.saving = false;
      this.bsModalRef.hide();
    }, 1000);
  }
}
