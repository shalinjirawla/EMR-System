import { Component, EventEmitter, Injector, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { AppComponentBase } from '@shared/app-component-base';
import { LabReportsTypeServiceProxy, CreateUpdateLabReportTypeDto, CreateUpdateDepartmentDto, DepartmentServiceProxy, RoomFacilityMasterServiceProxy, RoomTypeMasterServiceProxy, RoomFacilityMasterDto } from '@shared/service-proxies/service-proxies';
import { MultiSelectModule } from 'primeng/multiselect';

@Component({
  selector: 'app-add-data-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, InputTextModule, ButtonModule, DialogModule,MultiSelectModule ],
  providers: [LabReportsTypeServiceProxy, RoomFacilityMasterServiceProxy, RoomTypeMasterServiceProxy],
  templateUrl: './add-data-dialog.component.html',
  styleUrls: ['./add-data-dialog.component.css']
})
export class AddDataDialogComponent extends AppComponentBase {
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  name: string = '';
  price: number = 0;
  description:string=''
  dataType: 'report' | 'diagnosis' | 'department' | 'roomfacility' | 'roomtype';
allFacilities: RoomFacilityMasterDto[] = [];
  selectedFacilityIds: number[] = [];

  constructor(
    injector: Injector,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private _labReportTypeService: LabReportsTypeServiceProxy,
    private _departmentService: DepartmentServiceProxy,
    private _facilitySvc: RoomFacilityMasterServiceProxy,
    private _roomTypeSvc: RoomTypeMasterServiceProxy
    // private _diagnosisService: DiagnosisServiceProxy
  ) {
    super(injector);
    this.dataType = this.config.data.type;
    if (this.dataType === 'roomtype') {
      this.loadFacilities();
    }
  }

  save(): void {
    if (!this.name.trim()) return;
    this.saving = true;

    switch (this.dataType) {
      case 'report': return this.saveReport();
      case 'department': return this.saveDepartment();
      case 'roomfacility': return this.saveRoomFacility();
      case 'roomtype': return this.saveRoomType();
      default:             /* diagnosis or others */ return;
    }
  }
  private loadFacilities() {
    this._facilitySvc.getAllRoomFacilityByTenantID(abp.session.tenantId).subscribe({
      next: res => (this.allFacilities = res.items),
      error: () => this.notify.warn('Could not load facilities')
    });
  }

  getLabel(): string {
    switch (this.dataType) {
      case 'report': return 'Report';
      case 'diagnosis': return 'Diagnosis';
      case 'department': return 'Department';
      case 'roomfacility': return 'Facility';
      case 'roomtype': return 'Room Type';
      default: return '';
    }
  }
  private saveReport(): void {
    const input = new CreateUpdateLabReportTypeDto();
    input.reportType = this.name.trim();
    input.reportPrice = this.price
    input.tenantId = abp.session.tenantId;

    this._labReportTypeService.create(input).subscribe({
      next: () => this.handleSuccess(),
      error: () => this.handleError(),
      complete: () => this.saving = false
    });
  }
  private saveDepartment(): void {
    const input = new CreateUpdateDepartmentDto();
    input.name = this.name.trim();
    input.tenantId = abp.session.tenantId;

    this._departmentService.create(input).subscribe({
      next: () => this.handleSuccess(),
      error: () => this.handleError(),
      complete: () => this.saving = false
    });
  }

  private saveRoomFacility(): void {
    const dto = { facilityName: this.name.trim(), tenantId: abp.session.tenantId } as any;
    this._facilitySvc.create(dto).subscribe({
      next: () => this.handleSuccess(),
      error: () => this.handleError(),
      complete: () => (this.saving = false)
    });
  }

  private saveRoomType(): void {
    const dto = {
      typeName: this.name.trim(),
      description: this.description,
      defaultPricePerDay: this.price,
      facilityIds: this.selectedFacilityIds,
      tenantId: abp.session.tenantId
    } as any;
    this._roomTypeSvc.createWithFacilities(dto).subscribe({
      next: () => this.handleSuccess(),
      error: () => this.handleError(),
      complete: () => (this.saving = false)
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