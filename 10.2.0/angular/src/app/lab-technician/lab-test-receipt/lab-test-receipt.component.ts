// lab-test-receipt.component.ts
import { Component, ViewChild, Injector, OnInit, ChangeDetectorRef } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { 
  LabTestReceiptDto, 
  LabTestReceiptDtoPagedResultDto, 
  LabTestReceiptServiceProxy 
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
import { CreateLabReportComponent } from '../create-lab-report/create-lab-report.component';
import {ViewReceiptComponent} from '../view-receipt/view-receipt.component'
import { OverlayPanelModule } from 'primeng/overlaypanel';

@Component({
  selector: 'app-lab-test-receipt',
  templateUrl: './lab-test-receipt.component.html',
  styleUrls: ['./lab-test-receipt.component.css'],
  providers: [LabTestReceiptServiceProxy],
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    FormsModule,
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
export class LabTestReceiptComponent extends PagedListingComponentBase<LabTestReceiptDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  
  keyword = '';
  status: string | undefined = undefined;
  advancedFiltersVisible = false;
  selectedReceipt: LabTestReceiptDto | null = null;
  constructor(
    injector: Injector,
    private modalService: BsModalService,
    private receiptService: LabTestReceiptServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {}

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records.length) return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this.receiptService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: LabTestReceiptDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  createReceipt(): void {
    const dialog: BsModalRef = this.modalService.show(
      CreateLabReportComponent,
      { class: 'modal-lg' }
    );
    dialog.content.onSave.subscribe(() => this.list());
  }

  viewReceipt(id: number): void {
   const dialog: BsModalRef = this.modalService.show(
    ViewReceiptComponent,
    {
      class: 'modal-lg',
      initialState: {
        labReceiptId: id
      }
    }
  );
}

ViewLabReport(id: number): void {
  // Example navigation or modal open
  // this.modalService.show(ViewLabReportComponent, { class: 'modal-lg', initialState: { reportId: id } });
}

  deleteReceipt(receipt: LabTestReceiptDto): void {
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

  delete(entity: LabTestReceiptDto): void {
    this.deleteReceipt(entity);
  }

  clearFilters(): void {
    this.keyword = '';
    this.status = undefined;
    this.list();
  }
}