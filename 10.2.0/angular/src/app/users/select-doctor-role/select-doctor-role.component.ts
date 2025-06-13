import { Component, EventEmitter, forwardRef, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { fn } from 'moment';


@Component({
  selector: 'app-select-doctor-role',
  imports: [FormsModule,CommonModule,AbpValidationSummaryComponent],
  templateUrl: './select-doctor-role.component.html',
  styleUrl: './select-doctor-role.component.css'
})
export class SelectDoctorRoleComponent {
  @Output() doctorDataChange = new EventEmitter<any>();
  @ViewChild('doctorForm', { static: true }) doctorForm: NgForm;

  doctorData = {
    phoneNumber: '',
    gender: 'Male',
    specialization: '',
    qualification: '',
    yearsOfExperience: 0,
    department: '',
    registrationNumber: '',
    dateOfBirth: null
  };

  genders = ['Male', 'Female', 'Other'];



  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.doctorDataChange.emit(this.doctorData);
  }
}
