import { Component, ViewChild, Injector, OnInit, ChangeDetectorRef } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { TestResultLimitDto, TestResultLimitDtoPagedResultDto, TestResultLimitServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateupdateTestResultLimitComponent } from '../createupdate-test-result-limit/createupdate-test-result-limit.component';
import { CommonModule } from '@node_modules/@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { TableModule } from 'primeng/table';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-test-result-limit',
  imports: [CommonModule,LocalizePipe,TableModule,PaginatorModule,FormsModule, InputTextModule, MenuModule, TooltipModule, CardModule, TagModule,BreadcrumbModule],
  providers:[TestResultLimitServiceProxy],
  animations: [appModuleAnimation()],
  templateUrl: './test-result-limit.component.html',
  styleUrl: './test-result-limit.component.css'
})
export class TestResultLimitComponent extends PagedListingComponentBase<TestResultLimitDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  selectedRecord: TestResultLimitDto;
  editDeleteMenus: MenuItem[];
  items: MenuItem[];

  constructor(
    injector: Injector,
    private modalService: BsModalService,
    private testLimitService: TestResultLimitServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

 ngOnInit(): void {
    this.items = [{ label: 'Home', routerLink: '/' }, { label: 'Test Result Limits' }];
    this.editDeleteMenus = [
      { label: 'Edit', icon: 'pi pi-pencil', command: () => this.edit(this.selectedRecord) },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.delete(this.selectedRecord) }
    ];
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this.testLimitService
      .getAll(
        this.keyword,
        //this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: TestResultLimitDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  create(): void {
    const dialog: BsModalRef = this.modalService.show(CreateupdateTestResultLimitComponent, {
      class: 'modal-lg',
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  edit(testLimit: TestResultLimitDto): void {
    const dialog: BsModalRef = this.modalService.show(CreateupdateTestResultLimitComponent, {
      class: 'modal-lg',
      initialState: { id: testLimit.id },
    });
    dialog.content.onSave.subscribe(() => this.list());
  }

  delete(testLimit: TestResultLimitDto): void {
    abp.message.confirm('Are you sure you want to delete this limit?', undefined, (res: boolean) => {
      if (res) {
        this.testLimitService.delete(testLimit.id).subscribe(() => {
          abp.notify.success('Deleted successfully');
          this.list();
        });
      }
    });
  }

  deleteEntity(entity: TestResultLimitDto): void {
    this.delete(entity);
  }

  clearFilters(): void {
    this.keyword = '';
    this.list();
  }
}