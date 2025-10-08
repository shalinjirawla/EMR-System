import { Component, EventEmitter, ChangeDetectorRef, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AbpValidationSummaryComponent } from "../../../shared/components/validation/abp-validation.summary.component";
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NurseDto, DoctorDto, DoctorServiceProxy, NurseServiceProxy, PaymentMethod, BillingMethod, RoomDto, RoomServiceProxy } from '@shared/service-proxies/service-proxies';


import { DatePickerModule } from 'primeng/datepicker';
import { CreateUpdateDoctorDto, DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';

@Component({
  selector: 'app-select-patient-role',
  imports: [AbpValidationSummaryComponent, FormsModule, RadioButtonModule, InputTextModule,TextareaModule,
    DatePickerModule, InputNumberModule, CheckboxModule, SelectModule, CommonModule],
  providers: [RoomServiceProxy],
  templateUrl: './select-patient-role.component.html',
  styleUrl: './select-patient-role.component.css'
})
export class SelectPatientRoleComponent implements OnInit {
  @Output() patientDataChange = new EventEmitter<any>();
  @ViewChild('patientForm', { static: true }) patientForm: NgForm;
  today: Date = new Date();
  genders = ['Male', 'Female', 'Other'];
  bloodGroups = [
    { label: 'A+', value: 'A+' },
    { label: 'A−', value: 'A−' },
    { label: 'B−', value: 'B−' },
    { label: 'B+', value: 'B+' },
    { label: 'AB+', value: 'AB+' },
    { label: 'AB-', value: 'AB-' },
    { label: 'O+', value: 'O+' },
    { label: 'O-', value: 'O-' },
  ];
  patient = {
    gender: '',
    dateOfBirth: null,
    address: '',
    bloodGroup: '',
    emergencyContactName: '',
    emergencyContactNumber: '',
  };

  constructor(
  ) {
  }
  ngOnInit(): void {
  }

  onInputChange() {
    this.patientDataChange.emit(this.patient);
  }
}
