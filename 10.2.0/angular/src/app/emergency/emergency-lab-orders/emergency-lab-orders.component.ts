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
import { LabOrderListDtoPagedResultDto, LabRequestListDto, LabTestStatus, PrescriptionLabTestDto, PrescriptionLabTestServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ChipModule } from 'primeng/chip';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { EditLabReportComponent } from '@app/lab-technician/edit-lab-report/edit-lab-report.component';
import { GenerateLabReportComponent } from '@app/lab-technician/generate-lab-report/generate-lab-report.component';
import { ViewLabReportComponent } from '@app/lab-technician/view-lab-report/view-lab-report.component';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
@Component({
  selector: 'app-emergency-lab-orders',
  templateUrl: './emergency-lab-orders.component.html',
  styleUrl: './emergency-lab-orders.component.css',
  animations: [appModuleAnimation()],
  imports: [FormsModule, TableModule, TagModule, AvatarModule, BreadcrumbModule, CheckboxModule,
    AvatarGroupModule, CommonModule, PrimeTemplate, TooltipModule, InputTextModule,
    OverlayPanelModule, MenuModule, ButtonModule, NgIf, CardModule, SelectModule,
    PaginatorModule, ChipModule, LocalizePipe],
  providers: [PrescriptionLabTestServiceProxy]
})
export class EmergencyLabOrdersComponent extends PagedListingComponentBase<PrescriptionLabTestDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  items: MenuItem[] | undefined;
  editDeleteMenus: MenuItem[] | undefined;
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
  selectedRecord: PrescriptionLabTestDto;
  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _prescriptionLabTests: PrescriptionLabTestServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }
  ngOnInit(): void {
    this.GetLoggedInUserRole();
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Emergency Lab Orders' },
    ];
    this.editDeleteMenus = [
      {
        label: 'View',
        icon: 'pi pi-eye',
        command: () => this.ViewLabReport(this.selectedRecord.id)  // call edit
      }
    ];
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
        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => {
        this.primengTableHelper.hideLoadingIndicator();
      }))
      .subscribe((result: LabOrderListDtoPagedResultDto) => {
        const filterList = result.items.filter(x => x.isEmergencyPrescription == true);
        this.primengTableHelper.records = filterList;
        this.primengTableHelper.totalRecordsCount = filterList.length;
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
          patientName: record.patientName,
          isEmergencyCase: record.isEmergencyPrescription,
          emergencyCaseId: record.emergencyCaseId,
        },
      });
    }
    createReportDialog.content.onSave.subscribe(() => {
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
  }
  GetLoggedInUserRole() {
    this._prescriptionLabTests.getCurrentUserRoles().subscribe(res => {
      this.showDoctorColumn = false;
      this.showNurseColumn = false;
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
   getShortName(fullName: string | 'unknown'): string {
    if (!fullName) return '';
    const words = fullName.trim().split(' ');
    const firstInitial = words[0].charAt(0).toUpperCase();
    const lastInitial = words.length > 1 ? words[words.length - 1].charAt(0).toUpperCase() : '';
    return firstInitial + lastInitial;
  }
}
