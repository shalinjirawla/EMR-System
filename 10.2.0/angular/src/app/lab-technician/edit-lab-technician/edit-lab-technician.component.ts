import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormsModule, ControlValueAccessor, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-edit-lab-technician',
  imports: [CommonModule, FormsModule, AbpValidationSummaryComponent],
  templateUrl: './edit-lab-technician.component.html',
  styleUrl: './edit-lab-technician.component.css'
})
export class EditLabTechnicianComponent  {
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