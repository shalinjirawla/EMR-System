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
import { DatePipe, NgIf, CommonModule } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { EditAppoinmentComponent } from '@app/nurse/edit-appoinment/edit-appoinment.component';
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { AppSessionService } from '@shared/session/app-session.service';
import { TagModule } from 'primeng/tag';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';

@Component({
  selector: 'app-view-appoinment',
  animations: [appModuleAnimation()],
  templateUrl: './view-appoinment.component.html',
  styleUrl: './view-appoinment.component.css',
  imports: [FormsModule, TableModule, TagModule, ChipModule, BreadcrumbModule, CardModule, AvatarGroupModule, CheckboxModule,
    SelectModule, MenuModule, ButtonModule, OverlayPanelModule, CommonModule, TooltipModule, AvatarModule, InputTextModule,
    PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, DatePipe],
  providers: [AppointmentServiceProxy, UserServiceProxy]
})
export class ViewAppoinmentComponent extends PagedListingComponentBase<AppointmentDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  appointMents: AppointmentDto[] = [];
  AppointmentStatus = AppointmentStatus;
  keyword = '';
  selectedStatuses: number[] = [];
  advancedFiltersVisible = false;
  patients!: UserDto[];
  statusOptions = [
    { label: 'Scheduled', value: AppointmentStatus._0 },
    { label: 'Rescheduled', value: AppointmentStatus._1 },
    { label: 'Checked In', value: AppointmentStatus._2 },
    { label: 'Completed', value: AppointmentStatus._3 },
    { label: 'Cancelled', value: AppointmentStatus._4 },
  ];
  statuses = [
    { label: 'Scheduled', value: AppointmentStatus._0, selected: false },
    { label: 'Rescheduled', value: AppointmentStatus._1, selected: false },
    { label: 'Checked In', value: AppointmentStatus._2, selected: false },
    { label: 'Completed', value: AppointmentStatus._3, selected: false },
    { label: 'Cancelled', value: AppointmentStatus._4, selected: false },
  ];
  showDoctorColumn: boolean = false;
  showNurseColumn: boolean = false;
  editDeleteMenus: MenuItem[] | undefined;
  items: MenuItem[] | undefined;
  selectedRecord: AppointmentDto;
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
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Appointments' },
    ];
    this.editDeleteMenus = [
      {
        label: 'Rescheduled',
        icon: 'pi pi-clock',
        visible: this.selectedRecord?.status !== 0 && this.selectedRecord?.status !== 1,
        command: () => this.reschedulAppoinment(this.selectedRecord.id)
      },
      {
        label: 'Mark as In Progress',
        icon: 'pi pi-check',
        visible: this.selectedRecord?.status !== 1 && this.selectedRecord?.status !== 0,
        command: () => this.changeStatusofAppoinment(this.selectedRecord.id, 2)
      },
      {
        label: 'Cancel',
        icon: 'pi pi-times',
        visible: this.selectedRecord?.status !== 3,
        command: () => this.changeStatusofAppoinment(this.selectedRecord.id, 4)
      }
    ];
  }
  clearFilters(): void {
    this.keyword = '';
    this.selectedStatuses = undefined;
    this.list();
  }
  onStatusChange() {
    this.selectedStatuses = this.statuses
      .filter(s => s.selected)
      .map(s => s.value);
    this.cd.detectChanges();
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
    this._apointMentService
      .getAll(
        this.keyword,
        this.selectedStatuses,
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
  reschedulAppoinment(id?: number): void {
    let reschedulAppoinmentDialog: BsModalRef;
    if (id > 0) {
      reschedulAppoinmentDialog = this._modalService.show(EditAppoinmentComponent, {
        class: 'modal-lg',
        initialState: {
          id: id,
          status: 4,
        },
      });
    }
  }
  getStatusLabel(value: number): string {
    const status = this.statusOptions.find(s => s.value === value);
    return status ? status.label : '';
  }
  getStatusSeverity(value: number) {
    switch (value) {
      case AppointmentStatus._0: return 'badge-soft-primary p-1 rounded';        // Scheduled
      case AppointmentStatus._1: return 'badge-soft-warning p-1 rounded';   // Rescheduled
      case AppointmentStatus._2: return 'badge-soft-purple p-1 rounded';     // Checked In
      case AppointmentStatus._3: return 'badge-soft-success p-1 rounded';     // Completed
      case AppointmentStatus._4: return 'badge-soft-danger p-1 rounded';      // Cancelled
      default: return 'badge-soft-teal p-1 rounded';
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
  changeStatusofAppoinment(id: number, status: AppointmentStatus) {
    this._apointMentService.markAsAction(id, status).subscribe(res => {
      this.list();
      this.cd.detectChanges();
    })
  }
  getShortName(fullName: string | 'unknown'): string {
    if (!fullName) return '';
    const words = fullName.trim().split(' ');
    const firstInitial = words[0].charAt(0).toUpperCase();
    const lastInitial = words.length > 1 ? words[words.length - 1].charAt(0).toUpperCase() : '';
    return firstInitial + lastInitial;
  }
} 
