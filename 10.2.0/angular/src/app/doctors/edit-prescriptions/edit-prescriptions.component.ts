import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { AppointmentDto, AppointmentServiceProxy, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionItemDto, DoctorServiceProxy, PatientDto, PatientServiceProxy, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-edit-prescriptions',
  imports: [
    FormsModule, AbpModalHeaderComponent,
    AbpModalFooterComponent, CommonModule,
    CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent
  ],
  templateUrl: './edit-prescriptions.component.html',
  styleUrl: './edit-prescriptions.component.css',
  providers: [PrescriptionServiceProxy, PatientServiceProxy, AppointmentServiceProxy],
})
export class EditPrescriptionsComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @ViewChild('editPrescriptionForm', { static: true }) editPrescriptionForm: NgForm;

  id: number;
  saving = false;
  prescription = new CreateUpdatePrescriptionDto();
  patients!: PatientDto[];
  appointmentTitle: { id: number, title: string }[] = [];
  doctorID!: number;
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _prescriptionService: PrescriptionServiceProxy,
    private _patientService: PatientServiceProxy,
    private _appointmentService: AppointmentServiceProxy,
  ) { super(injector); }


  ngOnInit(): void {
    this.LoadPatients();
    this.FillEditForm();
  }
  LoadPatients() {
    this._patientService.getAllPatientByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.patients = res.items;
      }, error: (err) => {
      }
    })
  }
  FillEditForm() {
    this._prescriptionService.getPrescriptionDetailsById(this.id).subscribe((result) => {
      this.prescription.id = result.id;
      this.prescription.tenantId = result.tenantId;
      this.prescription.diagnosis = result.diagnosis;
      this.prescription.notes = result.notes;
      this.prescription.issueDate = result.issueDate;
      this.prescription.isFollowUpRequired = result.isFollowUpRequired;
      this.prescription.appointmentId = result.appointmentId;
      this.prescription.doctorId = result.doctorId;
      this.prescription.patientId = result.patientId;
      this.prescription.items = result.items.map(i => {
        const dto = new CreateUpdatePrescriptionItemDto();
        dto.id = i.id;
        dto.tenantId = i.tenantId;
        dto.medicineName = i.medicineName;
        dto.dosage = i.dosage;
        dto.frequency = i.frequency;
        dto.duration = i.duration;
        dto.instructions = i.instructions;
        dto.prescriptionId = i.prescriptionId;
        return dto;
      });
      this._appointmentService.get(this.prescription.appointmentId).subscribe((app) => {
        const selectedPatient = this.patients.find(p => p.id === this.prescription.patientId);
        const title = `${app.appointmentTimeSlot} - ${selectedPatient.fullName}`;
        this.appointmentTitle = [{ id: app.id, title: title }];
        this.cd.detectChanges();
      });
      this.cd.detectChanges();
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
  isSaveDisabled() {
    if (!this.editPrescriptionForm.valid || this.saving) {
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
    this.saving = true;
    this._prescriptionService.update(this.prescription).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      }, error: (err) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      }
    })
  }
}
