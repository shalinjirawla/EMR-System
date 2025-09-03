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
import { ConsultationRequestsDto, ConsultationRequestsServiceProxy, CreateUpdateConsultationRequestsDto, Status } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ChipModule } from 'primeng/chip';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
@Component({
  selector: 'app-consultation-requests',
  templateUrl: './consultation-requests.component.html',
  styleUrl: './consultation-requests.component.css',
  animations: [appModuleAnimation()],
  imports: [FormsModule, TableModule, DialogModule, TextareaModule, TagModule, CommonModule, PrimeTemplate, OverlayPanelModule, MenuModule, ButtonModule, NgIf, PaginatorModule, ChipModule, LocalizePipe],
  providers: [ConsultationRequestsServiceProxy]
})
export class ConsultationRequestsComponent extends PagedListingComponentBase<ConsultationRequestsDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  visible: boolean = false;
  keyword = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;
  showDoctorColumn: boolean = false;
  showNurseColumn: boolean = false;
  selectedRecords!: ConsultationRequestsDto;
  _adviceResponse!: any;
  statusOptions = [
    { label: 'Pending', value: Status._0 },
    { label: 'In_review', value: Status._1 },
    { label: 'Completed', value: Status._2 },
  ];
  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _consultationRequestsService: ConsultationRequestsServiceProxy,
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

    this._consultationRequestsService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => {
        this.primengTableHelper.hideLoadingIndicator();
      }))
      .subscribe((result) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount
        this.cd.detectChanges();
      });
  }
  delete(entity: ConsultationRequestsDto): void {
    abp.message.confirm("Are you sure u want to delete this", undefined, (result: boolean) => {
      if (result) {
        this._consultationRequestsService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
  GetLoggedInUserRole() {
    this._consultationRequestsService.getCurrentUserRoles().subscribe(res => {
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
  getStatusLabel(value: number): string {
    const status = this.statusOptions.find(s => s.value === value);
    return status ? status.label : '';
  }
  getStatusSeverity(value: number): 'info' | 'warn' | 'success' | 'contrast' {
    switch (value) {
      case Status._0: return 'warn';        // Pending
      case Status._1: return 'info';   // In_review
      case Status._2: return 'success';     // Completed
      default: return 'contrast';
    }
  }
  showDialog(record: ConsultationRequestsDto) {
    this.selectedRecords = record;
    this.visible = true;
    if (this.selectedRecords?.adviceResponse) {
      this._adviceResponse = this.selectedRecords.adviceResponse;
    }
  }
  UpdateStatus(_selectedRecords: ConsultationRequestsDto, _status: any) {
    const dto: any = {
      id: _selectedRecords.id,
      tenantId: _selectedRecords.tenantId,
      prescriptionId: _selectedRecords.prescriptionId,
      requestingDoctorId: _selectedRecords.requestingDoctorId,
      requestedSpecialistId: _selectedRecords.requestedSpecialistId,
      status: _status,
      notes: _selectedRecords.notes,
      adviceResponse: this._adviceResponse ? this._adviceResponse : null,
    }
    this._consultationRequestsService.update(dto).subscribe({
      next: (res) => {
        this.visible = false;
        this.refresh();
      }, error: (err) => {
      }
    });
  }
  OnDialogBoxHide() {
    this.visible = false;
    this._adviceResponse = '';
  }
  isInPending(_status: any) {
    return _status == Status._0
  }
  isInReview(_status: any) {
    return _status == Status._1
  }
  isCompleted(_status: any) {
    return _status == Status._2
  }
}
