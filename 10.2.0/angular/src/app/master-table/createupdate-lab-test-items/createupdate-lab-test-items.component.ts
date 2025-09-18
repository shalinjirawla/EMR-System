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
import {
  CreateUpdateLabReportTypeItemDto,
  LabReportTypeItemServiceProxy,
  LabReportsTypeServiceProxy,
  LabTestServiceProxy,
  LabReportsTypeDto,
  LabTestDto,
} from '@shared/service-proxies/service-proxies';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { CheckboxModule } from 'primeng/checkbox';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';

@Component({
  selector: 'app-createupdate-lab-test-items',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DropdownModule,
    MultiSelectModule,
    CheckboxModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
  ],
  providers: [LabReportTypeItemServiceProxy, LabReportsTypeServiceProxy, LabTestServiceProxy],
  templateUrl: './createupdate-lab-test-items.component.html',
  styleUrl: './createupdate-lab-test-items.component.css'
})
export class CreateupdateLabTestItemsComponent extends AppComponentBase implements OnInit {
  @ViewChild('labForm', { static: true }) labForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  labReportTypes: LabReportsTypeDto[] = [];
  labTests: LabTestDto[] = [];

  selectedReportTypeId?: number;
  selectedLabTestId?: number; // for update
  selectedLabTestIds: number[] = []; // for bulk create

  isActive = true;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _labReportTypeItemService: LabReportTypeItemServiceProxy, 
    private _labReportTypeService: LabReportsTypeServiceProxy,
    private _labTestService: LabTestServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isFormValid(): boolean {
    return !!(this.labForm?.form.valid && this.selectedReportTypeId != null && (this.id ? this.selectedLabTestId != null : this.selectedLabTestIds.length > 0));
  }

  ngOnInit(): void {
    this._labReportTypeService.getAll(undefined, undefined, undefined).subscribe((res) => {
      this.labReportTypes = res.items;
    });

    this._labTestService.getAllLabTestByTenantId(this.appSession.tenantId).subscribe((res) => {
      this.labTests = res.items;
    });

    if (this.id) {
      this._labReportTypeItemService.get(this.id).subscribe((res) => {
        this.selectedReportTypeId = res.labReportTypeId;
        this.selectedLabTestId = res.labTestId;
        this.isActive = res.isActive;
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
      const input = new CreateUpdateLabReportTypeItemDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.labReportTypeId = this.selectedReportTypeId!;
      input.labTestId = this.selectedLabTestId!;
      input.isActive = this.isActive;

      this._labReportTypeItemService.update(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
      });
    } else {
      const inputs = this.selectedLabTestIds.map((labTestId) => {
        const dto = new CreateUpdateLabReportTypeItemDto();
        dto.tenantId = this.appSession.tenantId;
        dto.labReportTypeId = this.selectedReportTypeId!;
        dto.labTestId = labTestId;
        dto.isActive = this.isActive;
        return dto;
      });
      this._labReportTypeItemService.createBulk(inputs).subscribe({
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