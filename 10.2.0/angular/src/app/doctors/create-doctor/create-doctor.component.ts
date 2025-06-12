import { Component, EventEmitter, forwardRef, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, NgForm } from '@angular/forms';
import { CommonModule } from '@node_modules/@angular/common';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';
import { fn } from 'moment';

@Component({
  selector: 'app-create-doctor',
  imports: [FormsModule,CommonModule,AbpValidationSummaryComponent],
  templateUrl: './create-doctor.component.html',
  styleUrl: './create-doctor.component.css',
})
export class CreateDoctorComponent  {
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