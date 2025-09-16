import { Component, ViewChild, Injector, ChangeDetectorRef, OnInit } from '@angular/core';import { CreateupdateMedicineFormTypeComponent } from '../createupdate-medicine-form-type/createupdate-medicine-form-type.component';
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
import { MedicineFormMasterDto, MedicineFormMasterDtoPagedResultDto, MedicineFormMasterServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-medicine-form-type',
  templateUrl: './medicine-form-type.component.html',
  styleUrl: './medicine-form-type.component.css',
  providers: [MedicineFormMasterServiceProxy],
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
export class MedicineFormTypeComponent extends PagedListingComponentBase<MedicineFormMasterDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  isActive: boolean | undefined = undefined;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _medicineFormTypeService: MedicineFormMasterServiceProxy,
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

    this._medicineFormTypeService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: MedicineFormMasterDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  createMedicineFormType(): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateMedicineFormTypeComponent, {
      class: 'modal-lg',
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  editMedicineFormType(formType: MedicineFormMasterDto): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateMedicineFormTypeComponent, {
      class: 'modal-lg',
      initialState: { id: formType.id },
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  deleteMedicineFormType(formType: MedicineFormMasterDto): void {
    abp.message.confirm(
      'Are you sure you want to delete this medicine form type?',
      undefined,
      (result: boolean) => {
        if (result) {
          this._medicineFormTypeService.delete(formType.id).subscribe(() => {
            abp.notify.success('Deleted successfully');
            this.list();
          });
        }
      }
    );
  }

  delete(entity: MedicineFormMasterDto): void {
    this.deleteMedicineFormType(entity);
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.list();
  }
}
