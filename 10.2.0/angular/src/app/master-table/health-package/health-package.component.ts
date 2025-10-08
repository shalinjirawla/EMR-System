import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { HealthPackageServiceProxy, HealthPackageDto, HealthPackageDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { CreateupdateHealthPackageComponent } from '../createupdate-health-package/createupdate-health-package.component';
import { FormsModule } from '@node_modules/@angular/forms';
import { CommonModule } from '@node_modules/@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-health-package',
  standalone: true,
  imports: [FormsModule, CommonModule,BreadcrumbModule,TooltipModule, CardModule,MenuModule, TableModule,
     PaginatorModule, LocalizePipe],
  animations: [appModuleAnimation()],
  providers: [HealthPackageServiceProxy],
  templateUrl: './health-package.component.html',
  styleUrl: './health-package.component.css'
})
export class HealthPackageComponent extends PagedListingComponentBase<HealthPackageDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  keyword = '';
   editDeleteMenus: MenuItem[];
   items: MenuItem[];
   isActive: boolean | undefined = undefined;
   selectedRecord:HealthPackageDto;
  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _healthPackageService: HealthPackageServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void { 
   this.items = [{ label: 'Home', routerLink: '/' }, { label: 'Health Packages' }];
   this.editDeleteMenus = [
      { label: 'Edit', icon: 'pi pi-pencil', command: () => this.editHealthPackage(this.selectedRecord) },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.deleteHealthPackage(this.selectedRecord) }
    ];

  }



  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }
    this.primengTableHelper.showLoadingIndicator();
    this._healthPackageService
      .getAll(
        this.keyword,
        this.isActive,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(
        finalize(() => {
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe((result: HealthPackageDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
        this.cd.detectChanges();
      });
  }

  createHealthPackage(): void {
    const createDialog: BsModalRef = this._modalService.show(CreateupdateHealthPackageComponent, { class: 'modal-lg' });
    createDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  editHealthPackage(entity: HealthPackageDto): void {
    const editDialog: BsModalRef = this._modalService.show(CreateupdateHealthPackageComponent, { class: 'modal-lg', initialState: { id: entity.id } });
    editDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
  delete(entity: HealthPackageDto): void {
    this.deleteHealthPackage(entity);
  }

  deleteHealthPackage(entity: HealthPackageDto): void {
    abp.message.confirm('Are you sure you want to delete this package?', undefined, (result: boolean) => {
      if (result) {
        this._healthPackageService.delete(entity.id).subscribe(() => {
          abp.notify.success('Deleted successfully');
          this.refresh();
        });
      }
    });
  }
}