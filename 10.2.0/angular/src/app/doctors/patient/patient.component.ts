import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DischargeStatus, PatientDischargeServiceProxy, PatientDto, PatientServiceProxy, PatientsForDoctorAndNurseDto, PatientsForDoctorAndNurseDtoPagedResultDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { FormsModule } from '@node_modules/@angular/forms';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { NgIf } from '@node_modules/@angular/common';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { PatientProfileComponent } from '@app/patient/patient-profile/patient-profile.component';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { CreatePrescriptionsComponent } from '../create-prescriptions/create-prescriptions.component';
import { Router } from '@angular/router';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
@Component({
    selector: 'app-patient',
    imports: [FormsModule, TableModule, ButtonModule, BreadcrumbModule, TooltipModule,
        ConfirmDialogModule, TagModule, PrimeTemplate, NgIf, CardModule, AvatarModule, CheckboxModule,
        PaginatorModule, LocalizePipe, OverlayPanelModule, AvatarGroupModule, SelectModule, InputTextModule,
        MenuModule],
    animations: [appModuleAnimation()],
    templateUrl: './patient.component.html',
    styleUrl: './patient.component.css',
    providers: [PatientServiceProxy, PatientDischargeServiceProxy, UserServiceProxy, ConfirmationService]
})
export class PatientComponent extends PagedListingComponentBase<PatientDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    items: MenuItem[] = []; // breadcrumb
    patientMenus: MenuItem[] = []; // row actions
    patients: PatientDto[] = [];

    keyword = '';
    isActive: boolean | null | undefined;
    showDoctorColumn = false;
    showNurseColumn = false;
    selectedRecord: PatientsForDoctorAndNurseDto;

    statusOptions = [
        { label: 'Pending', value: DischargeStatus._0 },
        { label: 'Initiated', value: DischargeStatus._1 },
        { label: 'Sent To Doctor', value: DischargeStatus._2 },
        { label: 'Doctor Verified', value: DischargeStatus._3 },
        { label: 'Sent To LabTechnician', value: DischargeStatus._4 },
        { label: 'LabTechnician Completed', value: DischargeStatus._5 },
        { label: 'Sent To Billing', value: DischargeStatus._6 },
        { label: 'Billing Completed', value: DischargeStatus._7 },
        { label: 'Final Approval', value: DischargeStatus._8 },
        { label: 'Discharged', value: DischargeStatus._9 },
    ];

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _userService: UserServiceProxy,
        private _patientService: PatientServiceProxy,
        private router: Router,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    ngOnInit(): void {
        this.GetLoggedInUserRole();

        // Breadcrumb
        this.items = [
            { label: 'Home', routerLink: '/' },
            { label: this.l('Patients') }
        ];
    }
    getPatientMenu(record: PatientsForDoctorAndNurseDto): MenuItem[] {
        return [
            {
                label: this.l('View'),
                icon: 'pi pi-eye',
                command: () => this.showPatientDetailsDialog(record.id),
            },
            {
                label: this.l('Prescription'),
                icon: 'pi pi-file-edit',
                command: () => this.showCreatePrescriptionDialog({ id: record.id } as PatientDto),
            },
            {
                label: this.l('Discharge'),
                icon: 'pi pi-sign-out',
                visible: record.isAdmitted && record.discharge_Status < 9,
                command: () => this.gotoDischargeSummaryPage(record.id),
            },
            { separator: true },
            {
                label: this.l('Delete'),
                icon: 'pi pi-trash',
                command: () => this.delete({ id: record.id } as PatientDto),
            },
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

        this._patientService
            .patientsForDoctor(
                this.keyword,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe((result: PatientsForDoctorAndNurseDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.cd.detectChanges();
            });
    }

    delete(patient: PatientDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
            if (result) {
                this._patientService.delete(patient.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }

    showCreatePrescriptionDialog(patient: PatientDto): void {
        const createPrescriptionDialog: BsModalRef = this._modalService.show(CreatePrescriptionsComponent, {
            class: 'modal-lg',
            initialState: { selectedPatient: patient },
        });

        createPrescriptionDialog.content.onSave.subscribe(() => this.refresh());
    }

    showCreatePatientDialog(): void {
        const createOrEditPatientDialog: BsModalRef = this._modalService.show(CreateUserDialogComponent, {
            class: 'modal-lg',
            initialState: { defaultRole: 'Patient', disableRoleSelection: true },
        });

        createOrEditPatientDialog.content.onSave.subscribe(() => this.refresh());
    }

    showPatientDetailsDialog(id: number): void {
        this._modalService.show(PatientProfileComponent, {
            class: 'modal-lg',
            initialState: { id: id },
        });
    }

    calculateAge(dob: string | Date): number {
        const birthDate = new Date(dob);
        const today = new Date();
        let age = today.getFullYear() - birthDate.getFullYear();

        const hasBirthdayPassed =
            today.getMonth() > birthDate.getMonth() ||
            (today.getMonth() === birthDate.getMonth() && today.getDate() >= birthDate.getDate());

        if (!hasBirthdayPassed) age--;
        return age;
    }

    getRandomAvatar(): string {
        const randomId = Math.floor(Math.random() * 100) + 1;
        return `https://i.pravatar.cc/150?img=${randomId}`;
    }

    GetLoggedInUserRole(): void {
        this._patientService.getCurrentUserRoles().subscribe((res) => {
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
        const status = this.statusOptions.find((s) => s.value === value);
        return status ? status.label : '';
    }

    getStatusSeverity(value: number): string {
        switch (value) {
            case DischargeStatus._0:
            case DischargeStatus._1:
            case DischargeStatus._2:
            case DischargeStatus._3:
            case DischargeStatus._4:
            case DischargeStatus._5:
            case DischargeStatus._6:
            case DischargeStatus._7:
                return 'badge-soft-primary p-1 rounded';
            case DischargeStatus._8:
            case DischargeStatus._9:
                return 'badge-soft-success p-1 rounded';
            default:
                return 'badge-soft-teal p-1 rounded';
        }
    }

    gotoDischargeSummaryPage(id: number): void {
        this.router.navigate(['app/patient-discharge/create', id]);
    }
}
