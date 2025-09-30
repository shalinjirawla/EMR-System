import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { NgIf } from '@node_modules/@angular/common';
import { ChangeDetectorRef, Component, HostListener, Injector, OnInit, ViewChild } from '@angular/core';
import { PatientServiceProxy, UserServiceProxy, VitalDto, VitalDtoPagedResultDto, VitalServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CreateVitalsComponent } from '../create-vitals/create-vitals.component'
import { EditVitalsComponent } from '../edit-vitals/edit-vitals.component'
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
@Component({
    selector: 'app-vitals-notes',
    imports: [FormsModule, TableModule, SelectModule, CardModule, InputTextModule, TagModule, TooltipModule,
        BreadcrumbModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, OverlayPanelModule, MenuModule,
        ButtonModule],
    animations: [appModuleAnimation()],
    templateUrl: './vitals-notes.component.html',
    styleUrl: './vitals-notes.component.css',
    providers: [UserServiceProxy, VitalServiceProxy]
})
export class VitalsNotesComponent extends PagedListingComponentBase<VitalDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    // selectedRecord: VitalDto;
    patients: VitalDto[] = [];
    keyword = '';
    isActive: boolean | null;
    items: MenuItem[] | undefined;
    editDeleteMenus: MenuItem[] | undefined;
    selectedRecord: VitalDto;

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _userService: UserServiceProxy,
        private _vitalService: VitalServiceProxy,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    ngOnInit(): void {
        this.items = [
            { label: 'Home', routerLink: '/' },
            { label: 'Vitals' },
        ];
        this.editDeleteMenus = [
            {
                label: 'Edit',
                icon: 'pi pi-pencil',
                command: () => this.editVitals(this.selectedRecord)  // call edit
            },
            {
                label: 'Delete',
                icon: 'pi pi-trash',
                command: () => this.delete(this.selectedRecord)  // call edit
            }
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

        this._vitalService
            .getAll(
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
            .subscribe((result: VitalDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }
    delete(vital: VitalDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage', vital.patient.fullName), undefined, (result: boolean) => {
            if (result) {
                this._vitalService.delete(vital.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }
    editVitals(record: any): void {
        this.showCreateOrEditPrescriptionDialog(record.id);
    }

    createVitals(): void {
        this.showCreateOrEditPrescriptionDialog();
    }

    showCreateOrEditPrescriptionDialog(id?: number): void {
        let createOrEditUserDialog: BsModalRef;

        if (!id) {
            createOrEditUserDialog = this._modalService.show(CreateVitalsComponent, {
                class: 'modal-lg',
            });
        } else {
            createOrEditUserDialog = this._modalService.show(EditVitalsComponent, {
                class: 'modal-lg',
                initialState: {
                    vitalId: id  // ✅ Pass the id to modal
                },
            });
        }

        createOrEditUserDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }


}