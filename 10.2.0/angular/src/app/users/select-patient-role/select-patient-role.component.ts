import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AbpValidationSummaryComponent } from "../../../shared/components/validation/abp-validation.summary.component";
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NurseDto, DoctorDto, DoctorServiceProxy, NurseServiceProxy, PaymentMethod, BillingMethod, RoomDto, RoomServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-select-patient-role',
  imports: [AbpValidationSummaryComponent, FormsModule, CommonModule],
  providers: [RoomServiceProxy],
  templateUrl: './select-patient-role.component.html',
  styleUrl: './select-patient-role.component.css'
})
export class SelectPatientRoleComponent implements OnInit {
  @Output() patientDataChange = new EventEmitter<any>();
  @ViewChild('patientForm', { static: true }) patientForm: NgForm;
  genders = ['Male', 'Female', 'Other'];
  bloodGroups = ['A+', 'A−', 'B+', 'B−', 'AB+', 'AB−', 'O+', 'O−'];
  roomList!: RoomDto[];
  paymentMethodOptions = [
    { label: 'Cash', value: PaymentMethod._0 },
    { label: 'Card', value: PaymentMethod._1 }
  ];
  billingMethods = [
    { label: 'Insurance Only', value: BillingMethod._0 },
    { label: 'Self Pay', value: BillingMethod._1 },
    { label: 'Insurance + SelfPay', value: BillingMethod._2 }
  ];
  BillingMethod = BillingMethod; // Add this line
  maxDate: string;
  nurseList!: NurseDto[];
  doctorList!: DoctorDto[];
  isDoctorLoggedIn: boolean = false;
  patient = {
    gender: '',
    dateOfBirth: null,
    address: '',
    bloodGroup: '',
    emergencyContactName: '',
    emergencyContactNumber: '',
    //assignedNurseId: null,
    //billingMethod: null,
    //paymentMethod: null,
    //depositAmount:0,
    //roomId: null,
    //isAdmitted: false,
    //admissionDate: null,
    // dischargeDate: null,
    // insuranceProvider: '',
    // insurancePolicyNumber: '',
    //assignedDoctorId: null
  };

  constructor(
    //   private _doctorService: DoctorServiceProxy,
    //   private _nurseService: NurseServiceProxy,
    // private _roomService: RoomServiceProxy
  ) {
    // this.GetLoggedInUserRole();
    //this.LoadDoctors();
    //this.LoadNurse();
    //this.LoadRooms(); 
  }
  ngOnInit(): void {
    const today = new Date();
    this.maxDate = today.toISOString().split('T')[0];
  }
  //  LoadRooms() {
  //     this._roomService.getAvailableRooms(abp.session.tenantId).subscribe({
  //       next: (res) => {          // अगर API simple list return करती है
  //         this.roomList = res;    // (PagedResult हो तो res.items दें)
  //       },
  //       error: () => { }
  //     });
  //   }
  //   LoadDoctors() {
  //     this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId).subscribe({
  //       next: (res) => {
  //         this.doctorList = res.items;
  //       }, error: (err) => {
  //       }
  //     })
  //   }
  //   LoadNurse() {
  //     this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
  //       next: (res) => {
  //         this.nurseList = res.items;
  //       }, error: (err) => {
  //       }
  //     })
  //   }

  onInputChange() {
    this.patientDataChange.emit(this.patient);
  }

  // onAdmitChange() {
  //   if (!this.patient.isAdmitted) {
  //     this.patient.admissionDate = null;
  //   }
  //   this.onInputChange();
  // }
  // onInputChange() {
  //   if (this.patient.billingMethod === BillingMethod._0) {
  //     this.patient.paymentMethod = null;
  //   }
  //   this.patientDataChange.emit(this.patient);
  // }
  // GetLoggedInUserRole() {
  //   this._doctorService.getCurrentUserRoles().subscribe(res => {
  //     if (res && res.includes('Doctors')) {
  //       this.isDoctorLoggedIn = true;
  //     } else {
  //       this.isDoctorLoggedIn = false;
  //     }
  //   })
  // }
}
