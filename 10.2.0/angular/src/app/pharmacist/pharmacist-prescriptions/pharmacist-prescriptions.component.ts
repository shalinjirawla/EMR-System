import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { Table, TableModule } from 'primeng/table';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { CollectionStatus, MedicineOrderDto, MedicineOrderServiceProxy, PharmacistPrescriptionsDto, PharmacistPrescriptionsDtoPagedResultDto, PharmacistPrescriptionsServiceProxy, PrescriptionDtoPagedResultDto, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
import { NgIf, DatePipe, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { MenuModule } from 'primeng/menu';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { CreatePharmacistPrescriptionComponent } from "../create-pharmacist-prescription/create-pharmacist-prescription.component"
import { DialogModule } from 'primeng/dialog';
@Component({
    selector: 'app-pharmacist-prescriptions',
    animations: [appModuleAnimation()],
    imports: [FormsModule, TableModule, PrimeTemplate, CalendarModule, NgIf, PaginatorModule,
        ButtonModule, LocalizePipe, DatePipe, CommonModule, OverlayPanelModule, MenuModule, DialogModule],
    templateUrl: './pharmacist-prescriptions.component.html',
    styleUrl: './pharmacist-prescriptions.component.css',
    providers: [PrescriptionServiceProxy, PharmacistPrescriptionsServiceProxy, PrescriptionServiceProxy]

})
export class PharmacistPrescriptionsComponent extends PagedListingComponentBase<PharmacistPrescriptionsDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    visible: boolean = false;
    medicineorder: MedicineOrderDto[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;
    selectedRecordForView!: PharmacistPrescriptionsDto;
    constructor(
        injector: Injector,
        private _pharmacistPrescriptionsService: PharmacistPrescriptionsServiceProxy,
        private _prescriptionsService: PrescriptionServiceProxy,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
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

        this._pharmacistPrescriptionsService
            .getPrescriptionFulfillment(
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: PharmacistPrescriptionsDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(medicineorder: PharmacistPrescriptionsDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
            if (result) {
                this._pharmacistPrescriptionsService.delete(medicineorder.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }
    View(selectedRecord: PharmacistPrescriptionsDto) {
        this.selectedRecordForView = selectedRecord;
        this.visible = true;
    }
    createNew(id?: number): void {
        let createDialog: BsModalRef;
        createDialog = this._modalService.show(CreatePharmacistPrescriptionComponent, {
            class: 'modal-lg',
        });

        createDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
    statusOptions = [
        { label: 'NotPickedUp', value: CollectionStatus._0 },
        { label: 'PickedUp', value: CollectionStatus._1 },
    ];
    getStatusLabel(value: number): string {
        const status = this.statusOptions.find(s => s.value === value);
        return status ? status.label : '';
    }
    getStatusSeverity(value: number): 'info' | 'warn' | 'success' {
        switch (value) {
            case CollectionStatus._0: return 'warn';        // Pending
            case CollectionStatus._1: return 'success';   // In_review
            default: return 'info';
        }
    }
}

