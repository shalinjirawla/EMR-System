import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormsModule, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-select-labtechnician-role',
  imports: [CommonModule, FormsModule, AbpValidationSummaryComponent],
  providers:[DepartmentServiceProxy],
  templateUrl: './select-labtechnician-role.component.html',
  styleUrl: './select-labtechnician-role.component.css'
})
export class SelectLabtechnicianRoleComponent implements OnInit{
  @Output() technicianDataChange = new EventEmitter<any>();
  @ViewChild('labTechnicianForm', { static: true }) labTechnicianForm: NgForm;

  technicianData = {
    gender: 'Male',
    qualification: '',
    yearsOfExperience: 0,
    departmentId: null,
    certificationNumber: '',
    dateOfBirth: null
  };
  constructor(
    private _departmentService: DepartmentServiceProxy,
    private cd: ChangeDetectorRef
  ) {}

  genders = ['Male', 'Female', 'Other'];
  departments: { id: number; name: string }[] = [];
  maxDate: string;

  onInputChange() {
    this.updateData();
  }
ngOnInit(): void {
    this.loadDepartments();
    const today = new Date();
    this.maxDate = today.toISOString().split('T')[0];
  }
  updateData() {
    this.technicianDataChange.emit(this.technicianData);
  }
 loadDepartments(): void {
    this._departmentService.getAllDepartmentForLabTechnician().subscribe({
      next: (res: any) => {
        const items = (res && res.items) ? res.items : res;
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
