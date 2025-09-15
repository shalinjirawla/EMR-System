import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DischargeStatus, PatientDischargeServiceProxy, PatientDto, PatientServiceProxy, PatientsForDoctorAndNurseDtoPagedResultDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
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
@Component({
    selector: 'app-patient',
    imports: [FormsModule, TableModule, ButtonModule, ConfirmDialogModule, TagModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, OverlayPanelModule, MenuModule],
    animations: [appModuleAnimation()],
    templateUrl: './patient.component.html',
    styleUrl: './patient.component.css',
    providers: [PatientServiceProxy, PatientDischargeServiceProxy, UserServiceProxy, ConfirmationService]
})
export class PatientComponent extends PagedListingComponentBase<PatientDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    patients: PatientDto[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;
    showDoctorColumn: boolean = false;
    showNurseColumn: boolean = false;
    statusOptions = [
        { label: 'Discharge Pending', value: DischargeStatus._0 },
        { label: 'Discharge Initiated', value: DischargeStatus._1 },
        { label: 'Doctor Summary', value: DischargeStatus._2 },
        { label: 'Sent To Billing', value: DischargeStatus._3 },
        { label: 'Billing Completed', value: DischargeStatus._4 },
        { label: 'Pharmacy Completed', value: DischargeStatus._5 },
        { label: 'Final Approval', value: DischargeStatus._6 },
        { label: 'Discharged', value: DischargeStatus._7 },
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
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: PatientsForDoctorAndNurseDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
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
        let createPrescriptionDialog: BsModalRef;
        createPrescriptionDialog = this._modalService.show(CreatePrescriptionsComponent, {
            class: 'modal-xl',
            initialState: {
                selectedPatient: patient   // 👈 patient को pass कर रहे हैं
            },
        });

        createPrescriptionDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
    showCreatePatientDialog(id?: number): void {
        let createOrEditPatientDialog: BsModalRef;
        if (!id) {
            createOrEditPatientDialog = this._modalService.show(CreateUserDialogComponent, {
                class: 'modal-lg',
                initialState: {
                    defaultRole: 'Patient',
                    disableRoleSelection: true
                }
            });
        }
        createOrEditPatientDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
    showPatientDetailsDialog(id: number): void {
        let patientDetailsDialog: BsModalRef;
        patientDetailsDialog = this._modalService.show(PatientProfileComponent, {
            class: 'modal-lg',
            initialState: {
                id: id,
            },
        });
    }
    calculateAge(dob: string | Date): number {
        const birthDate = new Date(dob);
        const today = new Date();

        let age = today.getFullYear() - birthDate.getFullYear();

        const hasBirthdayPassedThisYear =
            today.getMonth() > birthDate.getMonth() ||
            (today.getMonth() === birthDate.getMonth() && today.getDate() >= birthDate.getDate());

        if (!hasBirthdayPassedThisYear) {
            age--;
        }

        return age;
    }
    getRandomAvatar(): string {
        const randomId = Math.floor(Math.random() * 100) + 1; // IDs from 1 to 100
        return `https://i.pravatar.cc/150?img=${randomId}`;
    }
    GetLoggedInUserRole() {
        this._patientService.getCurrentUserRoles().subscribe(res => {
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
        const dataa = status ? status.label : '';
        return dataa;
    }
    getStatusSeverity(value: number) {
        switch (value) {
            case DischargeStatus._0: return 'warn';
            case DischargeStatus._1: return 'success';
            case DischargeStatus._2: return 'warn';
            case DischargeStatus._3: return 'warn';
            case DischargeStatus._4: return 'warn';
            case DischargeStatus._5: return 'warn';
            case DischargeStatus._6: return 'warn';
            case DischargeStatus._7: return 'success';
            default: return 'warn';
        }
    }
    gotoDischargeSummaryPage(id: number): void {
        this.router.navigate(['app/patient-discharge/create',id],);
    }
}
