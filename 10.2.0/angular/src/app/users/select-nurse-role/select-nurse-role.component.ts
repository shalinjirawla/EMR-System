import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';

import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectModule } from 'primeng/select';
import { DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-select-nurse-role',
  imports: [FormsModule, CommonModule, AbpValidationSummaryComponent, SelectModule,
    DatePickerModule, InputTextModule, InputNumberModule, CheckboxModule, RadioButtonModule
  ],
  templateUrl: './select-nurse-role.component.html',
  styleUrl: './select-nurse-role.component.css',
  providers: [DepartmentServiceProxy]
})
export class SelectNurseRoleComponent implements OnInit {
  @Output() nurseDataChange = new EventEmitter<any>();
  @ViewChild('nurseForm', { static: true }) nurseForm: NgForm;
  today: Date = new Date();
  qualifications: { label: string, value: string }[] = [
    { label: 'BSc Nursing', value: 'BSc Nursing' },
    { label: 'MSc Nursing', value: 'MSc Nursing' },
    { label: 'Diploma in Nursing', value: 'Diploma in Nursing' },
    { label: 'GNM', value: 'GNM' },
    { label: 'ANM', value: 'ANM' }
  ];
  departments: { id: number; name: string }[] = [];
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
    // Adjust method name if your generated proxy has a different name (e.g., getAllForDropdownAsync)
    this._departmentService.getAllDepartmentForDoctor().subscribe({
      next: (res: any) => {
        // If proxy returns ListResultDto<DepartmentDto> with "items"
        const items = (res && res.items) ? res.items : res;
        this.departments = items.map((d: any) => ({
          id: d.id ?? d.value ?? d.departmentId ?? d.key ?? 0,
          name: d.name ?? d.departmentName ?? d.label ?? d.text ?? ''
        }));
        this.cd.detectChanges();
      },
      error: () => {
        // optional: handle error (notify/log)
        this.departments = [];
      }
    });
  }
}
