import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-edit-doctor',
  imports: [FormsModule,CommonModule,AbpValidationSummaryComponent],
  templateUrl: './edit-doctor.component.html',
  styleUrl: './edit-doctor.component.css'
})
export class EditDoctorComponent  {
   @Output() doctorDataChange = new EventEmitter<any>();
    @ViewChild('doctorForm', { static: true }) doctorForm: NgForm;
 
   doctorData = {
     phoneNumber: '',
     gender: 'Male',
     specialization: '',
     qualification: '',
     yearsOfExperience: 1,
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
