import { Component, EventEmitter, Injector, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { AppComponentBase } from '@shared/app-component-base';
import { LabReportsTypeServiceProxy, CreateUpdateLabReportTypeDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-add-data-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, InputTextModule, ButtonModule, DialogModule],
  providers:[LabReportsTypeServiceProxy],
  templateUrl: './add-data-dialog.component.html',
  styleUrls: ['./add-data-dialog.component.css']
})
export class AddDataDialogComponent extends AppComponentBase {
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  name: string = '';
  price:number=0;
  dataType: 'report' | 'diagnosis';

  constructor(
    injector: Injector,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private _labReportTypeService: LabReportsTypeServiceProxy,
    // private _diagnosisService: DiagnosisServiceProxy
  ) {
    super(injector);
    this.dataType = this.config.data.type;
  }

  save(): void {
    if (!this.name.trim()) return;
    
    this.saving = true;

    if (this.dataType === 'report') {
      this.saveReport();
    } else {
      //this.saveDiagnosis();
    }
  }

  private saveReport(): void {
    const input = new CreateUpdateLabReportTypeDto();
    input.reportType = this.name.trim();
    input.reportPrice=this.price
    input.tenantId = abp.session.tenantId;
debugger
    this._labReportTypeService.create(input).subscribe({
      next: () => this.handleSuccess(),
      error: () => this.handleError(),
      complete: () => this.saving = false
    });
  }

  // private saveDiagnosis(): void {
  //   const input = new CreateUpdateDiagnosisDto();
  //   input.name = this.name.trim();
  //   input.tenantId = abp.session.tenantId;
  //   // Add other required diagnosis properties here

  //   this._diagnosisService.create(input).subscribe({
  //     next: () => this.handleSuccess(),
  //     error: () => this.handleError(),
  //     complete: () => this.saving = false
  //   });
  // }

  private handleSuccess(): void {
    this.notify.success(this.l('SavedSuccessfully'));
    this.close(true);
  }

  private handleError(): void {
    this.notify.error(this.l('ErrorSavingData'));
  }

  close(shouldRefresh = false): void {
    if (shouldRefresh) {
      this.onSave.emit();
    }
    this.ref.close(shouldRefresh);
  }

  cancel(): void {
    this.ref.close();
  }

  isSaveDisabled(): boolean {
    return !this.name?.trim() || this.saving;
  }
}