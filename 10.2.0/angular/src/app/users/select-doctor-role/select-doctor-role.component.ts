import { ChangeDetectorRef, Component, EventEmitter, forwardRef, OnInit, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { fn } from 'moment';
import { CreateUpdateDoctorDto, DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
@Component({
  selector: 'app-select-doctor-role',
  imports: [FormsModule, CommonModule, AbpValidationSummaryComponent, CheckboxModule, InputTextModule, InputNumberModule],
  providers: [DepartmentServiceProxy],
  templateUrl: './select-doctor-role.component.html',
  styleUrl: './select-doctor-role.component.css'
})
export class SelectDoctorRoleComponent implements OnInit {
  @Output() doctorDataChange = new EventEmitter<any>();
  @ViewChild('doctorForm', { static: true }) doctorForm: NgForm;
  isEmergencyDoctor = false;

  doctorData = {
    gender: 'Male',
    specialization: '',
    qualification: '',
    yearsOfExperience: 0,
    departmentId: null,
    registrationNumber: '',
    dateOfBirth: null,
    id: 0,
    isEmergencyDoctor: false,
  };
  constructor(
    private _departmentService: DepartmentServiceProxy,
    private cd: ChangeDetectorRef
  ) { }
  genders = ['Male', 'Female', 'Other'];
  departments: { id: number; name: string }[] = [];
  maxDate: string;

  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.doctorDataChange.emit(this.doctorData);
  }
  ngOnInit(): void {
    this.loadDepartments();
    const today = new Date();
    this.maxDate = today.toISOString().split('T')[0];
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
  OnCheckIsEmergencyDoctor(event: any) {
    this.isEmergencyDoctor = event.checked;
  }
}
