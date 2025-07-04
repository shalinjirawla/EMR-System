import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { UserServiceProxy, UserDto, UserDtoPagedResultDto, AppointmentDto, AppointmentServiceProxy, AppointmentDtoPagedResultDto, PatientDto, AppointmentStatus } from '@shared/service-proxies/service-proxies';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { EditAppoinmentComponent } from '@app/nurse/edit-appoinment/edit-appoinment.component';
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { AppSessionService } from '@shared/session/app-session.service';

@Component({
  selector: 'app-view-appoinment',
  animations: [appModuleAnimation()],
  templateUrl: './view-appoinment.component.html',
  styleUrl: './view-appoinment.component.css',
  imports: [FormsModule, TableModule, ChipModule, SelectModule, MenuModule, ButtonModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, DatePipe],
  providers: [AppointmentServiceProxy, UserServiceProxy]
})
export class ViewAppoinmentComponent extends PagedListingComponentBase<AppointmentDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  appointMents: AppointmentDto[] = [];
  AppointmentStatus = AppointmentStatus;
  keyword = '';
  status: number;
  advancedFiltersVisible = false;
  patients!: UserDto[];
  statusOptions = [
    { label: 'Scheduled', value: AppointmentStatus._0 },
    { label: 'Checked In', value: AppointmentStatus._1 },
    { label: 'Completed', value: AppointmentStatus._2 },
    { label: 'Cancelled', value: AppointmentStatus._3 },
    { label: 'Rescheduled', value: AppointmentStatus._4 },
  ];
  appointmentStatus!: any;
  showDoctorColumn: boolean = false;
  showNurseColumn: boolean = false;


  constructor(
    injector: Injector,
    private _appSessionService: AppSessionService,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _userService: UserServiceProxy,
    private _apointMentService: AppointmentServiceProxy,
    cd: ChangeDetectorRef,
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }
  ngOnInit(): void {
    this.GetLoggedInUserRole();
  }
  clearFilters(): void {
    this.keyword = '';
    this.status = undefined;
    this.list();
  }

  onStatusChange() {
    this.list(); // or this.list() depending on your implementation
  }
  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);

      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }
    this.primengTableHelper.showLoadingIndicator();
    this._apointMentService
      .getAll(
        this.keyword,
        this.status,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(
        finalize(() => {
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe((result: AppointmentDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
        this.cd.detectChanges();
      });
  }
  delete(appMt: AppointmentDto): void {
    abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
      if (result) {
        this._apointMentService.delete(appMt.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
  editAppoinment(dto: AppointmentDto): void {
    this.showCreateOrEditAppoinmentDialog(dto.id);
  }
  showCreateOrEditAppoinmentDialog(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (id > 0) {
      createOrEditUserDialog = this._modalService.show(EditAppoinmentComponent, {
        class: 'modal-lg',
        initialState: {
          id: id,
        },
      });
    }

    createOrEditUserDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
  getStatusLabel(value: number): string {
    const status = this.statusOptions.find(s => s.value === value);
    return status ? status.label : '';
  }
  getStatusClass(value: number): string {
    switch (value) {
      case AppointmentStatus._0: return 'status-scheduled';    // Scheduled
      case AppointmentStatus._1: return 'status-checkedin';    // Checked In
      case AppointmentStatus._2: return 'status-completed';    // Completed
      case AppointmentStatus._3: return 'status-cancelled';    // Cancelled
      case AppointmentStatus._4: return 'status-rescheduled';  // Rescheduled
      default: return '';
    }
  }
  GetLoggedInUserRole() {
    this._apointMentService.getCurrentUserRoles().subscribe(res => {
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
}
