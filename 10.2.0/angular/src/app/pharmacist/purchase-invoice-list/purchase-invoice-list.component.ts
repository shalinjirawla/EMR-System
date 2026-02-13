import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import {
  PurchaseInvoiceDto,
  PurchaseInvoiceDtoPagedResultDto,
  PurchaseInvoiceServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { LazyLoadEvent } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { CreatePurchaseInvoiceComponent } from '../create-purchase-invoice/create-purchase-invoice.component';
import { EditPurchaseInvoiceComponent } from '../edit-purchase-invoice/edit-purchase-invoice.component';
import { ViewPurchaseInvoiceComponent } from '../view-purchase-invoice/view-purchase-invoice.component';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { FormsModule } from '@angular/forms';
import { NgIf, DatePipe, DecimalPipe, CommonModule } from '@angular/common';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { MenuModule } from 'primeng/menu';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
@Component({
  selector: 'app-purchase-invoice-list',
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [LocalizePipe,TooltipModule,CardModule,InputTextModule,FormsModule,BreadcrumbModule,TableModule,CalendarModule,NgIf,PaginatorModule,ButtonModule,DatePipe,
    DecimalPipe,CommonModule,OverlayPanelModule,MenuModule,],
  providers: [PurchaseInvoiceServiceProxy],
  templateUrl: './purchase-invoice-list.component.html',
  styleUrl: './purchase-invoice-list.component.css',
})
export class PurchaseInvoiceListComponent
  extends PagedListingComponentBase<PurchaseInvoiceDto>
  implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  supplierName = '';
  invoiceDate: Date | null = null;
  items: MenuItem[] | undefined;
  editDeleteMenus: MenuItem[] | undefined;
  selectedRecord:PurchaseInvoiceDto

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _purchaseInvoiceService: PurchaseInvoiceServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Medicine-Purchase' },
    ];
    this.editDeleteMenus = [
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => this.editInvoice(this.selectedRecord)  // call edit
      },
      {
        label: 'View',
        icon: 'pi pi-eye',
        command: () => this.viewInvoice(this.selectedRecord)  // call edit
      }
    ];
  }

  clearFilters(): void {
    this.keyword = '';
    this.supplierName = '';
    this.invoiceDate = null;
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

    this._purchaseInvoiceService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: PurchaseInvoiceDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  protected delete(entity: PurchaseInvoiceDto): void {
    abp.message.confirm(
      "Are you sure you want to delete this invoice?",
      undefined,
      (result: boolean) => {
        if (result) {
          this._purchaseInvoiceService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  addNewInvoice(): void {
    this.showCreateOrEditInvoice();
  }

  editInvoice(dto: PurchaseInvoiceDto): void {
    this.showCreateOrEditInvoice(dto.id);
  }
  viewInvoice(dto: PurchaseInvoiceDto): void {
    const dialog: BsModalRef = this._modalService.show(ViewPurchaseInvoiceComponent, {
      class: 'modal-xl',
      initialState: { invoiceId: dto.id }
    });
  }

  showCreateOrEditInvoice(id?: number): void {
    let dialog: BsModalRef;
    if (!id) {
      dialog = this._modalService.show(CreatePurchaseInvoiceComponent, { class: 'modal-xl' });
    } else {
      dialog = this._modalService.show(EditPurchaseInvoiceComponent, {
        class: 'modal-xl',
        initialState: { invoiceId: id },
      });
    }

    dialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}
