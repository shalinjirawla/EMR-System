import { Component, ViewChild, Injector, OnInit, ChangeDetectorRef } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table, TableRowExpandEvent } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { LabReportTestWithUnitDto, LabReportTypeItemServiceProxy, LabReportsTypeDto, LabReportsTypeDtoPagedResultDto, LabReportsTypeServiceProxy } from 'shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CommonModule, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CreateupdateLabTestItemsComponent } from '../createupdate-lab-test-items/createupdate-lab-test-items.component';

type LabReportsTypeWithTests = Omit<LabReportsTypeDto, 'tests'> & {
  tests?: LabReportTestWithUnitDto[];
};
@Component({
  selector: 'app-lab-test-items',
  imports: [CommonModule,TableModule,ButtonModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe,],
  providers: [LabReportsTypeServiceProxy,LabReportTypeItemServiceProxy],
  standalone: true,
  animations:[appModuleAnimation()],
  templateUrl: './lab-test-items.component.html',
  styleUrl: './lab-test-items.component.css'
})
export class LabTestItemsComponent extends PagedListingComponentBase<LabReportsTypeDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  reportTypes: LabReportsTypeWithTests[] = [];
  expandedRows: { [key: number]: boolean } = {};
  keyword = '';
  isActive: boolean | undefined = undefined;

  constructor(
    injector: Injector,
    private _labReportsTypeService: LabReportsTypeServiceProxy,
    private _labReportTypeItemService: LabReportTypeItemServiceProxy,
    private modalService: BsModalService,
     cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    //this.list();
  }

  // Load parent rows with pagination & filtering
  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._labReportsTypeService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: LabReportsTypeDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
       this.reportTypes = result.items as unknown as LabReportsTypeWithTests[];
        this.cd.detectChanges();
      });
  }
  delete(entity: LabReportsTypeDto): void {
    this.deleteReportType(entity);
  }
  deleteTestItem(reportTypeId: number, testId: number): void {

    abp.message.confirm(
      'Are you sure you want to delete this test from this report type?',
      undefined,
      (res: boolean) => {
        if (res) {
          
          this._labReportTypeItemService.delete(testId).subscribe(() => {
            abp.notify.success('Test removed successfully');
  
            const type = this.reportTypes.find(x => x.id === reportTypeId) as LabReportsTypeWithTests;
  
            if (type) {
              this._labReportTypeItemService.getAllLabReportItems(reportTypeId).subscribe((tests) => {
                type.tests = tests.items;
                this.cd.detectChanges();
              });
            }
          });
        }
      }
    );
  }
  
  
  
  // Handle row expansion
  onRowExpand(event: TableRowExpandEvent) {
    const key = event.data.id;
    this.expandedRows[key] = true;
  
    const type = event.data as LabReportsTypeWithTests;
  
    if (type.id) {
      this._labReportTypeItemService.getAllLabReportItems(type.id).subscribe((tests) => {
        type.tests = tests.items;
        this.cd.detectChanges();
      });
    }
  }
  

  // Handle row collapse: remove from expandedKeys
  onRowCollapse(event: { data: LabReportsTypeDto }) {
    delete this.expandedRows[event.data.id];
  }

  // Create new report type
  createReportType(): void {
    const dlg: BsModalRef = this.modalService.show(
      CreateupdateLabTestItemsComponent,
      { class: 'modal-lg' }
    );
    dlg.content.onSave.subscribe(() => this.list());
  }

  // Edit existing
  editReportType(type: LabReportsTypeDto): void {
    const dlg: BsModalRef = this.modalService.show(
      CreateupdateLabTestItemsComponent,
      { class: 'modal-lg', initialState: { id: type.id } }
    );
    dlg.content.onSave.subscribe(() => this.list());
  }

  // Delete
  deleteReportType(type: LabReportsTypeDto): void {
    abp.message.confirm(
      'Are you sure you want to delete this report type?',
      undefined,
      (res: boolean) => {
        if (res) {
          this._labReportsTypeService.delete(type.id).subscribe(() => {
            abp.notify.success('Deleted successfully');
            this.list();
          });
        }
      }
    );
  }

  // Refresh button
  refresh(): void {
    this.list();
  }

  // Clear filters
  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.list();
  }
}