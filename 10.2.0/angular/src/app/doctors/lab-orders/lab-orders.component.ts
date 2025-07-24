import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { CommonModule, NgIf } from '@node_modules/@angular/common';
import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { LabRequestListDto, LabTestStatus, PrescriptionLabTestDto, PrescriptionLabTestsServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ChipModule } from 'primeng/chip';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { EditLabReportComponent } from '@app/lab-technician/edit-lab-report/edit-lab-report.component';
import { GenerateLabReportComponent } from '@app/lab-technician/generate-lab-report/generate-lab-report.component';
import { ViewLabReportComponent } from '@app/lab-technician/view-lab-report/view-lab-report.component';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-lab-orders',
  templateUrl: './lab-orders.component.html',
  styleUrl: './lab-orders.component.css',
  animations: [appModuleAnimation()],
  imports: [FormsModule, TableModule,TagModule, CommonModule, PrimeTemplate, OverlayPanelModule, MenuModule, ButtonModule, NgIf, PaginatorModule, ChipModule, LocalizePipe],
  providers: [PrescriptionLabTestsServiceProxy]
})
export class LabOrdersComponent extends PagedListingComponentBase<PrescriptionLabTestDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;
  testStatus = [
    { label: 'Pending', value: LabTestStatus._0 },
    { label: 'In Progress', value: LabTestStatus._1 },
    { label: 'Completed', value: LabTestStatus._2 },
  ];
  showDoctorColumn: boolean = false;
  showNurseColumn: boolean = false;
  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _prescriptionLabTests: PrescriptionLabTestsServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }
  ngOnInit(): void {
    this.GetLoggedInUserRole();
  }
  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
  }
  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }
    this.primengTableHelper.showLoadingIndicator();

    this._prescriptionLabTests
      .getAllLabOrders(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => {
        this.primengTableHelper.hideLoadingIndicator();
      }))
      .subscribe((result) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }
  delete(entity: PrescriptionLabTestDto): void {
    abp.message.confirm("Are you sure u want to delete this", undefined, (result: boolean) => {
      if (result) {
        this._prescriptionLabTests.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
  getStatusLabel(value: number): string {
    const status = this.testStatus.find(s => s.value === value);
    return status ? status.label : '';
  }
  getStatusClass(value: number): string {
    switch (value) {
      case LabTestStatus._0: return 'status-pending';    // Scheduled
      case LabTestStatus._1: return 'status-in-progress';    // Checked In
      case LabTestStatus._2: return 'status-completed';    // Completed
      default: return '';
    }
  }
  getStatusSeverity(value: number): 'info' | 'warn' | 'success' | 'danger' | 'secondary' | 'contrast' {
    switch (value) {
      case LabTestStatus._0: return 'info';        // Pending
      case LabTestStatus._1: return 'secondary';   // In Progress
      case LabTestStatus._2: return 'success';     // Completed
      default: return 'contrast';
    }
  }
  CreateReport(record: LabRequestListDto): void {
    let createReportDialog: BsModalRef;
    if (record.id) {
      createReportDialog = this._modalService.show(GenerateLabReportComponent, {
        class: 'modal-xl',
        initialState: {
          id: record.id,
          testName: record.labReportTypeName,
          patientName: record.patientName
        },
      });
    }
    createReportDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
  EditReport(record: LabRequestListDto): void {
    let editReportDialog: BsModalRef;
    if (record.id) {
      editReportDialog = this._modalService.show(EditLabReportComponent, {
        class: 'modal-xl',
        initialState: {
          id: record.id,
          testName: record.labReportTypeName,
          patientName: record.patientName
        },
      });
    }
    editReportDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
  ViewLabReport(id?: number) {
    let viewReportDialog: BsModalRef;
    viewReportDialog = this._modalService.show(ViewLabReportComponent, {
      class: 'modal-xl',
      initialState: {
        id: id,
      },
    });
    // editReportDialog.content.onSave.subscribe(() => {
    //     this.refresh();
    // });
  }
  GetLoggedInUserRole() {
    this._prescriptionLabTests.getCurrentUserRoles().subscribe(res => {
      this.showDoctorColumn = false;
      this.showNurseColumn = false;
      debugger
      if (res && Array.isArray(res)) {
        if (res.includes('Admin')) {
          this.showDoctorColumn = true;
          this.showNurseColumn = true;
        } else if (res.includes('Doctors')) {
          this.showNurseColumn = true;
        } else if (res.includes('Nurse')) {
          this.showDoctorColumn = true;
        }
      }
      this.cd.detectChanges();
    });
  }
}
