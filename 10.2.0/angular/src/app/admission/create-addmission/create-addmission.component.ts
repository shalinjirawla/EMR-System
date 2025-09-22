import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, EventEmitter, Output, Input } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { PatientDropDownDto, PatientServiceProxy, DoctorDto, DoctorServiceProxy, NurseDto, NurseServiceProxy, RoomDto, RoomServiceProxy, AdmissionType, BillingMethod, CreateUpdateAdmissionDto, AdmissionServiceProxy, BedServiceProxy } from '@shared/service-proxies/service-proxies';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import moment from 'moment';

@Component({
  selector: 'app-create-addmission',
  templateUrl: './create-addmission.component.html',
  styleUrl: './create-addmission.component.css',
  providers: [PatientServiceProxy, DoctorServiceProxy, BedServiceProxy, NurseServiceProxy, RoomServiceProxy, AdmissionServiceProxy],
  imports: [
    FormsModule,
    CommonModule,
    SelectModule,
    DatePickerModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
  ]
})
export class CreateAddmissionComponent extends AppComponentBase implements OnInit {
  @ViewChild('createAdmissionForm', { static: true }) createAdmissionForm: NgForm;
  saving = false;
  patients: PatientDropDownDto[] = [];
  doctors: DoctorDto[] = [];
  nurses: NurseDto[] = [];
  rooms: RoomDto[] = [];
  beds: any[] = [];
  get roomOptions() {
    return this.rooms.map(room => ({
      label: `${room.roomNumber} – ${room.roomTypeName}`,
      value: room.id
    }));
  }
  admissionTypeOptions = [
    { label: 'In-Patient', value: AdmissionType._0 },
    { label: 'Day-Care', value: AdmissionType._1 },
    { label: 'Emergency', value: AdmissionType._2 }
  ];
  billingMethodOptions = [
    //{ label: 'Insurance Only', value: BillingMethod._0 },
    { label: 'Self Pay(No Insurance)', value: BillingMethod._1 },
    { label: 'Insurance + Self Pay(Insurance But not Cashless)', value: BillingMethod._2 }
  ];
  @Input() disableRoleSelection: boolean = false;
  @Input() selectedPatientId: number = 0;
  @Input() selectedNurseId: number = 0;
  @Input() selectDoctorId: number = 0;
  @Input() admissionType: number = 0;
  admission: any = {
    tenantId: abp.session.tenantId,
    patientId: null,
    admissionDateTime: null,
    doctorId: null,
    nurseId: null,
    roomId: null,
    admissionType: null,
  };
  @Output() onSave = new EventEmitter<any>();
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _bedService: BedServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _roomService: RoomServiceProxy,
    private _admissionService: AdmissionServiceProxy
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.loadPatients();
    this.loadDoctors();
    this.loadNurses();
    this.loadRooms();
    if (this.selectedPatientId>0) {
      this.admission.patientId = this.selectedPatientId;
    }
    if (this.selectDoctorId>0) {
      this.admission.doctorId = this.selectDoctorId;
    }
    if (this.selectedNurseId>0) {
      this.admission.nurseId = this.selectedNurseId;
    }
    if (this.admissionType>0) {
      this.admission.admissionType=AdmissionType._2;
    }
  }
  loadPatients() {
    this._patientService.getOpdPatients().subscribe(res => {
      this.patients = res;
      this.cd.detectChanges();
    });
  }
  loadDoctors() {
    this._doctorService.getAllDoctors().subscribe(res => {
      this.doctors = res.items;
      this.cd.detectChanges();
    });
  }
  loadNurses() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(res => {
      this.nurses = res.items;
      this.cd.detectChanges();
    });
  }
  loadRooms() {
    this._roomService.getAvailableRooms(abp.session.tenantId).subscribe(res => {
      this.rooms = res;
      this.cd.detectChanges();
    });
  }

  onRoomChange(roomId: number) {
    if (!roomId) {
      this.beds = [];
      this.admission.bedId = null;
      return;
    }

    this._bedService.getAvailableBedsByRoom(this.admission.tenantId, roomId,this.admission.id).subscribe(res => {
      this.beds = res;
      this.cd.detectChanges();
    });
  }

  save() {
    if (!this.createAdmissionForm?.form?.valid) {
      this.message.warn('Please complete the form properly.');
      return;
    }
    this.saving = true;
    const input = new CreateUpdateAdmissionDto();
    input.tenantId = this.admission.tenantId;
    input.patientId = this.admission.patientId;
    input.admissionDateTime = moment();
    input.doctorId = this.admission.doctorId;
    input.nurseId = this.admission.nurseId;
    input.roomId = this.admission.roomId;
    input.bedId = this.admission.bedId
    input.admissionType = this.admission.admissionType;

    this._admissionService.create(input).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.saving = false;
        this.bsModalRef.hide();
        this.onSave.emit(res.id);
      },
      error: (err) => {
        this.saving = false;
      }
    });
  }
}
