import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, ControlValueAccessor, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';
import { InputNumberModule } from 'primeng/inputnumber';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
@Component({
  selector: 'app-edit-nurse',
   imports: [
    CommonModule,
    FormsModule,
    AbpValidationSummaryComponent,
    DatePickerModule,
    InputNumberModule,
    RadioButtonModule,
    SelectModule,
    CheckboxModule,
    InputTextModule
  ],
  providers: [DepartmentServiceProxy],
  templateUrl: './edit-nurse.component.html',
  styleUrl: './edit-nurse.component.css'
})
export class EditNurseComponent implements OnInit{
  @Output() nurseDataChange = new EventEmitter<any>();
  @ViewChild('nurseForm', { static: true }) nurseForm: NgForm;
  today: Date = new Date();
  genders = ['Male', 'Female', 'Other'];
  departments: { id: number; name: string }[] = [];
  qualifications: { label: string, value: string }[] = [
    { label: 'BSc Nursing', value: 'BSc Nursing' },
    { label: 'MSc Nursing', value: 'MSc Nursing' },
    { label: 'Diploma in Nursing', value: 'Diploma in Nursing' },
    { label: 'GNM', value: 'GNM' },
    { label: 'ANM', value: 'ANM' }
  ];

  @Input() nurseData: {
    gender: string;
    shiftTiming: string;
    qualification: string;
    yearsOfExperience: number;
    department: string;
    dateOfBirth: string | null;
  };

  constructor(
    private _departmentService: DepartmentServiceProxy,
    private cd: ChangeDetectorRef
  ) { }
   ngOnInit(): void {
    this.loadDepartments();
  }
  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.nurseDataChange.emit(this.nurseData);
  }
  loadDepartments(): void {
    this._departmentService.getAllDepartmentForDoctor().subscribe({
      next: (res: any) => {
        const items = res?.items ?? res;
        this.departments = items.map((d: any) => ({
          id: d.id ?? d.value ?? d.departmentId ?? d.key ?? 0,
          name: d.name ?? d.departmentName ?? d.label ?? d.text ?? ''
        }));
        this.cd.detectChanges();
      },
      error: () => {
        this.departments = [];
      }
    });
  }

}
