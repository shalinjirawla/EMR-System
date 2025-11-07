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
import { CommonModule, DatePipe, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateAppoinmentComponent } from '../create-appoinment/create-appoinment.component';
import { EditAppoinmentComponent } from '../edit-appoinment/edit-appoinment.component';
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ButtonModule } from 'primeng/button';
import { ViewAppointmentReceiptComponent } from '../view-appointment-receipt/view-appointment-receipt.component';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
@Component({
    selector: 'app-appointments',
    templateUrl: './appointments.component.html',
    styleUrl: './appointments.component.css',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, TooltipModule, CardModule, AvatarModule, AvatarGroupModule, InputTextModule,
        CheckboxModule, CommonModule, ChipModule, BreadcrumbModule, TagModule, SelectModule, MenuModule,
        ButtonModule, TagModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, DatePipe],
    providers: [AppointmentServiceProxy, UserServiceProxy]
})
export class AppointmentsComponent extends PagedListingComponentBase<AppointmentDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    appointMents: AppointmentDto[] = [];
    AppointmentStatus = AppointmentStatus;
    keyword = '';
    selectedStatuses: number[] = [];
    statuses = [
        { label: 'Scheduled', value: AppointmentStatus._0, selected: false },
        { label: 'Rescheduled', value: AppointmentStatus._1, selected: false },
        { label: 'Checked In', value: AppointmentStatus._2, selected: false },
        { label: 'Completed', value: AppointmentStatus._3, selected: false },
        { label: 'Cancelled', value: AppointmentStatus._4, selected: false },
    ];
    statusOptions = [
        { label: 'Scheduled', value: AppointmentStatus._0 },
        { label: 'Rescheduled', value: AppointmentStatus._1 },
        { label: 'Checked In', value: AppointmentStatus._2 },
        { label: 'Completed', value: AppointmentStatus._3 },
        { label: 'Cancelled', value: AppointmentStatus._4 }
    ];
    items: any[];
    selectedRecord: AppointmentDto;
    appointmentMenu: MenuItem[] = [];

    constructor(
        injector: Injector,
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
        this.items = [
            { label: 'Home', routerLink: '/' },
            { label: 'Appointments' }
        ];
        this.appointmentMenu = this.getAppointmentMenu(null);
    }

    // ✅ keep all visible conditions intact
    getAppointmentMenu(record: AppointmentDto | null): MenuItem[] {
        return [
            {
                label: this.l('Edit'),
                icon: 'pi pi-pencil',
                visible: record ? this.canEdit(record) : false,
                command: () => this.editAppoinment(record),
            },
            {
                label: this.l('View Receipt'),
                icon: 'pi pi-file',
                visible: record ? this.canViewReceipt(record) : false,
                command: () => this.viewReceipt(record),
            },
            {
                label: this.l('Pay Now'),
                icon: 'pi pi-credit-card',
                visible: record ? this.canPay(record) : false,
                command: () => this.makePayment(record),
            },
            {
                label: this.l('Cancel'),
                icon: 'pi pi-times',
                visible: record ? this.canCancel(record) : false,
                command: () => this.changeStatusofAppoinment(record.id, 4),
            },
        ];
    }

    // 🟣 call this when row is clicked
    onRowMenuClick(event: any, record: AppointmentDto, rowMenu: any): void {
        this.selectedRecord = record;
        this.appointmentMenu = this.getAppointmentMenu(record); // 👈 refresh menu visibility
        rowMenu.toggle(event);
    }
    getShortName(fullName: string | 'unknown'): string {
        if (!fullName) return '';
        const words = fullName.trim().split(' ');
        const firstInitial = words[0].charAt(0).toUpperCase();
        const lastInitial = words.length > 1 ? words[words.length - 1].charAt(0).toUpperCase() : '';
        return firstInitial + lastInitial;
    }
    clearFilters(): void {
        this.keyword = '';
        this.selectedStatuses = undefined;
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


    onStatusChange() {
        this.selectedStatuses = this.statuses.filter(s => s.selected).map(s => s.value);
        this.list();
        this.cd.detectChanges();
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
    // Add to component class
    makePayment(appointment: AppointmentDto): void {
        this._apointMentService.initiatePaymentForAppointment(appointment.id)
            .subscribe({
                next: (stripeUrl) => {
                    localStorage.setItem('pendingAppointment', JSON.stringify({
                        id: appointment.id,
                        date: appointment.appointmentDate,
                        doctor: appointment.doctor.fullName
                    }));
                    window.location.href = stripeUrl;
                },
                error: (err) => {
                    this.notify.error('Failed to initiate payment');
                    console.error(err);
                }
            });
    }

    viewReceipt(record: AppointmentDto): void {
        const modalRef: BsModalRef = this._modalService.show(
            ViewAppointmentReceiptComponent,
            {
                class: 'modal-lg',
                initialState: {
                    appointmentId: record.id
                }
            }
        );
    }
    createAppoinment(): void {
        this.showCreateOrEditAppoinmentDialog();
    }
    editAppoinment(dto: AppointmentDto): void {
        this.showCreateOrEditAppoinmentDialog(dto.id);
    }
    showCreateOrEditAppoinmentDialog(id?: number): void {
        let createOrEditUserDialog: BsModalRef;
        if (!id) {
            createOrEditUserDialog = this._modalService.show(CreateAppoinmentComponent, {
                class: 'modal-lg',
            });
        }
        else {
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
    getStatusSeverity(value: number) {
        switch (value) {
            case AppointmentStatus._0: return 'badge-soft-primary p-1 rounded';
            case AppointmentStatus._1: return 'badge-soft-warning p-1 rounded';
            case AppointmentStatus._2: return 'badge-soft-teal p-1 rounded';
            case AppointmentStatus._3: return 'badge-soft-success p-1 rounded';
            case AppointmentStatus._4: return 'badge-soft-danger p-1 rounded';
            default: return 'badge-soft-teal';
        }
    }

    changeStatusofAppoinment(id: number, status: AppointmentStatus) {
        this._apointMentService.markAsAction(id, status).subscribe(res => {
            this.list();
            this.cd.detectChanges();
        })
    }

    canEdit(record: any): boolean {
        return record.status === 0 || record.status === 1 ||
            (record.status === 4) ||
            (record.patient.isAdmitted && (record.status === 0 || record.status === 1 || record.status === 4));
    }

    canViewReceipt(record: any): boolean {
        return record.isPaid &&
            ((record.status === 0 || record.status === 1) || record.status === 4 || record.status === 3);
    }

    canPay(record: any): boolean {
        return !record.isPaid && (record.status === 0 || record.status === 1) && !record.patient.isAdmitted;
    }

    canCancel(record: any): boolean {
        return (record.status === 0 || record.status === 1) && !record.patient.isAdmitted;
    }

}
