import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, ControlValueAccessor, NgForm } from '@angular/forms';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { DepartmentDto, DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-edit-lab-technician',
  imports: [CommonModule, FormsModule, AbpValidationSummaryComponent],
  providers:[DepartmentServiceProxy],
  templateUrl: './edit-lab-technician.component.html',
  styleUrl: './edit-lab-technician.component.css'
})
export class EditLabTechnicianComponent implements OnInit  {
  @Output() technicianDataChange = new EventEmitter<any>();
  @ViewChild('labTechnicianForm', { static: true }) labTechnicianForm: NgForm; 

  @Input() technicianData: {
      gender: string;
      qualification: string;
      yearsOfExperience: number;
       departmentId: number | null;
      certificationNumber: string;
      dateOfBirth: string | null;
    };

  genders = ['Male', 'Female', 'Other'];
 departments: DepartmentDto[] = [];
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
    this._departmentService.getAllDepartmentForLabTechnician().subscribe(res => {
      this.departments = res.items;

      if (this.technicianData.departmentId) {
        this.technicianData.departmentId = Number(this.technicianData.departmentId);
      }
      this.cd.detectChanges();
    });
  }
  
}