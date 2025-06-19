import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';

@Component({
  selector: 'app-edit-doctor',
  imports: [FormsModule,CommonModule,AbpValidationSummaryComponent],
  templateUrl: './edit-doctor.component.html',
  styleUrl: './edit-doctor.component.css'
})
export class EditDoctorComponent implements OnChanges {
  @Input() doctorData: {
    gender: string;
    specialization: string;
    qualification: string;
    yearsOfExperience: number;
    department: string;
    registrationNumber: string;
    dateOfBirth: string | null;
  };
  @Output() doctorDataChange = new EventEmitter<any>();
  @ViewChild('doctorForm', { static: true }) doctorForm: NgForm;

  genders = ['Male', 'Female', 'Other'];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.doctorData && changes.doctorData.currentValue) {
      // ensure the form model updates when parent data changes
      this.doctorData = { ...changes.doctorData.currentValue };
    }
  }

  onInputChange() {
    this.doctorDataChange.emit(this.doctorData);
  }
}
