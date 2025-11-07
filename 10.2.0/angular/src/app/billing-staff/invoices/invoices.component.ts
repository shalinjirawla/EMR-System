import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { CommonModule, DatePipe, NgIf } from '@node_modules/@angular/common';
import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { BillingDto, BillingDtoPagedResultDto, BillingServiceProxy, InvoiceDto, InvoiceDtoPagedResultDto, InvoiceServiceProxy, PatientServiceProxy, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CreateInvoiceComponent } from '../create-invoice/create-invoice.component';
import { ViewInvoiceComponent } from '../view-invoice/view-invoice.component';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { MenuModule } from 'primeng/menu';
import { ViewInsuranceClaimComponent } from '../view-insurance-claim/view-insurance-claim.component';


@Component({
    selector: 'app-invoices',
    imports: [FormsModule, TableModule, CardModule, MenuModule, BreadcrumbModule, InputTextModule, TooltipModule, PrimeTemplate, NgIf,
        PaginatorModule, OverlayPanelModule, ButtonModule, LocalizePipe, DatePipe, CommonModule],
    animations: [appModuleAnimation()],
    templateUrl: './invoices.component.html',
    styleUrl: './invoices.component.css',
    providers: [BillingServiceProxy, UserServiceProxy, InvoiceServiceProxy]
})
export class InvoicesComponent extends PagedListingComponentBase<InvoiceDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    patients: BillingDto[] = [];
    keyword = '';
    isActive: boolean | null;
    items: MenuItem[] | undefined;
    editDeleteMenus: MenuItem[] | undefined;
    selectedRecord: InvoiceDto;
    menuItems: MenuItem[] = [];


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

    ngOnInit(): void {
        this.items = [
            { label: 'Home', routerLink: '/' },
            { label: 'Invoice' },
        ];
    }

    getRowMenuItems(invoice: InvoiceDto): MenuItem[] {
        const menus: MenuItem[] = [
            {
                label: 'View Invoice',
                icon: 'pi pi-eye',
                command: () => this.viewInvoice(invoice)
            }
        ];

        // Show "View Insurance Claim" only if any claim status > 2
        if (invoice.claims && invoice.claims.some(c => c.status >= 2)) {
            menus.push({
                label: 'View Insurance Claim',
                icon: 'pi pi-file',
                command: () => this.viewInsuranceClaim(invoice.id)
            });
        }
        if (invoice.claims && invoice.claims.some(c => c.status >= 2 && c.status < 5)
            && invoice.status == 0 && invoice.coPayAmount != 0) {
            menus.push({
                label: 'Collect Co-Pay Amount',
                icon: 'pi pi-wallet',
                command: () => this.collectCoPayAmount(invoice.id)
            });
        }

        return menus;
    }
    onRowMenuClick(event: Event, record: InvoiceDto, menu: any): void {
        this.selectedRecord = record;
        this.menuItems = this.getRowMenuItems(record); // build current menu dynamically
        menu.toggle(event);
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
        switch (status) {
            case 0: return 'Unpaid';
            case 1: return 'Paid';
            case 2: return 'CoPay Collected';
        }
    }

    getClaimStatusString(status: number): string {
        switch (status) {
            case 0: return 'Pending';
            case 1: return 'Submitted';
            case 2: return 'Partial Approved';
            case 3: return 'Approved';
            case 4: return 'Rejected';
            case 5: return 'Paid';
            default: return '-';
        }
    }

    getClaimStatuses(claims: any[]): string {
        if (!claims || claims.length === 0) return '-';
        return claims.map(c => this.getClaimStatusString(c.status)).join(', ');
    }

    getStatusClass(status: number) {
        switch (status) {
            case 0: return 'text-danger';       // Unpaid → red
            case 1: return 'text-success';      // Paid → green
            case 2: return 'text-warning';      // CoPay Collected → orange
            default: return '';
        }
    }

    getCoPayClass(status: number) {
        if (status === 2) return 'text-warning fw-bold';
        return '';
    }

    getClaimClass(status: number) {
        switch (status) {
            case 0: return 'text-secondary';   // Pending → gray
            case 1: return 'text-info';        // Submitted → blue
            case 2: return 'text-primary';     // Partial Approved → dark blue
            case 3: return 'text-success';     // Approved → green
            case 4: return 'text-danger';      // Rejected → red
            case 5: return 'text-success fw-bold'; // Paid → green bold
            default: return '';
        }
    }
    getPaymentMethodString(method: number): string {
        switch (method) {
            case 0: return 'Cash';
            case 1: return 'Card';
        }
    }

    createInvoice(): void {
        this.showCreateOrEditPrescriptionDialog();
    }

    viewInsuranceClaim(invoiceId: number): void {
        this._modalService.show(ViewInsuranceClaimComponent, {
            class: 'modal-lg',
            initialState: {
                invoiceId: invoiceId
            }
        });
    }

    collectCoPayAmount(id: number): void {
        abp.message.confirm(
            `Are you sure you want to collect Co-Pay Amount from deposit?`,
            'Confirm',
            (result: boolean) => {
                if (result) {
                    this._invoiceService.collectCoPay(id).subscribe(() => {
                        this.notify.success('Co-Pay amount collected from deposit successfully.');
                        this.refresh();
                    });
                }
            }
        );
    }
    viewInvoice(invoice: InvoiceDto): void {
        let viewInvoiceDialog: BsModalRef;

        viewInvoiceDialog = this._modalService.show(ViewInvoiceComponent, {
            class: 'modal-lg',
            initialState: {
                id: invoice.id,   // Pass invoice id to modal
            },
        });
    }
    showCreateOrEditPrescriptionDialog(id?: number): void {
        let createOrEditUserDialog: BsModalRef;
        if (!id) {
            createOrEditUserDialog = this._modalService.show(CreateInvoiceComponent, {
                class: 'modal-lg',
            });
        }

        createOrEditUserDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }

}
