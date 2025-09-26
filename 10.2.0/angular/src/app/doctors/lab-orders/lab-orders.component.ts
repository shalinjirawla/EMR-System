import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate, MenuItem } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ChipModule } from 'primeng/chip';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { PrescriptionLabTestDto, PrescriptionLabTestServiceProxy, LabTestStatus } from '@shared/service-proxies/service-proxies';
import { ViewLabReportComponent } from '@app/lab-technician/view-lab-report/view-lab-report.component';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-lab-orders',
  templateUrl: './lab-orders.component.html',
  styleUrl: './lab-orders.component.css',
  animations: [appModuleAnimation()],
  imports: [FormsModule,TableModule,CommonModule,BreadcrumbModule,PrimeTemplate,InputTextModule,
    OverlayPanelModule,MenuModule,ButtonModule,NgIf,PaginatorModule,ChipModule,LocalizePipe],
  providers: [PrescriptionLabTestServiceProxy]
})
export class LabOrdersComponent extends PagedListingComponentBase<PrescriptionLabTestDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;
  showDoctorColumn = false;
  showNurseColumn = false;
  items: MenuItem[] | undefined;

  testStatus = [
    { label: 'Pending', value: LabTestStatus._0 },
    { label: 'In Progress', value: LabTestStatus._1 },
    { label: 'Completed', value: LabTestStatus._2 },
  ];

  selectedRecord: PrescriptionLabTestDto;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _prescriptionLabTests: PrescriptionLabTestServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Lab Orders' },
    ];
    this.GetLoggedInUserRole();
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.list();
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
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result) => {
        const filteredList = result.items.filter(x => !x.isEmergencyPrescription);
        this.primengTableHelper.records = filteredList;
        this.primengTableHelper.totalRecordsCount = filteredList.length;
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

  ViewLabReport(id?: number): void {
    let viewReportDialog: BsModalRef;
    viewReportDialog = this._modalService.show(ViewLabReportComponent, {
      class: 'modal-lg',
      initialState: { id: id },
    });
  }

  getStatusLabel(value: number): string {
    const status = this.testStatus.find(s => s.value === value);
    return status ? status.label : '';
  }

  getStatusSeverity(value: number) {
    
    switch (value) {
      case LabTestStatus._0: return 'badge-soft-primary p-1 rounded';
      case LabTestStatus._1: return 'badge-soft-warning p-1 rounded';
      case LabTestStatus._2: return 'badge-soft-success p-1 rounded';
      default: return 'badge-soft-teal p-1 rounded';
    }
  }

  GetLoggedInUserRole(): void {
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

  getShortName(fullName: string): string {
    if (!fullName) return '';
    const words = fullName.trim().split(' ');
    const firstInitial = words[0].charAt(0).toUpperCase();
    const lastInitial = words.length > 1 ? words[words.length - 1].charAt(words.length - 1).toUpperCase() : '';
    return firstInitial + lastInitial;
  }
}
