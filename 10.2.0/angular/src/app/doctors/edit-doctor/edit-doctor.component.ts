import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';
import { DepartmentDto, DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
@Component({
  selector: 'app-edit-doctor',
  imports: [FormsModule, RadioButtonModule, DatePickerModule, SelectModule, CommonModule, AbpValidationSummaryComponent, InputTextModule, InputNumberModule, CheckboxModule],
  providers: [DepartmentServiceProxy],
  templateUrl: './edit-doctor.component.html',
  styleUrl: './edit-doctor.component.css'
})
export class EditDoctorComponent implements OnInit, OnChanges {
  isEmergencyDoctor = false;
  @Input() doctorData: {
    gender: string;
    specialization: string;
    qualification: string;
    yearsOfExperience: number;
    departmentId: number | null;
    registrationNumber: string;
    dateOfBirth: string | null;
    isEmergencyDoctor: boolean | false;
  };
  @Output() doctorDataChange = new EventEmitter<any>();
  @ViewChild('doctorForm', { static: true }) doctorForm: NgForm;

  genders = ['Male', 'Female', 'Other'];
  departments: DepartmentDto[] = [];
  maxDate: string;
  today: Date = new Date();
  constructor(private _departmentService: DepartmentServiceProxy,
    private cd: ChangeDetectorRef

  ) { }

  ngOnInit(): void {
    this.loadDepartments();

    // today date in yyyy-MM-dd format
    const today = new Date();
    this.maxDate = today.toISOString().split('T')[0];
    this.isEmergencyDoctor = this.doctorData.isEmergencyDoctor;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.doctorData && changes.doctorData.currentValue) {
      // ensure the form model updates when parent data changes
      this.doctorData = { ...changes.doctorData.currentValue };
    }
  }
  loadDepartments(): void {
    this._departmentService.getAllDepartmentForDoctor().subscribe(res => {
      this.departments = res.items;

      if (this.doctorData.departmentId) {
        this.doctorData.departmentId = Number(this.doctorData.departmentId);
      }
      this.cd.detectChanges();
    });
  }

  onInputChange() {
    if (this.doctorData.isEmergencyDoctor) {
      this.doctorData.departmentId = null
    }
    this.doctorDataChange.emit(this.doctorData);
  }
  OnCheckIsEmergencyDoctor(event: any) {
    this.isEmergencyDoctor = event.checked;
  }
}
