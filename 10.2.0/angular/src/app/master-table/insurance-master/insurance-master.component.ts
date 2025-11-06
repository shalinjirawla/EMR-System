import { Component, ViewChild, Injector, ChangeDetectorRef, OnInit } from '@angular/core';
import { CreateupdateInsuranceMasterComponent } from '../createupdate-insurance-master/createupdate-insurance-master.component';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent, PrimeTemplate, MenuItem } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { InsuranceMasterDto, InsuranceMasterDtoPagedResultDto, InsuranceMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { MenuModule } from 'primeng/menu';
import { OverlayPanelModule } from "primeng/overlaypanel";
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';

@Component({
  selector: 'app-insurance-master',
   providers: [InsuranceMasterServiceProxy],
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    FormsModule, BreadcrumbModule, InputTextModule, TooltipModule, CardModule, MenuModule,
    TableModule, OverlayPanelModule, CheckboxModule, PaginatorModule, ButtonModule,
    PrimeTemplate, NgIf, LocalizePipe
  ],
  templateUrl: './insurance-master.component.html',
  styleUrl: './insurance-master.component.css'
})
export class InsuranceMasterComponent extends PagedListingComponentBase<InsuranceMasterDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  isActive: boolean | undefined = undefined;
  activeFilter = { all: true, isActive: false, isNotActive: false };
  selectedRecord: InsuranceMasterDto;
  items: MenuItem[];
  editDeleteMenus: MenuItem[];

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _insuranceService: InsuranceMasterServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Insurance Masters' }
    ];

    this.editDeleteMenus = [
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          if (this.selectedRecord) {
            this.editInsuranceMaster(this.selectedRecord);
          }
        }
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
          if (this.selectedRecord) {
            this.deleteInsuranceMaster(this.selectedRecord);
          }
        }
      }
    ];
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._insuranceService
      .getAll(
        this.keyword,
        this.isActive,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: InsuranceMasterDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  delete(entity: InsuranceMasterDto): void {
      this.deleteInsuranceMaster(entity);
    }
  createInsuranceMaster(): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateInsuranceMasterComponent, { class: 'modal-md' });
    dialog.content.onSave.subscribe(() => this.list());
  }

  editInsuranceMaster(record: InsuranceMasterDto): void {
    const dialog: BsModalRef = this._modalService.show(CreateupdateInsuranceMasterComponent, {
      class: 'modal-md',
      initialState: { id: record.id },
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  deleteInsuranceMaster(record: InsuranceMasterDto): void {
    abp.message.confirm(
      'Are you sure you want to delete this insurance master?',
      undefined,
      (result: boolean) => {
        if (result) {
          this._insuranceService.delete(record.id).subscribe(() => {
            abp.notify.success('Deleted successfully');
            this.refresh();
          });
        }
      }
    );
  }

  onFilterChange(selected: 'all' | 'active' | 'notActive', event: any) {
    if (selected === 'all') {
      if (event.checked) { this.activeFilter = { all: true, isActive: false, isNotActive: false }; this.isActive = undefined; }
      else { this.activeFilter.all = true; this.isActive = undefined; }
    } else if (selected === 'active') {
      if (event.checked) { this.activeFilter = { all: false, isActive: true, isNotActive: false }; this.isActive = true; }
      else { this.activeFilter.all = true; this.isActive = undefined; }
    } else if (selected === 'notActive') {
      if (event.checked) { this.activeFilter = { all: false, isActive: false, isNotActive: true }; this.isActive = false; }
      else { this.activeFilter.all = true; this.isActive = undefined; }
    }
    this.list();
    this.cd.detectChanges();
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.list();
  }
}
