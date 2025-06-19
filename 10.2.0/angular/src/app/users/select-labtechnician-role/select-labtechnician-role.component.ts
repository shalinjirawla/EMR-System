import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-select-labtechnician-role',
  imports: [CommonModule, FormsModule, AbpValidationSummaryComponent],
  templateUrl: './select-labtechnician-role.component.html',
  styleUrl: './select-labtechnician-role.component.css'
})
export class SelectLabtechnicianRoleComponent {
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

  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.technicianDataChange.emit(this.technicianData);
  }


}
