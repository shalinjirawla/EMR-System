import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe, NgIf } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { LabReportsTypeDto, LabReportsTypeDtoPagedResultDto, LabReportsTypeServiceProxy } from '@shared/service-proxies/service-proxies';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateupdateLabReportTypeComponent } from '../createupdate-lab-report-type/createupdate-lab-report-type.component';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { InputTextModule } from 'primeng/inputtext';
@Component({
  selector: 'app-lab-report-type',
  imports: [FormsModule, TableModule, ButtonModule,BreadcrumbModule,TooltipModule, CardModule,MenuModule,InputTextModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe,CommonModule],
  animations:[appModuleAnimation()],
  templateUrl: './lab-report-type.component.html',
  styleUrl: './lab-report-type.component.css',
  providers: [LabReportsTypeServiceProxy],
  standalone: true,
})
export class LabReportTypeComponent extends PagedListingComponentBase<LabReportsTypeDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
   
    selectedRecord: LabReportsTypeDto;
  editDeleteMenus: MenuItem[];
  items: MenuItem[];

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _labReportsTypeService: LabReportsTypeServiceProxy,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
    }

    
  ngOnInit(): void {
    this.items = [{ label: 'Home', routerLink: '/' }, { label: 'Lab Report Type' }];
    this.editDeleteMenus = [
      { label: 'Edit', icon: 'pi pi-pencil', command: () => this.editLabReportType(this.selectedRecord) },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.deleteLabReportType(this.selectedRecord) }
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
        this._labReportsTypeService
            .getAll(
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: LabReportsTypeDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    createLabReportType(): void {
      let createDialog: BsModalRef = this._modalService.show(CreateupdateLabReportTypeComponent, { class: 'modal-lg' });
      createDialog.content.onSave.subscribe(() => {
        this.refresh();
      });
    }

    editLabReportType(facility: LabReportsTypeDto): void {
      let editDialog: BsModalRef = this._modalService.show(CreateupdateLabReportTypeComponent, { class: 'modal-lg', initialState: { id: facility.id } });
      editDialog.content.onSave.subscribe(() => {
        this.refresh();
      });
    }

    deleteLabReportType(facility: LabReportsTypeDto): void {
        abp.message.confirm('Are you sure you want to delete this facility?', undefined, (result: boolean) => {
            if (result) {
                this._labReportsTypeService.delete(facility.id).subscribe(() => {
                    abp.notify.success('Deleted successfully');
                    this.refresh();
                });
            }
        });
    }

    delete(entity: LabReportsTypeDto): void {
        this.deleteLabReportType(entity);
    }

    clearFilters(): void {
        this.list();
    }
}
