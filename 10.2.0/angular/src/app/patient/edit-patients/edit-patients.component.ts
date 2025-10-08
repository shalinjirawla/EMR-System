import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';

@Component({
  selector: 'app-edit-patients',
  imports: [AbpValidationSummaryComponent, FormsModule, RadioButtonModule, InputTextModule,TextareaModule,
    DatePickerModule, InputNumberModule, CheckboxModule, SelectModule, CommonModule],
  providers: [],
  templateUrl: './edit-patients.component.html',
  styleUrl: './edit-patients.component.css'
})
export class EditPatientsComponent implements OnInit {

  @Output() patientDataChange = new EventEmitter<any>();
  @ViewChild('patientForm', { static: true }) patientForm: NgForm;
  today: Date = new Date();

  bloodGroups = [
    { label: 'A+', value: 'A+' },
    { label: 'A−', value: 'A−' },
    { label: 'B−', value: 'B−' },
    { label: 'B+', value: 'B+' },
    { label: 'AB+', value: 'AB+' },
    { label: 'AB-', value: 'AB-' },
    { label: 'O+', value: 'O+' },
    { label: 'O-', value: 'O-' },
  ];
  genders = ['Male', 'Female', 'Other'];

  @Input() patientData: {
    gender: string;
    dateOfBirth: string | null;
    address: string,
    bloodGroup: string,
    emergencyContactName: string,
    emergencyContactNumber: string,
  };
  constructor(
    private cdr: ChangeDetectorRef) {

  }

  ngOnInit() {
  }

  onInputChange() {
    this.updateData();
  }
  onAdmitChange() {

    this.onInputChange();
  }
  updateData() {
    this.patientDataChange.emit(this.patientData);
  }
}
