import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { VisitListDto, VisitListDtoPagedResultDto, VisitServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { FormsModule } from '@node_modules/@angular/forms';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { NgIf } from '@node_modules/@angular/common';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { CreateVisitComponent } from '@app/doctors/create-visit/create-visit.component';
import { EditVisitComponent } from '@app/doctors/edit-visit/edit-visit.component';

@Component({
    selector: 'app-visits',
    animations: [appModuleAnimation()],
    imports: [FormsModule, TableModule, ButtonModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, OverlayPanelModule, MenuModule],
    templateUrl: './visits.component.html',
    styleUrl: './visits.component.css',
    providers: [VisitServiceProxy]
})
export class VisitsComponent extends PagedListingComponentBase<VisitListDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    patients: any[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;
    showDoctorColumn: boolean = false;
    showNurseColumn: boolean = false;
    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _visitService: VisitServiceProxy,
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

        this._visitService
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
            .subscribe((result: VisitListDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }
    delete(visits: any): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
            if (result) {
                this._visitService.delete(visits.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }
    showVistiteCreateUpdateDialog(id?: number): void {
        let createOrEditVisitDialog: BsModalRef;
        if (!id) {
            createOrEditVisitDialog = this._modalService.show(CreateVisitComponent, {
                class: 'modal-lg',
            });
        }
        else {
            createOrEditVisitDialog = this._modalService.show(EditVisitComponent, {
                class: 'modal-lg',
                initialState: {
                    id: id,
                },
            });
        }

        createOrEditVisitDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
    createVisit(): void {
        this.showVistiteCreateUpdateDialog();
    }
    editVisit(id: number): void {
        this.showVistiteCreateUpdateDialog(id);
    }
    GetLoggedInUserRole() {
        this._visitService.getCurrentUserRoles().subscribe(res => {
            this.showDoctorColumn = false;
            this.showNurseColumn = false;
            debugger
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
