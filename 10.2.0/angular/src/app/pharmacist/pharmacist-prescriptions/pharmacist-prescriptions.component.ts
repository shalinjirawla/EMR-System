import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { Table, TableModule } from 'primeng/table';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { MedicineOrderDto, MedicineOrderDtoPagedResultDto, MedicineOrderServiceProxy, PrescriptionDto, PrescriptionDtoPagedResultDto, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { NgIf, DatePipe, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { MenuModule } from 'primeng/menu';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { appModuleAnimation } from "../../../shared/animations/routerTransition";

@Component({
  selector: 'app-pharmacist-prescriptions',
    animations: [appModuleAnimation()],
  imports: [FormsModule, TableModule, PrimeTemplate,CalendarModule, NgIf, PaginatorModule,
     ButtonModule, LocalizePipe, DatePipe, CommonModule, OverlayPanelModule, MenuModule],
  templateUrl: './pharmacist-prescriptions.component.html',
  styleUrl: './pharmacist-prescriptions.component.css',
  providers: [PrescriptionServiceProxy,MedicineOrderServiceProxy]

})
export class PharmacistPrescriptionsComponent extends PagedListingComponentBase<MedicineOrderDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

   medicineorder: MedicineOrderDto[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;

    constructor(
        injector: Injector,
        private _orderService: MedicineOrderServiceProxy,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    // createOrder(): void {
    //     this.showCreateOrEditOrderDialog();
    // }

    // editOrder(medicineorder: MedicineOrderDto): void {
    //     this.showCreateOrEditOrderDialog(medicineorder.id);
    // }

    // public resetPassword(medicineorder: MedicineOrderDto): void {
    //     this.showResetPasswordUserDialog(medicineorder.id);
    // }

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

        this._orderService
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
            .subscribe((result: MedicineOrderDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(medicineorder: MedicineOrderDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
            if (result) {
                this._orderService.delete(medicineorder.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }

    // private showResetPasswordUserDialog(id?: number): void {
    //     this._modalService.show(ResetPasswordDialogComponent, {
    //         class: 'modal-lg',
    //         initialState: {
    //             id: id,
    //         },
    //     });
    // }

    // private showCreateOrEditOrderDialog(id?: number): void {
    //     let createOrEditUserDialog: BsModalRef;
    //     if (!id) {
    //         createOrEditUserDialog = this._modalService.show(CreateOrderMedicineComponent, {
    //             class: 'modal-lg',
    //         });
    //     } else {
    //         createOrEditUserDialog = this._modalService.show(EditOrderMedicineComponent, {
    //             class: 'modal-lg',
    //             initialState: {
    //                 orderId: id,
    //             },
    //         });
    //     }

    //     createOrEditUserDialog.content.onSave.subscribe(() => {
    //         this.refresh();
    //     });
    // }
}

