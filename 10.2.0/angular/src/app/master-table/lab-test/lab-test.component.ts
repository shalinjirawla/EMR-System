import { Component, ViewChild, Injector, OnInit, ChangeDetectorRef } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { LabTestDto, LabTestDtoPagedResultDto, LabTestServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { ButtonModule } from 'primeng/button';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateupdateLabTestComponent } from '../createupdate-lab-test/createupdate-lab-test.component';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { OverlayPanelModule } from "primeng/overlaypanel";
@Component({
  selector: 'app-lab-test',
  templateUrl: './lab-test.component.html',
  styleUrl: './lab-test.component.css',
  providers: [LabTestServiceProxy],
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    FormsModule,SelectModule, InputTextModule, CheckboxModule, MenuModule, TooltipModule, CardModule, TagModule,
    TableModule,BreadcrumbModule,OverlayPanelModule,
    PaginatorModule,
    ButtonModule,
    NgIf,
    LocalizePipe
  ]
})
export class LabTestComponent extends PagedListingComponentBase<LabTestDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  isActive: boolean | undefined = undefined;
  advancedFiltersVisible = false;
  selectedRecord: LabTestDto;
  editDeleteMenus: MenuItem[];
  items: MenuItem[];
  activeFilter = { all: true, isActive: false, isNotActive: false };

  constructor(
    injector: Injector,
    private modalService: BsModalService,
    private labTestService: LabTestServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [{ label: 'Home', routerLink: '/' }, { label: 'Lab Tests' }];
    this.editDeleteMenus = [
      { label: 'Edit', icon: 'pi pi-pencil', command: () => this.editLabTest(this.selectedRecord) },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.deleteLabTest(this.selectedRecord) }
    ];
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this.labTestService
      .getAll(
        this.keyword,
        this.isActive,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: LabTestDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  createLabTest(): void {
    const dialog: BsModalRef = this.modalService.show(
      // Replace with your actual component
      CreateupdateLabTestComponent,
      { class: 'modal-lg' }
    );
    dialog.content.onSave.subscribe(() => this.list());
  }

  editLabTest(test: LabTestDto): void {
    const dialog: BsModalRef = this.modalService.show(
      // Replace with your actual component
      CreateupdateLabTestComponent,
      { class: 'modal-lg', initialState: { id: test.id } }
    );
    dialog.content.onSave.subscribe(() => this.list());
  }

  deleteLabTest(test: LabTestDto): void {
    abp.message.confirm('Are you sure you want to delete this test?', undefined, (res: boolean) => {
      if (res) {
        this.labTestService.delete(test.id).subscribe(() => {
          abp.notify.success('Deleted successfully');
          this.list();
        });
      }
    });
  }

  delete(entity: LabTestDto): void {
    this.deleteLabTest(entity);
  }

  onFilterChange(selected: 'all' | 'active' | 'notActive', event: any) {
    if (selected === 'all') {
      this.activeFilter = { all: true, isActive: false, isNotActive: false };
      this.isActive = undefined;
    } else if (selected === 'active') {
      this.activeFilter = { all: false, isActive: true, isNotActive: false };
      this.isActive = event.checked ? true : undefined;
    } else if (selected === 'notActive') {
      this.activeFilter = { all: false, isActive: false, isNotActive: true };
      this.isActive = event.checked ? false : undefined;
    }
    this.list();
    this.cd.detectChanges();
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.activeFilter = { all: true, isActive: false, isNotActive: false };
    this.list();
  }
}
