import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PharmacistInventoryDto, PharmacistInventoryDtoPagedResultDto, PharmacistInventoryServiceProxy, PrescriptionDto } from '@shared/service-proxies/service-proxies';
import { PaginatorModule, Paginator } from "primeng/paginator";
import { TableModule, Table } from "primeng/table";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { ActivatedRoute } from "@angular/router";
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import moment from 'moment';
import { AddMedicineComponent } from '../add-medicine/add-medicine.component';
import { EditMedicineComponent } from '../edit-medicine/edit-medicine.component';
import { LocalizePipe } from "../../../shared/pipes/localize.pipe";
import { FormsModule } from '@node_modules/@angular/forms';
import { NgIf } from '@node_modules/@angular/common';
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { DatePipe } from "@angular/common";
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';



@Component({
  selector: 'app-pharmacist-inventory',
  animations: [appModuleAnimation()],
  imports: [LocalizePipe, FormsModule, TableModule,CalendarModule, PrimeTemplate, NgIf, PaginatorModule, DatePipe, ButtonModule, OverlayPanelModule, MenuModule],
  providers: [PharmacistInventoryServiceProxy],
  templateUrl: './pharmacist-inventory.component.html',
  styleUrl: './pharmacist-inventory.component.css'
})
export class PharmacistInventoryComponent extends PagedListingComponentBase<PharmacistInventoryDto> {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  PharmacistInventory: PharmacistInventoryDto[] = [];
  keyword = '';
  dateRange: Date[];
  minStock = undefined;
  maxStock = undefined;

  isAvailable: boolean | null;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _pharmacistInventoryService: PharmacistInventoryServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  clearFilters(): void {
    this.keyword = '';
    this.dateRange = [];
    this.isAvailable = undefined;
    this.minStock = undefined;
    this.maxStock = undefined;
    this.list();
}

list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
        this.paginator.changePage(0);
        if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
            return;
        }
    }

    this.primengTableHelper.showLoadingIndicator();

    // Prepare date filters
    const fromExpiryDate = this.dateRange?.[0] ? moment(this.dateRange[0]) : undefined;
    const toExpiryDate = this.dateRange?.[1] ? moment(this.dateRange[1]) : undefined;

    this._pharmacistInventoryService
        .getAll(
            this.keyword,
            this.primengTableHelper.getSorting(this.dataTable),
            this.minStock,
            this.maxStock,
            fromExpiryDate,
            toExpiryDate,
            this.isAvailable,
            this.primengTableHelper.getSkipCount(this.paginator, event),
            this.primengTableHelper.getMaxResultCount(this.paginator, event)
        )
        .pipe(finalize(() => {
            this.primengTableHelper.hideLoadingIndicator();
        }))
        .subscribe((result: PharmacistInventoryDtoPagedResultDto) => {
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.cd.detectChanges();
        });
}

  protected delete(entity: PharmacistInventoryDto): void {
    abp.message.confirm("Are you sure u want to delete this", undefined, (result: boolean) => {
      if (result) {
        this._pharmacistInventoryService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }

  addNewMedicine(): void {
    this.showCreateOrEditMedicine();
  }
  editNewMedicine(dto: PharmacistInventoryDto): void {
    this.showCreateOrEditMedicine(dto.id);
  }
  showCreateOrEditMedicine(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (!id) {
      createOrEditUserDialog = this._modalService.show(AddMedicineComponent, {
        class: 'modal-lg',
      });
    }
    else {
      createOrEditUserDialog = this._modalService.show(EditMedicineComponent, {
        class: 'modal-lg',
        initialState: {
          inventoryId: id,
        },
      });
    }

    createOrEditUserDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}
