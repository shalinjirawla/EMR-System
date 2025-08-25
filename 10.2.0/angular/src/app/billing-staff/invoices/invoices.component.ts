import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { DatePipe, NgIf } from '@node_modules/@angular/common';
import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { BillingDto, BillingDtoPagedResultDto, BillingServiceProxy, InvoiceDto, InvoiceDtoPagedResultDto, InvoiceServiceProxy, PatientServiceProxy, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CreateInvoiceComponent } from '../create-invoice/create-invoice.component';



@Component({
  selector: 'app-invoices',
  imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe,DatePipe],
  animations: [appModuleAnimation()],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.css',
   providers: [BillingServiceProxy,UserServiceProxy,InvoiceServiceProxy]
})
export class InvoicesComponent extends PagedListingComponentBase<InvoiceDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    patients: BillingDto[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _invoiceService: InvoiceServiceProxy,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
        //this.processPaymentResult();
    }
//     private processPaymentResult(): void {
//     const params = new URLSearchParams(window.location.search);
//     const paymentStatus = params.get('payment');
//     const invoiceId = params.get('invoiceId');
//     const amountpaid = params.get('amount');



//     if (paymentStatus && invoiceId && amountpaid) {
//         const id = Number(invoiceId);
//         const amount = Number(amountpaid);


//         if (paymentStatus === 'success') {
//             this._invoiceService.markAsPaid(id,amount).subscribe({
//                 next: () => {  // Remove the success parameter
//                     this.notify.success('Payment processed successfully!');
//                     this.refresh(); // Refresh invoice list
//                 },
//                 error: () => this.notify.error('Error verifying payment')
//             });
//         } 
//         else if (paymentStatus === 'canceled') {
//             this.notify.warn('Payment was canceled');
//         }

//         // Clear URL parameters
//         window.history.replaceState({}, document.title, window.location.pathname);
//     }
// }

    clearFilters(): void {
        this.keyword = '';
        this.isActive = undefined;
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

        this._invoiceService
            .getAll(
                this.keyword,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: InvoiceDtoPagedResultDto) => {
                
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }
    delete(invoice: InvoiceDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
            if (result) {
                this._invoiceService.delete(invoice.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }
    // Add these methods to your InvoicesComponent class
getStatusString(status: number): string {
    switch(status) {
        case 0: return 'Unpaid';
        case 1: return 'Paid';
        case 2: return 'Partial Paid';
    }
}

getPaymentMethodString(method: number): string {
    switch(method) {
        case 0: return 'Cash';
        case 1: return 'Card';
    }
}

    createInvoice(): void {
   this.showCreateOrEditPrescriptionDialog();
  }

  showCreateOrEditPrescriptionDialog(id?: number): void {
      let createOrEditUserDialog: BsModalRef;
      if (!id) {
        createOrEditUserDialog = this._modalService.show(CreateInvoiceComponent, {
          class: 'modal-lg',
        });
      }
    //    else {
    //       createOrEditUserDialog = this._modalService.show(CreateAppoinmentComponent, {
    //           class: 'modal-lg',
    //           initialState: {
    //               id: id,
    //           },
    //       });
    //   }
  
      createOrEditUserDialog.content.onSave.subscribe(() => {
        debugger
       this.refresh();
      });
    }

}
