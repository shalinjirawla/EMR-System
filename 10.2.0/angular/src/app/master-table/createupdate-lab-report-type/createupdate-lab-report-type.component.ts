import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CreateUpdateLabReportTypeDto, LabReportsTypeDto, LabReportsTypeServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-createupdate-lab-report-type',
  imports: [AbpModalHeaderComponent, AbpModalFooterComponent, FormsModule, CommonModule, InputTextModule, ButtonModule ],
  templateUrl: './createupdate-lab-report-type.component.html',
  styleUrl: './createupdate-lab-report-type.component.css',
  providers: [LabReportsTypeServiceProxy],
  standalone: true,
})
export class CreateupdateLabReportTypeComponent extends AppComponentBase implements OnInit {
  @ViewChild('labReportTypeForm', { static: true }) labReportTypeForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  id?: number;
  labReportType: Partial<CreateUpdateLabReportTypeDto> = {
    reportType: '',
    reportPrice: 0
  };

  get isFormValid(): boolean {
    return this.labReportTypeForm?.form?.valid;
  }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _labReportTypeService: LabReportsTypeServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.id) {
      this._labReportTypeService.get(this.id).subscribe(res => {
        this.labReportType = res;
        this.cd.detectChanges();
      });
    }
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please enter a lab report type name.');
      return;
    }
    this.saving = true;
      const input = new CreateUpdateLabReportTypeDto();
    input.id = this.id ?? 0;
    input.tenantId = this.appSession.tenantId;
    input.reportType = this.labReportType.reportType;
    input.reportPrice = this.labReportType.reportPrice;

    const request = this.id
      ? this._labReportTypeService.update(input)
      : this._labReportTypeService.create(input);

    request.subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}
