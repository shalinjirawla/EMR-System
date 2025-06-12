import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-create-nurse',
  imports: [FormsModule, CommonModule, AbpValidationSummaryComponent],
  templateUrl: './create-nurse.component.html',
  styleUrl: './create-nurse.component.css'
})
export class CreateNurseComponent implements ControlValueAccessor {
  @Output() nurseDataChange = new EventEmitter<any>();
  @ViewChild('nurseForm', { static: true }) nurseForm: NgForm;

  nurseData = {
    phoneNumber: '',
    gender: '',
    shiftTiming: '',
    department: '',
    qualification: '',
    yearsOfExperience: 0
  };

  genders = ['Male', 'Female', 'Other'];

  private onChange: any = () => { };
  private onTouched: any = () => { };

  writeValue(value: any): void {
    if (value) {
      this.nurseData = value;
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
    this.onChange(this.nurseData);
    this.onTouched();
    this.nurseDataChange.emit(this.nurseData);
  }
  resetForm() {
    this.nurseData = {
      phoneNumber: '',
      gender: '',
      shiftTiming: '',
      department: '',
      qualification: '',
      yearsOfExperience: 0
    };
    this.updateData();
  }
}
