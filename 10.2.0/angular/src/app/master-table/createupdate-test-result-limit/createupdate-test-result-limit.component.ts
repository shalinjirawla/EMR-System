import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateUpdateTestResultLimitDto, LabTestDto, LabTestServiceProxy, TestResultLimitServiceProxy } from '@shared/service-proxies/service-proxies';
import { FormsModule, NgForm } from '@angular/forms';
import { MultiSelectModule } from 'primeng/multiselect';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@angular/common';
import { AbpModalFooterComponent } from "@shared/components/modal/abp-modal-footer.component";
import { AbpModalHeaderComponent } from "@shared/components/modal/abp-modal-header.component";

@Component({
  selector: 'app-createupdate-test-result-limit',
  imports: [FormsModule, MultiSelectModule, CommonModule, AbpModalFooterComponent, AbpModalHeaderComponent, LocalizePipe],
  providers:[LabTestServiceProxy,TestResultLimitServiceProxy],
  templateUrl: './createupdate-test-result-limit.component.html',
  styleUrl: './createupdate-test-result-limit.component.css'
})
export class CreateupdateTestResultLimitComponent extends AppComponentBase implements OnInit {
  @Input() id?: number;
  @ViewChild('labForm', { static: true }) labForm: NgForm;
  @Output() onSave = new EventEmitter<void>();  

  saving = false;

  labTests: LabTestDto[] = [];
  selectedLabTests: LabTestDto[] = [];
  testLimits: any[] = [];

  constructor(
    injector: Injector,
    public modalRef: BsModalRef,
    private labTestService: LabTestServiceProxy,
    private testResultLimitService: TestResultLimitServiceProxy,
    private cdr: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.id) {
      this.loadForEdit(this.id);
    } else {
      this.loadLabTests();
    }
  }

  loadLabTests(): void {
    this.labTestService.getAllLabTestByTenantId(abp.session.tenantId).subscribe((result) => {
      this.labTests = result.items;
    });
  }

  loadForEdit(id: number): void {
    this.testResultLimitService.get(id).subscribe((dto) => {
      this.selectedLabTests = [{
        id: dto.labTestId,
        name: dto.labTestName
      } as LabTestDto];

      this.testLimits = [{
        labTestId: dto.labTestId,
        labTestName: dto.labTestName,
        minRange: dto.minRange,
        maxRange: dto.maxRange
      }];

      this.cdr.detectChanges();
    });
  }

  onTestChange(): void {
    const addedTests = this.selectedLabTests.filter(
      test => !this.testLimits.some(l => l.labTestId === test.id)
    );

    for (let test of addedTests) {
      this.testLimits.push({
        labTestId: test.id,
        labTestName: test.name,
        minRange: null,
        maxRange: null
      });
    }

    this.testLimits = this.testLimits.filter((item) =>
      this.selectedLabTests.some((t) => t.id === item.labTestId)
    );
  }

  removeTest(index: number): void {
    const removedId = this.testLimits[index].labTestId;
    this.testLimits.splice(index, 1);
    this.selectedLabTests = this.selectedLabTests.filter(t => t.id !== removedId);
  }

  save(): void {
    if (!this.labForm.valid || !this.testLimits.length) {
      this.notify.warn('Please fill all required fields and select at least one lab test');
      return;
    }

    this.saving = true;
    if (this.id) {
      // 🔁 Edit mode: single record update
      const item = this.testLimits[0];
      const dto = new CreateUpdateTestResultLimitDto();
      dto.id = this.id;
      dto.tenantId = this.appSession.tenantId;
      dto.labTestId = item.labTestId;
      dto.minRange = item.minRange;
      dto.maxRange = item.maxRange;

      this.testResultLimitService.update(dto).subscribe({
        next: () => {
          this.notify.success(this.l('UpdatedSuccessfully'));
          this.modalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.notify.error(this.l('UpdateFailed'));
          this.saving = false;
        },
        complete: () => (this.saving = false),
      });
    } else {
      // ➕ Create mode: bulk insert
      const input: CreateUpdateTestResultLimitDto[] = this.testLimits.map((item) => {
        const dto = new CreateUpdateTestResultLimitDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.labTestId = item.labTestId;
        dto.minRange = item.minRange;
        dto.maxRange = item.maxRange;
        return dto;
      });

      this.testResultLimitService.createBulk(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.modalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.notify.error(this.l('SaveFailed'));
          this.saving = false;
        },
        complete: () => (this.saving = false),
      });
    }
  }
}