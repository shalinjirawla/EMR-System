import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import {
  MedicineStockDto,
  MedicineStockDtoPagedResultDto,
  MedicineStockServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { LazyLoadEvent } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { FormsModule } from '@angular/forms';
import { NgIf, DatePipe, CommonModule } from '@angular/common';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
@Component({
  selector: 'app-medicine-stock',
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    LocalizePipe,BreadcrumbModule,TooltipModule,CardModule,InputTextModule,FormsModule,TableModule,CalendarModule,
    CommonModule,NgIf,PaginatorModule,ButtonModule,OverlayPanelModule,MenuModule],
  providers: [MedicineStockServiceProxy],
  templateUrl: './medicine-stock.component.html',
  styleUrl: './medicine-stock.component.css',
})
export class MedicineStockComponent
  extends PagedListingComponentBase<MedicineStockDto>
  implements OnInit {

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  minStock: number | undefined;
  isAvailable: boolean | null;
  items: MenuItem[] | undefined;

  selectedRecord: MedicineStockDto;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _stockService: MedicineStockServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Medicine-Stock' },
    ];
  }

  clearFilters(): void {
    this.keyword = '';
    this.minStock = undefined;
    this.isAvailable = undefined;
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

    this._stockService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: MedicineStockDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  protected delete(entity: MedicineStockDto): void {
    abp.message.confirm(
      'Are you sure you want to delete this?',
      undefined,
      (result: boolean) => {
        if (result) {
          this._stockService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  
}
