import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ButtonModule } from 'primeng/button';
import { DepartmentDto, DepartmentDtoPagedResultDto, DepartmentServiceProxy } from '@shared/service-proxies/service-proxies';
import {CreateupdateDepartmentComponent} from '../createupdate-department/createupdate-department.component'
@Component({
  selector: 'app-department',
 imports: [
    FormsModule,
    TableModule,
    ButtonModule,
    PrimeTemplate,
    NgIf,
    PaginatorModule,
    LocalizePipe
  ],
  providers: [DepartmentServiceProxy],
  standalone: true,
  animations: [appModuleAnimation()],
  templateUrl: './department.component.html',
  styleUrl: './department.component.css'
})
export class DepartmentComponent extends PagedListingComponentBase<DepartmentDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  departments: DepartmentDto[] = [];

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _departmentService: DepartmentServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {}

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();

    this._departmentService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: DepartmentDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  createDepartment(): void {
    let createDialog: BsModalRef = this._modalService.show(CreateupdateDepartmentComponent, { class: 'modal-lg' });
    createDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  editDepartment(department: DepartmentDto): void {
    let editDialog: BsModalRef = this._modalService.show(CreateupdateDepartmentComponent, { class: 'modal-lg', initialState: { id: department.id } });
    editDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  deleteDepartment(department: DepartmentDto): void {
    abp.message.confirm('Are you sure you want to delete this department?', undefined, (result: boolean) => {
      if (result) {
        this._departmentService.delete(department.id).subscribe(() => {
          abp.notify.success('Deleted successfully');
          this.refresh();
        });
      }
    });
  }

  delete(entity: DepartmentDto): void {
    this.deleteDepartment(entity);
  }

  clearFilters(): void {
    this.list();
  }
}