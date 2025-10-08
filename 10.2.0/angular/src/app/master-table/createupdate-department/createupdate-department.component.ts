import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  ViewChild,
  ChangeDetectorRef,
} from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { InputTextModule } from 'primeng/inputtext';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { InputSwitchModule } from 'primeng/inputswitch';
import {
  DepartmentServiceProxy,
  DepartmentDto,
  CreateUpdateDepartmentDto,
  DepartmentType,
} from '@shared/service-proxies/service-proxies';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-createupdate-department',
  standalone: true,
  imports: [CommonModule,FormsModule,InputTextModule,AutoCompleteModule,DropdownModule,InputSwitchModule,
    AbpModalHeaderComponent,AbpModalFooterComponent,],
  providers: [DepartmentServiceProxy],
  templateUrl: './createupdate-department.component.html',
  styleUrl: './createupdate-department.component.css',
})
export class CreateupdateDepartmentComponent extends AppComponentBase implements OnInit
{
  @ViewChild('departmentForm', { static: true }) departmentForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  // Update mode
  department: CreateUpdateDepartmentDto = new CreateUpdateDepartmentDto();
  departmentNames: string[] = [];
  selectedDepartmentNames: string[] = [];

  departmentTypes = [
    { label: 'Doctor', value: DepartmentType._0 },
    { label: 'Lab Technician', value: DepartmentType._1},
  ];

  isActive: boolean = true;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _departmentService: DepartmentServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isFormValid(): boolean {
    if (this.id) {
      return this.departmentForm?.form.valid && !!this.department.departmentName;
    } else {
      return (
        this.departmentForm?.form.valid &&
        this.selectedDepartmentNames.length > 0
      );
    }
  }

  ngOnInit(): void {
    if (this.id) {
      this._departmentService.get(this.id).subscribe((res) => {
        this.department = res;
        this.isActive = res.isActive!;
        this.cd.detectChanges();
      });
    }
  }

 

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please fill in required fields.');
      return;
    }

    this.saving = true;

    if (this.id) {
      // UPDATE
      const input = new CreateUpdateDepartmentDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.departmentName = this.department.departmentName!;
      input.isActive = this.isActive;
      input.departmentType = this.department.departmentType!;
      this._departmentService.update(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
      });
    } else {
      // BULK CREATE
      const inputs = this.selectedDepartmentNames.map((name) => {
        const dto = new CreateUpdateDepartmentDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.departmentName = name;
        dto.isActive = this.isActive;
        dto.departmentType = this.department.departmentType!;
        return dto;
      });
      this._departmentService.createBulk(inputs).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
        complete: () => (this.saving = false),
      });
    }
  }
}
