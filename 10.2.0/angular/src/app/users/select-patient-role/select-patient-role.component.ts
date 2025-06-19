import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { AbpValidationSummaryComponent } from "../../../shared/components/validation/abp-validation.summary.component";
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NurseDto, DoctorDto, DoctorServiceProxy, NurseServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-select-patient-role',
  imports: [AbpValidationSummaryComponent,FormsModule,CommonModule],
  templateUrl: './select-patient-role.component.html',
  styleUrl: './select-patient-role.component.css'
})
export class SelectPatientRoleComponent {
   @Output() patientDataChange = new EventEmitter<any>();
    @ViewChild('patientForm', { static: true }) patientForm: NgForm;

  genders = ['Male', 'Female', 'Other'];
bloodGroups = ['A+', 'A−', 'B+', 'B−', 'AB+', 'AB−', 'O+', 'O−'];
  nurseList!: NurseDto[];
  doctorList!: DoctorDto[];
patient = {
  gender: '',
  dateOfBirth: null,
  address: '',
  bloodGroup: '',
  emergencyContactName: '',
  emergencyContactNumber: '',
  assignedNurseId: null,
  isAdmitted: false,
  admissionDate: null,
  dischargeDate: null,
  insuranceProvider: '',
  insurancePolicyNumber: '',
  assignedDoctorId: null
};

constructor(
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,)
    {
      this.LoadDoctors();
      this.LoadNurse();
    }

LoadDoctors() {
    this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.doctorList = res.items;
      }, error: (err) => {
      }
    })
  }
  LoadNurse() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.nurseList = res.items;
      }, error: (err) => {
      }
    })
  }
// nurseList = [
//   { id: 1, name: 'Nurse A' },
//   { id: 2, name: 'Nurse B' }
// ];

// doctorList = [
//   { id: 101, name: 'Dr. John' },
//   { id: 102, name: 'Dr. Smith' }
// ];

 onInputChange() {
    this.patientDataChange.emit(this.patient);
  }

  onAdmitChange() {
    if (!this.patient.isAdmitted) {
      this.patient.admissionDate = null;
    }
    this.onInputChange();
  }
}
