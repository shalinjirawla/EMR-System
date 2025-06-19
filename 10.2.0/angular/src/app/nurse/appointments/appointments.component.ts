import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
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
import { CreateAppoinmentComponent } from '../create-appoinment/create-appoinment.component';
import { EditAppoinmentComponent } from '../edit-appoinment/edit-appoinment.component';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { ChipModule } from 'primeng/chip';
@Component({
    selector: 'app-appointments',
    templateUrl: './appointments.component.html',
    styleUrl: './appointments.component.css',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, ChipModule,PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, DatePipe],
    providers: [AppointmentServiceProxy, UserServiceProxy]
})
export class AppointmentsComponent extends PagedListingComponentBase<AppointmentDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    appointMents: AppointmentDto[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;
    patients!: UserDto[];
    statusOptions = [
        { label: 'Scheduled', value: AppointmentStatus._0 },
        { label: 'Checked In', value: AppointmentStatus._1 },
        { label: 'Completed', value: AppointmentStatus._2 },
        { label: 'Cancelled', value: AppointmentStatus._3 },
        { label: 'Rescheduled', value: AppointmentStatus._4 },
    ];
    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _userService: UserServiceProxy,
        private _apointMentService: AppointmentServiceProxy,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
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

        this._apointMentService
            .getAll(
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
}
