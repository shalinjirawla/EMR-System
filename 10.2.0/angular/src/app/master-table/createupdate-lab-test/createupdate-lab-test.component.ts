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
import { CheckboxModule } from 'primeng/checkbox';
import { AutoCompleteModule } from 'primeng/autocomplete';
import {
  LabTestServiceProxy,
  LabTestDto,
  CreateUpdateLabTestDto,
  MeasureUnitServiceProxy,
  MeasureUnitDto,
} from '@shared/service-proxies/service-proxies';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-createupdate-lab-test',
  standalone: true,
  templateUrl: './createupdate-lab-test.component.html',
  styleUrl: './createupdate-lab-test.component.css',
  imports: [CommonModule,FormsModule,InputTextModule,CheckboxModule,AutoCompleteModule,
    AbpModalHeaderComponent,AbpModalFooterComponent,DropdownModule],
  providers: [LabTestServiceProxy,MeasureUnitServiceProxy],
})
export class CreateupdateLabTestComponent extends AppComponentBase implements OnInit {
  @ViewChild('labTestForm', { static: true }) labTestForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;
  measureUnits: MeasureUnitDto[] = [];
selectedUnitId?: number;

  // Update mode
  labTest: Partial<LabTestDto> = { name: '', isActive: true };

  // Bulk create mode
  selectedTestNames: string[] = [];
  filteredTestNames: string[] = [];
  testNameMasterList: string[] = [];

  isActive = true;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _labTestService: LabTestServiceProxy,
    private _measureUnitService: MeasureUnitServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._measureUnitService.getAllMeasureUnitsByTenantId(this.appSession.tenantId).subscribe((res) => {
      this.measureUnits = res.items;
    });
  
    if (this.id) {
      this._labTestService.get(this.id).subscribe((res) => {
        this.labTest = res;
        this.selectedUnitId = res.measureUnitId!; 
        this.isActive = res.isActive!;
        this.cd.detectChanges();
      });
    }
  }

  get isFormValid(): boolean {
    if (this.id) {
      return this.labTestForm?.form.valid && !!this.labTest.name;
    } else {
      return this.labTestForm?.form.valid && this.selectedTestNames.length > 0;
    }
  }

  filterTestNames(event: { query: string }): void {
    const q = event.query.toLowerCase();
    this.filteredTestNames = this.testNameMasterList.filter((name) =>
      name.toLowerCase().includes(q)
    );
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please fill in required fields.');
      return;
    }

    this.saving = true;

    if (this.id) {
      const input = new CreateUpdateLabTestDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.name = this.labTest.name!;
      input.isActive = this.isActive;
      input.measureUnitId = this.selectedUnitId!;

      this._labTestService.update(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
      });
    } else {
      const inputs = this.selectedTestNames.map((name) => {
        const dto = new CreateUpdateLabTestDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.name = name;
        dto.isActive = this.isActive;
        dto.measureUnitId = this.selectedUnitId!;
        return dto;
      });
      this._labTestService.createBulk(inputs).subscribe({
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
