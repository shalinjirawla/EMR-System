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
   providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CreateDoctorComponent),
      multi: true
    }
  ]
})
export class CreateDoctorComponent implements ControlValueAccessor {
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

  private onChange: any = () => {};
  private onTouched: any = () => {};

  writeValue(value: any): void {
    if (value) {
      this.doctorData = value;
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.onChange(this.doctorData);
    this.onTouched();
    this.doctorDataChange.emit(this.doctorData);
  }

  resetForm() {
    this.doctorData = {
      phoneNumber: '',
      gender: 'Male',
      specialization: '',
      qualification: '',
      yearsOfExperience: 0,
      department: '',
      registrationNumber: '',
      dateOfBirth: null
    };
    this.updateData();
  }
}