import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-select-nurse-role',
  imports: [FormsModule, CommonModule, AbpValidationSummaryComponent],
  templateUrl: './select-nurse-role.component.html',
  styleUrl: './select-nurse-role.component.css'
})
export class SelectNurseRoleComponent {
  @Output() nurseDataChange = new EventEmitter<any>();
  @ViewChild('nurseForm', { static: true }) nurseForm: NgForm;

  nurseData = {
    phoneNumber: '',
    gender: '',
    shiftTiming: '',
    department: '',
    qualification: '',
    yearsOfExperience: 0,
    dateOfBirth: null
  };

  genders = ['Male', 'Female', 'Other'];


  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.nurseDataChange.emit(this.nurseData);
  }

}
