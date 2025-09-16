import { Component, ViewChild, Injector, ChangeDetectorRef, OnInit } from '@angular/core';
import { CreateupdateMedicineStrengthTypeComponent } from '../createupdate-medicine-strength-type/createupdate-medicine-strength-type.component';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { StrengthUnitMasterDto, StrengthUnitMasterDtoPagedResultDto, StrengthUnitMasterServiceProxy } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'app-medicine-strength-type',
  templateUrl: './medicine-strength-type.component.html',
  styleUrl: './medicine-strength-type.component.css',
  providers: [StrengthUnitMasterServiceProxy],
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    FormsModule,
    TableModule,
    Paginator,
    ButtonModule,
    PrimeTemplate,
    NgIf,
    LocalizePipe
  ],
})
export class MedicineStrengthTypeComponent extends PagedListingComponentBase<StrengthUnitMasterDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  isActive: boolean | undefined = undefined;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _medicineStrengthTypeService: StrengthUnitMasterServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {}

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._medicineStrengthTypeService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: StrengthUnitMasterDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  createMedicineStrengthType(): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateMedicineStrengthTypeComponent, {
      class: 'modal-lg',
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  editMedicineStrengthType(strengthType: StrengthUnitMasterDto): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateMedicineStrengthTypeComponent, {
      class: 'modal-lg',
      initialState: { id: strengthType.id },
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  deleteMedicineStrengthType(strengthType: StrengthUnitMasterDto): void {
    abp.message.confirm(
      'Are you sure you want to delete this medicine strength type?',
      undefined,
      (result: boolean) => {
        if (result) {
          this._medicineStrengthTypeService.delete(strengthType.id).subscribe(() => {
            abp.notify.success('Deleted successfully');
            this.list();
          });
        }
      }
    );
  }

  delete(entity: StrengthUnitMasterDto): void {
    this.deleteMedicineStrengthType(entity);
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.list();
  }
}
