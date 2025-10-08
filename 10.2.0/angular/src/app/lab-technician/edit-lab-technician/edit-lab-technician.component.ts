import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, ControlValueAccessor, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { DepartmentDto, DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
@Component({
  selector: 'app-edit-lab-technician',
  imports: [CommonModule,CheckboxModule,InputTextModule,InputNumberModule,SelectModule,DatePickerModule,
     RadioButtonModule,FormsModule, AbpValidationSummaryComponent],
  providers:[DepartmentServiceProxy],
  templateUrl: './edit-lab-technician.component.html',
  styleUrl: './edit-lab-technician.component.css'
})
export class EditLabTechnicianComponent implements OnInit  {
  @Output() technicianDataChange = new EventEmitter<any>();
  @ViewChild('labTechnicianForm', { static: true }) labTechnicianForm: NgForm; 
  today: Date = new Date();
  @Input() technicianData: {
      gender: string;
      qualification: string;
      yearsOfExperience: number;
       departmentId: number | null;
      certificationNumber: string;
      dateOfBirth: string | null;
    };

  genders = ['Male', 'Female', 'Other'];
 departments: { id: number; name: string }[] = [];
  maxDate: string;
  constructor(
    private _departmentService: DepartmentServiceProxy,
    private cd: ChangeDetectorRef
  ) {}
  
  onInputChange() {
    this.updateData();
  }

  updateData() {
    this.technicianDataChange.emit(this.technicianData);
  }
 ngOnInit(): void {
    this.loadDepartments();
    const today = new Date();
    this.maxDate = today.toISOString().split('T')[0];
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