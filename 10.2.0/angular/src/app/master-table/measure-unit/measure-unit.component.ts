import { Component, ViewChild } from '@angular/core';
import { MeasureUnitDto, MeasureUnitDtoPagedResultDto, MeasureUnitServiceProxy } from '@shared/service-proxies/service-proxies';
import{CreateupdateMeasureUnitComponent} from '../createupdate-measure-unit/createupdate-measure-unit.component'
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Injector, OnInit, ChangeDetectorRef } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
@Component({
  selector: 'app-measure-unit',
  templateUrl: './measure-unit.component.html',
  styleUrl: './measure-unit.component.css',
  providers: [MeasureUnitServiceProxy],
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    FormsModule,
    TableModule,
    PaginatorModule,
    ButtonModule,
    PrimeTemplate,
    NgIf,
    LocalizePipe
  ],
})
  export class MeasureUnitsComponent extends PagedListingComponentBase<MeasureUnitDto>implements OnInit
{
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _measureUnitService: MeasureUnitServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {}

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();
    this._measureUnitService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: MeasureUnitDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
        this.cd.detectChanges();
      });
  }

  createMeasureUnit(): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateMeasureUnitComponent, {
      class: 'modal-lg',
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  editMeasureUnit(unit: MeasureUnitDto): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateMeasureUnitComponent, {
      class: 'modal-lg',
      initialState: { id: unit.id },
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  deleteMeasureUnit(unit: MeasureUnitDto): void {
    abp.message.confirm(
      'Are you sure you want to delete this unit?',
      undefined,
      (result: boolean) => {
        if (result) {
          this._measureUnitService.delete(unit.id).subscribe(() => {
            abp.notify.success('Deleted successfully');
            this.list();
          });
        }
      }
    );
  }

  delete(entity: MeasureUnitDto): void {
    this.deleteMeasureUnit(entity);
  }

  clearFilters(): void {
    this.list();
  }
}
