import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { AddDataDialogComponent } from './add-data-dialog/add-data-dialog.component';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { DepartmentListDto, DepartmentServiceProxy, LabReportsTypeDto, LabReportsTypeDtoListResultDto, LabReportsTypeServiceProxy } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'app-master-table',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  providers: [DialogService, LabReportsTypeServiceProxy,DepartmentServiceProxy],
  templateUrl: './master-table.component.html',
  styleUrls: ['./master-table.component.css']
})
export class MasterTableComponent implements OnInit {
  medicalReportCount!: number;
  totalDepartmentCount!: number;
  labReportsTypeList!: LabReportsTypeDto[];
  ldepartmentList!: DepartmentListDto[];
  constructor(private dialogService: DialogService,
    private _labReportTypeService: LabReportsTypeServiceProxy,
    private _departmentService: DepartmentServiceProxy,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.GetMedicalReports();
    this.GetDepartmentReports();
  }
  createReport(): void {
    this.showCreateDialog('report');
  }
  createDiagnosis(): void {
    this.showCreateDialog('diagnosis');
  }
  createDepartment(): void {
    this.showCreateDialog('department');
  }
  private showCreateDialog(dataType: 'report' | 'diagnosis' | 'department'): void {
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
    console.log('Refreshing data...');
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
        console.log('Could not load lab tests');
      }
    });
  }
  GetDepartmentReports() {
    this._departmentService.getAllDepartmentByTenantID().subscribe({
      next: (res) => {
        this.totalDepartmentCount=res.items.length;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.log('Could not load lab tests');
      }
    });
  }
}