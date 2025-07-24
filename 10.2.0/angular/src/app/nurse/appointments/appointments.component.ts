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
import { CreateAppoinmentComponent } from '../create-appoinment/create-appoinment.component';
import { EditAppoinmentComponent } from '../edit-appoinment/edit-appoinment.component';
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { ViewAppointmentReceiptComponent } from '../view-appointment-receipt/view-appointment-receipt.component';
import { TagModule } from 'primeng/tag';
@Component({
    selector: 'app-appointments',
    templateUrl: './appointments.component.html',
    styleUrl: './appointments.component.css',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, ChipModule,TagModule, SelectModule, MenuModule, ButtonModule,TagModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, DatePipe],
    providers: [AppointmentServiceProxy, UserServiceProxy]
})
export class AppointmentsComponent extends PagedListingComponentBase<AppointmentDto> implements OnInit {
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
        { label: 'Rescheduled', value: AppointmentStatus._1 },
        { label: 'Checked In', value: AppointmentStatus._2 },
        { label: 'Completed', value: AppointmentStatus._3 },
        { label: 'Cancelled', value: AppointmentStatus._4 }
    ];
    appointmentStatus!: any;
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
    getStatusSeverity(value: number): 'info' | 'warn' | 'success' | 'danger' | 'secondary' | 'contrast' {
        switch (value) {
            case AppointmentStatus._0: return 'info';        // Scheduled
            case AppointmentStatus._1: return 'secondary';   // Rescheduled
            case AppointmentStatus._2: return 'success';     // Checked In
            case AppointmentStatus._3: return 'success';     // Completed
            case AppointmentStatus._4: return 'danger';      // Cancelled
            default: return 'contrast';
        }
    }
    
    changeStatusofAppoinment(id: number, status: AppointmentStatus) {
    this._apointMentService.markAsAction(id, status).subscribe(res => {
      this.list();
      this.cd.detectChanges();
    })
  }
}
