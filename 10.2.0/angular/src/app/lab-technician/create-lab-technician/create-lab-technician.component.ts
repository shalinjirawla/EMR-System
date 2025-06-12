import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-create-lab-technician',
  imports: [CommonModule,FormsModule,AbpValidationSummaryComponent],
  templateUrl: './create-lab-technician.component.html',
  styleUrl: './create-lab-technician.component.css'
})
export class CreateLabTechnicianComponent implements ControlValueAccessor {
  @Output() technicianDataChange = new EventEmitter<any>();
@ViewChild('labTechnicianForm', { static: true }) labTechnicianForm: NgForm; 

 technicianData = {
   
    gender: 'Male',    
    qualification: '',
    yearsOfExperience: 0,
    department: '',
    certificationNumber: '',
    dateOfBirth: null
  };
 genders = ['Male', 'Female', 'Other'];

  private onChange: any = () => {};
  private onTouched: any = () => {};

  writeValue(value: any): void {
    if (value) {
      this.technicianData = value;
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
    this.onChange(this.technicianData);
    this.onTouched();
    this.technicianDataChange.emit(this.technicianData);
  }

  resetForm() {
    this.technicianData = {
      
    gender: 'Male',    
    qualification: '',
    yearsOfExperience: 0,
    department: '',
    certificationNumber: '',
    dateOfBirth: null

    };
    this.updateData();
  }
}