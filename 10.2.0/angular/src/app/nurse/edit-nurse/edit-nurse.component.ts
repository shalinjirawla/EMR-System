import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule, ControlValueAccessor, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-edit-nurse',
  imports: [FormsModule, CommonModule, AbpValidationSummaryComponent],
  templateUrl: './edit-nurse.component.html',
  styleUrl: './edit-nurse.component.css'
})
export class EditNurseComponent {
  @Output() nurseDataChange = new EventEmitter<any>();
  @ViewChild('nurseForm', { static: true }) nurseForm: NgForm;

  
   @Input() nurseData: {
      gender: string;
       shiftTiming: string;
      qualification: string;
      yearsOfExperience: number;
      department: string;
      dateOfBirth: string | null;
    };

  genders = ['Male', 'Female', 'Other'];

  
  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.nurseDataChange.emit(this.nurseData);
  }
  
}
