import { Component, ViewChild, Injector, ChangeDetectorRef } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { PaginatorModule, Paginator } from "primeng/paginator";
import { TableModule, Table } from "primeng/table";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { LocalizePipe } from "../../../shared/pipes/localize.pipe";
import { PrescriptionServiceProxy, PrescriptionDto, PrescriptionDtoPagedResultDto } from "../../../shared/service-proxies/service-proxies";
import { CreatePrescriptionsComponent } from "../create-prescriptions/create-prescriptions.component";
import { PagedListingComponentBase } from "@shared/paged-listing-component-base";
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-prescriptions',
  animations: [appModuleAnimation()],
  imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe],
  templateUrl: './prescriptions.component.html',
  styleUrl: './prescriptions.component.css',
  providers: [PrescriptionServiceProxy]
})
export class PrescriptionsComponent extends PagedListingComponentBase<PrescriptionDto> {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  prescriptions: PrescriptionDto[] = [];
  keyword = '';
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

    this._prescriptionService
      .getAll(
        // this.keyword,
        // this.isActive,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(
        finalize(() => {
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe((result: PrescriptionDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
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
  createPrescription(): void {
    this.showCreateOrEditPrescriptionDialog();
  }
  editPrescription(dto: PrescriptionDto): void {
    this.showCreateOrEditPrescriptionDialog(dto.id);
  }
  showCreateOrEditPrescriptionDialog(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (!id) {
      createOrEditUserDialog = this._modalService.show(CreatePrescriptionsComponent, {
        class: 'modal-lg',
      });
    }
  }
}
