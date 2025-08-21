import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { BedDto, BedDtoPagedResultDto, BedServiceProxy, BedStatus } from '@shared/service-proxies/service-proxies';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator } from 'primeng/paginator';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {CreateupdateBedsComponent} from '../createupdate-beds/createupdate-beds.component'
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-beds',
  standalone:true,
  imports: [FormsModule, TableModule, Paginator,TagModule, OverlayPanelModule, NgIf, LocalizePipe,ButtonModule],
  animations: [appModuleAnimation()],
  providers: [BedServiceProxy],
  templateUrl: './beds.component.html',
  styleUrls: ['./beds.component.css']
})
export class BedsComponent extends PagedListingComponentBase<BedDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  beds: BedDto[] = [];
  keyword = '';
  roomTypeId:number|undefined;
  roomId: number | undefined;
  status: BedStatus | undefined;

  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _bedService: BedServiceProxy,
    cd: ChangeDetectorRef,
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  ngOnInit(): void { }

  clearFilters(): void {
    this.keyword = '';
    this.status = undefined;
    this.list();
  }

  onStatusChange() {
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
    this._bedService
      .getAll(
        this.keyword,
        this.roomId,
        this.roomTypeId,
        this.status,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result: BedDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
        this.cd.detectChanges();
      });
  }

  delete(bed: BedDto): void {
    abp.message.confirm(this.l('BedDeleteWarningMessage'), undefined, (result: boolean) => {
      if (result) {
        this._bedService.delete(bed.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }

  AddNewBed(): void {
    this.showCreateOrEditBedDialog();
  }

  EditNewBed(dto: BedDto): void {
    this.showCreateOrEditBedDialog(dto.id);
  }

  showCreateOrEditBedDialog(id?: number): void {
    let CreateOrEditBedDialog: BsModalRef;
    if (!id) {
      CreateOrEditBedDialog = this._modalService.show(CreateupdateBedsComponent, { class: 'modal-lg' });
    } else {
      CreateOrEditBedDialog = this._modalService.show(CreateupdateBedsComponent, {
        class: 'modal-lg',
        initialState: { id: id },
      });
    }

    CreateOrEditBedDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  getStatusLabel(status: BedStatus): string {
    switch (status) {
      case BedStatus._0: return 'Available';
      case BedStatus._1: return 'Occupied';
      case BedStatus._2: return 'Reserved';
      case BedStatus._3: return 'Under Maintenance';
      default: return '';
    }
  }

  getStatusSeverity(status: BedStatus): 'info' | 'warn' | 'success' | 'danger' | 'secondary' {
    switch (status) {
      case BedStatus._0: return 'success';
      case BedStatus._1: return 'danger';
      case BedStatus._2: return 'info';
      case BedStatus._3: return 'secondary';
      default: return 'info';
    }
  }
}
