import { Component, ViewChild, Injector, OnInit, ChangeDetectorRef } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import {
  ProcedureReceiptDto,
  ProcedureReceiptDtoPagedResultDto,
  ProcedureReceiptServiceProxy
} from '@shared/service-proxies/service-proxies';
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
import { CommonModule, DatePipe, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ViewProcedureReceiptComponent } from '../view-procedure-receipt/view-procedure-receipt.component';
import { CreateProcedureReceiptComponent } from '../create-procedure-receipt/create-procedure-receipt.component'
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
@Component({
  selector: 'app-procedure-receipt',
  templateUrl: './procedure-receipt.component.html',
  styleUrl: './procedure-receipt.component.css',
  providers: [ProcedureReceiptServiceProxy],
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    FormsModule, BreadcrumbModule, CardModule, InputTextModule, TooltipModule,MenuModule,
    TableModule,
    PaginatorModule,
    ButtonModule,
    NgIf,
    OverlayPanelModule,
    LocalizePipe,
    DatePipe,
    CommonModule
  ]
})
export class ProcedureReceiptComponent extends PagedListingComponentBase<ProcedureReceiptDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  status: string | undefined = undefined;
  selectedReceipt: ProcedureReceiptDto;
  items: MenuItem[] | undefined;
  editDeleteMenus: MenuItem[] | undefined;
  constructor(
    injector: Injector,
    private modalService: BsModalService,
    private receiptService: ProcedureReceiptServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Procedure-receipt' },
    ];
    this.editDeleteMenus = [
      {
        label: 'View Receipt',
        icon: 'pi pi-file',
        command: () => this.viewReceipt(this.selectedReceipt.id)  // call edit
      }
    ];
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this.receiptService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: ProcedureReceiptDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  createReceipt(): void {
    const dialog: BsModalRef = this.modalService.show(
      CreateProcedureReceiptComponent,
      { class: 'modal-lg' }
    );
    dialog.content.onSave.subscribe(() => this.list());
  }

  viewReceipt(id: number): void {
    debugger
    const dialog: BsModalRef = this.modalService.show(
      ViewProcedureReceiptComponent,
      {
        class: 'modal-lg',
        initialState: {
          receiptId: id
        }
      }
    );
  }

  deleteReceipt(receipt: ProcedureReceiptDto): void {
    abp.message.confirm(
      this.l('ReceiptDeleteWarningMessage', receipt.receiptNumber),
      undefined,
      (res: boolean) => {
        if (res) {
          this.receiptService.delete(receipt.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.list();
          });
        }
      }
    );
  }

  delete(entity: ProcedureReceiptDto): void {
    this.deleteReceipt(entity);
  }

  clearFilters(): void {
    this.keyword = '';
    this.status = undefined;
    this.list();
  }
}