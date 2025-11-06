import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, Input, EventEmitter, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { PatientDropDownDto, PatientServiceProxy, DoctorDto, DoctorServiceProxy, NurseDto, NurseServiceProxy, RoomDto, RoomServiceProxy, AdmissionType, BillingMethod, CreateUpdateAdmissionDto, AdmissionServiceProxy, BedDto, BedServiceProxy, InsuranceMasterServiceProxy, CreateUpdatePatientInsuranceDto, InsuranceMasterDto } from '@shared/service-proxies/service-proxies';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';
import moment from 'moment';


@Component({
  selector: 'app-edit-addmission',
  templateUrl: './edit-addmission.component.html',
  styleUrl: './edit-addmission.component.css',
  providers: [PatientServiceProxy, InsuranceMasterServiceProxy, DoctorServiceProxy, BedServiceProxy, NurseServiceProxy, RoomServiceProxy, AdmissionServiceProxy],
  imports: [
    FormsModule, TextareaModule,
    CommonModule,
    SelectModule,
    DatePickerModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
  ]
})
export class EditAddmissionComponent extends AppComponentBase implements OnInit {
  @ViewChild('editAdmissionForm', { static: true }) editAdmissionForm: NgForm;
  @Input() id: number;
  @Output() onSave = new EventEmitter<void>();

  saving = false;

  BillingMethod = BillingMethod;

  patients: PatientDropDownDto[] = [];
  doctors: DoctorDto[] = [];
  nurses: NurseDto[] = [];
  rooms: RoomDto[] = [];
  beds: any[] = [];
  insurances: InsuranceMasterDto[] = [];

  get roomOptions() {
    return this.rooms.map(x => ({ label: `${x.roomNumber} – ${x.roomTypeName}`, value: x.id }));
  }

  admissionTypeOptions = [
    { label: 'In-Patient', value: AdmissionType._0 },
    { label: 'Day-Care', value: AdmissionType._1 },
    { label: 'Emergency', value: AdmissionType._2 }
  ];

  billingMethodOptions = [
    { label: 'Self Pay (No Insurance)', value: BillingMethod._0 },
    { label: 'Insurance + Self Pay', value: BillingMethod._1 },
    { label: 'Insurance Only (Cashless)', value: BillingMethod._2 }
  ];

  admission: any = {};

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _roomService: RoomServiceProxy,
    private _bedService: BedServiceProxy,
    private _insuranceService: InsuranceMasterServiceProxy,
    private _admissionService: AdmissionServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadPatients();
    this.loadDoctors();
    this.loadNurses();
    this.loadRooms();
    this.loadInsurances();
    this.loadAdmission();
  }

  loadPatients() { this._patientService.patientDropDown().subscribe(r => { this.patients = r; this.cd.detectChanges(); }); }
  loadDoctors() { this._doctorService.getAllDoctors().subscribe(r => { this.doctors = r.items; this.cd.detectChanges(); }); }
  loadNurses() { this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(r => { this.nurses = r.items; this.cd.detectChanges(); }); }
  loadRooms() { this._roomService.getAvailableRooms(abp.session.tenantId).subscribe(r => { this.rooms = r; this.cd.detectChanges(); }); }
  loadInsurances() { this._insuranceService.getAllInsuranceForDropdown().subscribe(r => { this.insurances = r.items; this.cd.detectChanges(); }); }

  loadAdmission() {
    this._admissionService.get(this.id).subscribe(res => {
      this.admission = res;
      if (this.admission.roomId) this.onRoomChange(this.admission.roomId);
      this.cd.detectChanges();
    });
  }

  onRoomChange(roomId: number) {
    this._bedService.getAvailableBedsByRoom(abp.session.tenantId, roomId, this.admission.id)
      .subscribe(r => { this.beds = r; this.cd.detectChanges(); });
  }

  save() {
    this.saving = true;

    const input = new CreateUpdateAdmissionDto();
    Object.assign(input, this.admission);
    input.admissionDateTime = moment();

    if (this.admission.billingMode != BillingMethod._0) {
      const p = new CreateUpdatePatientInsuranceDto();
      p.id = this.admission.patientInsuranceId;
      p.tenantId = this.admission.tenantId;
      p.patientId = this.admission.patientId;
      p.insuranceId = this.admission.insuranceId;
      p.policyNumber = this.admission.policyNumber;
      p.coverageLimit = this.admission.coverageLimit;
      p.coPayPercentage = this.admission.coPayPercentage;
      p.isActive = true;

      input.patientInsurance = p;
    }
    this._admissionService.update(input).subscribe(() => {
      this.notify.success('Updated Successfully');
      this.bsModalRef.hide();
      this.onSave.emit();
    }).add(() => this.saving = false);
  }
}
