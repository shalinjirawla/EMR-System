import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { Table, TableModule } from 'primeng/table';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { CollectionStatus, MedicineOrderDto, MedicineOrderServiceProxy, NurseDto, NurseServiceProxy, PharmacistPrescriptionItemWithUnitPriceDto, PharmacistPrescriptionsDto, PharmacistPrescriptionsDtoPagedResultDto, PharmacistPrescriptionsServiceProxy, PrescriptionDtoPagedResultDto, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
import { NgIf, DatePipe, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { MenuModule } from 'primeng/menu';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { CreatePharmacistPrescriptionComponent } from "../create-pharmacist-prescription/create-pharmacist-prescription.component"
import { EditPharmacistPrescriptionComponent } from "../edit-pharmacist-prescription/edit-pharmacist-prescription.component"
import { ViewPharmacistPrescriptionComponent } from "../view-pharmacist-prescription/view-pharmacist-prescription.component"
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
@Component({
    selector: 'app-pharmacist-prescriptions',
    animations: [appModuleAnimation()],
    imports: [FormsModule, TableModule, PrimeTemplate, CalendarModule, NgIf, PaginatorModule,
        ButtonModule, LocalizePipe, DatePipe, SelectModule, CheckboxModule, CommonModule, TagModule, OverlayPanelModule, MenuModule, DialogModule],
    templateUrl: './pharmacist-prescriptions.component.html',
    styleUrl: './pharmacist-prescriptions.component.css',
    providers: [NurseServiceProxy, PharmacistPrescriptionsServiceProxy]

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
    statusOptions = [
        { label: 'Not PickedUp', value: CollectionStatus._0 },
        { label: 'Picked Up', value: CollectionStatus._1 },
    ];
    pickupDialog: boolean = false;
    selectedID!: number;
    selectedNurseId!: number;
    nurseOptions: NurseDto[] = []; // Load from backend
    isPickedpByPatient = false;
    constructor(
        injector: Injector,
        private _pharmacistPrescriptionsService: PharmacistPrescriptionsServiceProxy,
        private _nurseService: NurseServiceProxy,
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
                const filterdList = result.items.filter(x => x.isPaid);
                this.primengTableHelper.records = filterdList;
                this.primengTableHelper.totalRecordsCount = filterdList.length;
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
    createUpdate(id?: number): void {
        let createDialog: BsModalRef;
        if (!id) {
            createDialog = this._modalService.show(CreatePharmacistPrescriptionComponent, {
                class: 'modal-xl',
            });
        } else {
            createDialog = this._modalService.show(EditPharmacistPrescriptionComponent, {
                class: 'modal-xl',
                initialState: {
                    id: id,
                },
            });
        }
        createDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
    ViewPharmacistPrescriptions(prescriptionId: number, pharmacistPrescriptionId: number) {
        let createDialog: BsModalRef;
        createDialog = this._modalService.show(ViewPharmacistPrescriptionComponent, {
            class: 'modal-xl',
            initialState: {
                _prescriptionId: prescriptionId,
                _pharmacistPrescriptionId: pharmacistPrescriptionId
            },
        });
    }
    getStatusLabel(value: number): string {
        const status = this.statusOptions.find(s => s.value === value);
        const dataa = status ? status.label : '';
        return dataa;
    }
    getStatusSeverity(value: number): 'info' | 'warn' | 'success' {
        switch (value) {
            case CollectionStatus._0: return 'warn';        // Pending
            case CollectionStatus._1: return 'success';   // In_review
            default: return 'info';
        }
    }
    dispensePrescription(record: PharmacistPrescriptionItemWithUnitPriceDto[]) {
        alert("Prescription dispensed successfully!");
    }
    markPickedUp() {
        if (this.isPickedpByPatient) {
            this.selectedNurseId = 0;
        }
        this._pharmacistPrescriptionsService.markAsPickedUp(this.selectedID, this.selectedNurseId, !this.isPickedpByPatient).subscribe({
            next: (res) => {
                this.pickupDialog = false;
                this.selectedID = null;
                this.selectedNurseId = null;
                this.refresh();
            }, error: (err) => {

            }
        })
    }
    closePickupDialog() {
        this.pickupDialog = false;
        this.isPickedpByPatient = false;
        this.selectedID = null;
        this.selectedNurseId = null;
    }
    openPickupDialog(record_id: number) {
        this.selectedID = record_id;
        this.pickupDialog = true;
        this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
            next: (res) => {
                this.nurseOptions = res.items;
            },
            error: (err) => {

            }
        });
    }
    changeToPatient(event: any) {
        this.isPickedpByPatient = event;
        this.selectedNurseId = null;
    }
}

