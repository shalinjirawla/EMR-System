import { Component, EventEmitter, Output } from '@angular/core';
import { ControlValueAccessor, FormsModule } from '@angular/forms';
import { CommonModule } from '@node_modules/@angular/common';
import { fn } from 'moment';

@Component({
  selector: 'app-create-doctor',
  imports: [FormsModule,CommonModule],
  templateUrl: './create-doctor.component.html',
  styleUrl: './create-doctor.component.css'
})
export class CreateDoctorComponent implements ControlValueAccessor {
  @Output() doctorDataChange = new EventEmitter<any>();
  
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

  updateData() {
    this.onChange(this.doctorData);
    this.onTouched();
    this.doctorDataChange.emit(this.doctorData);
  }
}
