import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { DoctorDto, DoctorServiceProxy, NurseDto, NurseServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { forkJoin } from 'rxjs';


@Component({
  selector: 'app-edit-patients',
  imports: [FormsModule, CommonModule, AbpValidationSummaryComponent],
  providers: [DoctorServiceProxy, NurseServiceProxy],
  templateUrl: './edit-patients.component.html',
  styleUrl: './edit-patients.component.css'
})
export class EditPatientsComponent implements OnInit {

  @Output() patientDataChange = new EventEmitter<any>();
  @ViewChild('patientForm', { static: true }) patientForm: NgForm;
  
  bloodGroups = ['A+', 'A−', 'B+', 'B−', 'AB+', 'AB−', 'O+', 'O−'];
  genders = ['Male', 'Female', 'Other'];
  nurseList!: NurseDto[];
  doctorList!: DoctorDto[];

  @Input() patientData: {
    gender: string;
    dateOfBirth: string | null;
    address: string,
    bloodGroup: string,
    emergencyContactName: string,
    emergencyContactNumber: string,
    assignedNurseId: number,
    isAdmitted: boolean | false,
    admissionDate: string | null,
    dischargeDate: string | null,
    insuranceProvider: string,
    insurancePolicyNumber: string,
    assignedDoctorId: number
  };
  constructor(
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    private cdr: ChangeDetectorRef) {

  }

  ngOnInit() {
    this.loadAllStaff();
  }

  loadAllStaff() {
    forkJoin({
      doctors: this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId),
      nurses: this._nurseService.getAllNursesByTenantID(abp.session.tenantId)
    }).subscribe({
      next: ({ doctors, nurses }) => {
        this.doctorList = doctors.items;
        this.nurseList = nurses.items;
        console.log(this.doctorList, this.nurseList)

        if (!this.doctorList.find(d => d.id === this.patientData.assignedDoctorId)) {
          this.patientData.assignedDoctorId = null;
        }
        if (!this.nurseList.find(n => n.id === this.patientData.assignedNurseId)) {
          this.patientData.assignedNurseId = null;
        }

        this.cdr.detectChanges();
      }
    });
  }



  onInputChange() {
    this.updateData();
  }
  onAdmitChange() {
    if (!this.patientData.isAdmitted) {
      this.patientData.admissionDate = null;
    }
    this.onInputChange();
  }
  updateData() {
    this.patientDataChange.emit(this.patientData);
  }
}
