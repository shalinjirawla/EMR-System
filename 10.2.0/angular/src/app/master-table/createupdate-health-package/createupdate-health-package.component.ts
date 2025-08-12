import { Component, Injector, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MultiSelectModule } from 'primeng/multiselect';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ChangeDetectorRef } from '@angular/core';
import { AppComponentBase } from '../../../shared/app-component-base';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { CreateUpdateHealthPackageDto, CreateUpdateHealthPackageLabReportsTypeDto, HealthPackageDto, HealthPackageServiceProxy, LabReportsTypeDto, LabReportsTypeServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-createupdate-health-package',
  standalone: true,
  imports: [AbpModalHeaderComponent, AbpModalFooterComponent, FormsModule, CommonModule, InputTextModule, ButtonModule, MultiSelectModule],
  providers: [HealthPackageServiceProxy, LabReportsTypeServiceProxy],
  templateUrl: './createupdate-health-package.component.html',
  styleUrl: './createupdate-health-package.component.css'
})
export class CreateupdateHealthPackageComponent extends AppComponentBase implements OnInit {
  @ViewChild('healthPackageForm', { static: true }) healthPackageForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  // form model
  healthPackage: Partial<CreateUpdateHealthPackageDto> = {
    packageName: '',
    description: '',
    packagePrice: 0,
    isActive: true,
    labReportsTypes: []
  };

  // dropdown options
  labReportsTypesOptions: LabReportsTypeDto[] = [];
  // selected ids (multi select uses ids)
  selectedLabReportTypeIds: number[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _healthPackageService: HealthPackageServiceProxy,
    private _labReportsTypeService: LabReportsTypeServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isFormValid(): boolean {
    return this.healthPackageForm?.form?.valid;
  }

  ngOnInit(): void {
    // load lab report types for multi-select
    this._labReportsTypeService.getAllTestByTenantID(abp.session.tenantId).subscribe(res => {
      this.labReportsTypesOptions = res.items || [];
      this.cd.detectChanges();
    });

    if (this.id) {
      this._healthPackageService.get(this.id).subscribe((res: HealthPackageDto) => {
        // map api dto to form model
        this.healthPackage = {
          packageName: res.packageName,
          description: res.description,
          packagePrice: res.packagePrice,
          isActive: res.isActive
        } as CreateUpdateHealthPackageDto;

        this.selectedLabReportTypeIds = res.labReportsTypeIds || [];

        this.cd.detectChanges();
      });
    }

  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please fill required fields.');
      return;
    }

    this.saving = true;

    const input = new CreateUpdateHealthPackageDto();
    // include id for update
    if (this.id) {
      (input as any).id = this.id;
    }
    input.tenantId = this.appSession.tenantId;
    input.packageName = this.healthPackage.packageName!;
    input.description = this.healthPackage.description;
    input.packagePrice = this.healthPackage.packagePrice ?? 0;
    input.isActive = this.healthPackage.isActive ?? true;

    // map selected ids to CreateUpdateHealthPackageLabReportsTypeDto[]
    input.labReportsTypes = (this.selectedLabReportTypeIds || []).map(id => {
      const item = new CreateUpdateHealthPackageLabReportsTypeDto();
      (item as any).labReportsTypeId = id;
      (item as any).tenantId = this.appSession.tenantId;
      return item;
    });

    const request = this.id ? this._healthPackageService.update(input) : this._healthPackageService.create(input);

    request.subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.onSave.emit();
        this.bsModalRef.hide();
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}