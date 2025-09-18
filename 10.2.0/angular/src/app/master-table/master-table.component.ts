import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { AddDataDialogComponent } from './add-data-dialog/add-data-dialog.component';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { DepartmentServiceProxy, LabReportsTypeDto, LabReportsTypeDtoListResultDto, LabReportsTypeServiceProxy, RoomFacilityMasterServiceProxy, RoomTypeMasterServiceProxy } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'app-master-table',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  providers: [DialogService, LabReportsTypeServiceProxy,DepartmentServiceProxy,RoomFacilityMasterServiceProxy,RoomTypeMasterServiceProxy],
  templateUrl: './master-table.component.html',
  styleUrls: ['./master-table.component.css']
})
export class MasterTableComponent implements OnInit {
  medicalReportCount!: number;
  totalDepartmentCount!: number;
  labReportsTypeList!: LabReportsTypeDto[];
  //ldepartmentList!: DepartmentListDto[];
   totalRoomFacilityCount!: number;
  totalRoomTypeCount!: number;
  constructor(private dialogService: DialogService,
    private _labReportTypeService: LabReportsTypeServiceProxy,
    private _departmentService: DepartmentServiceProxy,
    private _facilitySvc: RoomFacilityMasterServiceProxy,
    private _roomTypeSvc: RoomTypeMasterServiceProxy,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.GetMedicalReports();
    //this.GetDepartmentReports();
    this.GetRoomFacilityCount();
    this.GetRoomTypeCount();
  }
  createReport(): void {
    this.showCreateDialog('report');
  }
  createDiagnosis(): void {
    this.showCreateDialog('diagnosis');
  }
  createRoomFacility(): void {
    this.showCreateDialog('roomfacility');
  }

  createRoomType(): void {
    this.showCreateDialog('roomtype');
  }
  createDepartment(): void {
    this.showCreateDialog('department');
  }
  private showCreateDialog(dataType: 'report' | 'diagnosis' | 'department'|'roomtype'|'roomfacility'): void {
    const dialogRef = this.dialogService.open(AddDataDialogComponent, {
      header: `Add New ${this.capitalizeFirstLetter(dataType)}`,
      width: '450px',
      data: { type: dataType },
      dismissableMask: true,
      styleClass: 'custom-dialog'
    });

    dialogRef.onClose.subscribe((shouldRefresh: boolean) => {
      if (shouldRefresh) {
        this.refresh();
      }
    });
  }
  private refresh(): void {
    // Implement your refresh logic here
  }
  private capitalizeFirstLetter(string: string): string {
    return string.charAt(0).toUpperCase() + string.slice(1);
  }
  GetMedicalReports() {
    this._labReportTypeService.getAllTestByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.medicalReportCount = res.items.length;
        this.labReportsTypeList = res.items;
        this.cd.detectChanges();
      },
      error: (err) => {
      }
    });
  }
  // GetDepartmentReports() {
  //   this._departmentService.getAllDepartmentByTenantID().subscribe({
  //     next: (res) => {
  //       this.totalDepartmentCount=res.items.length;
  //       this.cd.detectChanges();
  //     },
  //     error: (err) => {
  //     }
  //   });
  // }
   GetRoomFacilityCount() {
    this._facilitySvc.getAllRoomFacilityByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.totalRoomFacilityCount = res.items.length;
        this.cd.detectChanges();
      },
      error: () => console.warn('Could not load facilities')
    });
  }

  GetRoomTypeCount() {
    this._roomTypeSvc.getAllRoomTypeByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.totalRoomTypeCount = res.items.length;
        this.cd.detectChanges();
      },
      error: () => console.warn('Could not load room types')
    });
  }
}