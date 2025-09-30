import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { EmergencyProcedureStatus, SelectedEmergencyProceduresDto, SelectedEmergencyProceduresServiceProxy } from '@shared/service-proxies/service-proxies';
import { InputTextModule } from 'primeng/inputtext';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
@Component({
  selector: 'app-procedure-requests',
  imports: [
    FormsModule, InputTextModule, BreadcrumbModule,
    TableModule,
    TagModule,
    CommonModule,
    PrimeTemplate,
    OverlayPanelModule,
    ButtonModule,
    NgIf,
    PaginatorModule,
    LocalizePipe,
  ],
  animations: [appModuleAnimation()],
  templateUrl: './procedure-requests.component.html',
  styleUrl: './procedure-requests.component.css',
  providers: [SelectedEmergencyProceduresServiceProxy],
})
export class ProcedureRequestsComponent extends PagedListingComponentBase<SelectedEmergencyProceduresDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  isActive: boolean | null;
  items: MenuItem[] = [];

  procedureStatus = [
    { label: 'Pending', value: EmergencyProcedureStatus._0 },
    { label: 'Completed', value: EmergencyProcedureStatus._1 },
  ];

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _procedureService: SelectedEmergencyProceduresServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: this.l('Procedure-Request') }
    ];
  }


  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }
    this.primengTableHelper.showLoadingIndicator();

    this._procedureService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  delete(entity: SelectedEmergencyProceduresDto): void {
    abp.message.confirm('Are you sure you want to delete this?', undefined, (result: boolean) => {
      if (result) {
        this._procedureService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }

  markAsComplete(id?: number): void {
    if (id > 0) {
      this._procedureService.markAsComplete(id).subscribe(() => {
        abp.notify.success(this.l('MarkedAsCompleted'));
        this.refresh();
      });
    }
  }

  getStatusLabel(value: number): string {
    const status = this.procedureStatus.find((s) => s.value === value);
    return status ? status.label : '';
  }

  getStatusSeverity(value: number) {
    switch (value) {
      case EmergencyProcedureStatus._0:
        return 'badge-soft-warning p-1 rounded'; // Pending
      case EmergencyProcedureStatus._1:
        return 'badge-soft-success p-1 rounded'; // Completed
      default:
        return 'badge-soft-teal p-1 rounded';
    }
  }
}