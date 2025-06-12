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
export class CreateNurseComponent  {
  @Output() nurseDataChange = new EventEmitter<any>();
  @ViewChild('nurseForm', { static: true }) nurseForm: NgForm;

  nurseData = {
    phoneNumber: '',
    gender: '',
    shiftTiming: '',
    department: '',
    qualification: '',
    yearsOfExperience: 0,
    dateOfBirth:null
  };

  genders = ['Male', 'Female', 'Other'];

 
  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.nurseDataChange.emit(this.nurseData);
  }
 
}
