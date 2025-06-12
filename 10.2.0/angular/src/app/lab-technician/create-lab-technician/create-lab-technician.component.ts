import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-create-lab-technician',
  imports: [CommonModule, FormsModule, AbpValidationSummaryComponent],
  templateUrl: './create-lab-technician.component.html',
  styleUrl: './create-lab-technician.component.css'
})
export class CreateLabTechnicianComponent {
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