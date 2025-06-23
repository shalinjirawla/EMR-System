import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PatientDto, PatientServiceProxy, PatientsForDoctorAndNurseDtoPagedResultDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
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
@Component({
    selector: 'app-patient',
    imports: [FormsModule, TableModule, ButtonModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, OverlayPanelModule, MenuModule],
    animations: [appModuleAnimation()],
    templateUrl: './patient.component.html',
    styleUrl: './patient.component.css',
    providers: [PatientServiceProxy, UserServiceProxy]
})
export class PatientComponent extends PagedListingComponentBase<PatientDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    patients: PatientDto[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _userService: UserServiceProxy,
        private _patientService: PatientServiceProxy,
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

        this._patientService
            .patientsForDoctor(
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
        // else {
        //     createOrEditPatientDialog = this._modalService.show(EditPatientsComponent, {
        //         class: 'modal-lg',
        //         initialState: {
        //             id: id,
        //         },
        //     });
        // }
        createOrEditPatientDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }

    showPatientDetailsDialog(): void {
        let patientDetailsDialog: BsModalRef;
        patientDetailsDialog = this._modalService.show(PatientProfileComponent, {
            class: 'modal-lg',
        });

        // patientDetailsDialog.content.onSave.subscribe(() => {
        //     this.refresh();
        // });
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

}
