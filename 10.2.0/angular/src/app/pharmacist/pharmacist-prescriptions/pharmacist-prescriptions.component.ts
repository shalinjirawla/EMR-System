import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { Table, TableModule } from 'primeng/table';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { PrescriptionDto, PrescriptionDtoPagedResultDto, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
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
  providers: [PrescriptionServiceProxy]

})
export class PharmacistPrescriptionsComponent extends PagedListingComponentBase<PrescriptionDto> {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  prescriptions: PrescriptionDto[] = [];
  keyword = '';
  dateRange: Date[];

  isActive: boolean | null;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _prescriptionService: PrescriptionServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  clearFilters(): void {
    this.keyword = '';
    this.dateRange = [];
    this.list();
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    const fromDate = this.dateRange?.[0] ? moment(this.dateRange[0]) : undefined;
    const toDate = this.dateRange?.[1] ? moment(this.dateRange[1]) : undefined;

    this.primengTableHelper.showLoadingIndicator();

    this._prescriptionService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        fromDate,
        toDate,
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => {
        this.primengTableHelper.hideLoadingIndicator();
      }))
      .subscribe((result: PrescriptionDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }


  protected delete(entity: PrescriptionDto): void {
    abp.message.confirm("Are you sure u want to delete this", undefined, (result: boolean) => {
      if (result) {
        this._prescriptionService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
  // createPrescription(): void {
  //   this.showCreateOrEditPrescriptionDialog();
  // }
  // editPrescription(dto: PrescriptionDto): void {
  //   this.showCreateOrEditPrescriptionDialog(dto.id);
  // }
  // showCreateOrEditPrescriptionDialog(id?: number): void {
  //   let createOrEditUserDialog: BsModalRef;
  //   if (!id) {
  //     createOrEditUserDialog = this._modalService.show(CreatePrescriptionsComponent, {
  //       class: 'modal-lg',
  //     });
  //   }
  //   else {
  //     createOrEditUserDialog = this._modalService.show(EditPrescriptionsComponent, {
  //       class: 'modal-lg',
  //       initialState: {
  //         id: id,
  //       },
  //     });
  //   }

  //   createOrEditUserDialog.content.onSave.subscribe(() => {
  //     this.refresh();
  //   });
  // }
}
