import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { CommonModule, NgIf } from '@node_modules/@angular/common';
import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { CreatePrescriptionLabTestsServiceProxy, LabRequestListDto, LabTestReceiptServiceProxy, LabTestStatus, PaymentMethod, PrescriptionLabTestDto, PrescriptionLabTestServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ChipModule } from 'primeng/chip';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { EditLabReportComponent } from '@app/lab-technician/edit-lab-report/edit-lab-report.component';
import { GenerateLabReportComponent } from '@app/lab-technician/generate-lab-report/generate-lab-report.component';
import { ViewLabReportComponent } from '@app/lab-technician/view-lab-report/view-lab-report.component';
import { TagModule } from 'primeng/tag';
import { CreateLabReportComponent } from '../create-lab-report/create-lab-report.component';
import { LabTestReceiptComponent } from '../lab-test-receipt/lab-test-receipt.component'

@Component({
    selector: 'app-test-requests',
    imports: [FormsModule, TableModule, TagModule, CommonModule, PrimeTemplate, OverlayPanelModule, MenuModule, ButtonModule, NgIf, PaginatorModule, ChipModule, LocalizePipe],
    animations: [appModuleAnimation()],
    templateUrl: './test-requests.component.html',
    styleUrl: './test-requests.component.css',
    providers: [PrescriptionLabTestServiceProxy, CreatePrescriptionLabTestsServiceProxy, LabTestReceiptServiceProxy]
})
export class TestRequestsComponent extends PagedListingComponentBase<PrescriptionLabTestDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    PaymentMethod = PaymentMethod;
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;
    testStatus = [
        { label: 'Pending', value: LabTestStatus._0 },
        { label: 'In Progress', value: LabTestStatus._1 },
        { label: 'Completed', value: LabTestStatus._2 },
    ];
    PAYMENT_METHOD_NAMES: { [key in PaymentMethod]: string } = {
        [PaymentMethod._0]: 'Cash',
        [PaymentMethod._1]: 'Card'
    };
    PaymentMethodNames = this.PAYMENT_METHOD_NAMES;
    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _prescriptionLabTests: PrescriptionLabTestServiceProxy,
        private _createprescriptionLabTest: CreatePrescriptionLabTestsServiceProxy,
        private _labtestrecipt: LabTestReceiptServiceProxy,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
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

        this._prescriptionLabTests
            .getAllLabTestRequests(
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(finalize(() => {
                this.primengTableHelper.hideLoadingIndicator();
            }))
            .subscribe((result) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.cd.detectChanges();
            });
    }
    delete(entity: PrescriptionLabTestDto): void {
        abp.message.confirm("Are you sure u want to delete this", undefined, (result: boolean) => {
            if (result) {
                this._prescriptionLabTests.delete(entity.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }

    // makePayment(record: LabRequestListDto): void {
    //     if (!record.id) {
    //         abp.message.warn('Invalid lab test record.');
    //         return;
    //     }

    //     abp.ui.setBusy();
    //     this._createprescriptionLabTest
    //         .initiatePaymentForLabTest(record.id)
    //         .pipe(finalize(() => abp.ui.clearBusy()))
    //         .subscribe({
    //             next: (sessionUrl: string) => {
    //                 // Redirect user to Stripe
    //                 window.location.href = sessionUrl;
    //             },
    //             error: (err) => {
    //                 abp.message.error(err.message || 'Payment initiation failed.');
    //             }
    //         });
    // }
    // pay(record: PrescriptionLabTestDto, method: PaymentMethod): void {
    //     abp.ui.setBusy();

    //     if (method === PaymentMethod._1) {
    //         // Card → existing Stripe flow
    //         this._createprescriptionLabTest
    //             .initiatePaymentForLabTest(record.id)
    //             .pipe(finalize(() => abp.ui.clearBusy()))
    //             .subscribe(
    //                 (sessionUrl: string) => window.location.href = sessionUrl,
    //                 (err: any) => abp.message.error(err.message || 'Payment initiation failed.')
    //             );
    //     } else {
    //         // Cash → generate receipt immediately
    //         const methodName = this.PAYMENT_METHOD_NAMES[method]; // "Cash"
    //         this._labtestrecipt
    //             .generateLabTestReceipt(record.id, methodName)
    //             .pipe(finalize(() => abp.ui.clearBusy()))
    //             .subscribe();
    //     }
    // }

    viewReceipt(prescriptionLabTestId: number): void {
        if (prescriptionLabTestId) {
            const modalRef: BsModalRef = this._modalService.show(
                LabTestReceiptComponent,
                {
                    class: 'modal-lg',
                    initialState: {
                        //prescriptionLabTestId: prescriptionLabTestId
                    }
                }
            );
        }
    }
    getStatusLabel(value: number): string {
        const status = this.testStatus.find(s => s.value === value);
        return status ? status.label : '';
    }
    getStatusClass(value: number): string {
        switch (value) {
            case LabTestStatus._0: return 'status-pending';    // Scheduled
            case LabTestStatus._1: return 'status-in-progress';    // Checked In
            case LabTestStatus._2: return 'status-completed';    // Completed
            default: return '';
        }
    }
    getStatusSeverity(value: number): 'info' | 'warn' | 'success' | 'danger' | 'secondary' | 'contrast' {
        switch (value) {
            case LabTestStatus._0: return 'info';        // Pending
            case LabTestStatus._1: return 'secondary';   // In Progress
            case LabTestStatus._2: return 'success';     // Completed
            default: return 'contrast';
        }
    }

    createLabReport(): void {
        const createDialog: BsModalRef = this._modalService.show(CreateLabReportComponent, {
            class: 'modal-lg',   // or modal-lg/modal-md as you desire
            initialState: {
                // You can pass data if needed here
            }
        });

        // Optional: refresh the grid on save
        createDialog.content.onSave?.subscribe(() => {
            this.refresh();
        });
    }
    CreateReport(record: LabRequestListDto): void {
        let createReportDialog: BsModalRef;
        if (record.labReportsTypeId) {
            createReportDialog = this._modalService.show(GenerateLabReportComponent, {
                class: 'modal-xl',
                initialState: {
                    id: record.id,
                    labReportsTypeId: record.labReportsTypeId,
                    testName: record.labReportTypeName,
                    patientName: record.patientName,
                    emergencyCaseId: record.emergencyCaseId,
                    isEmergencyCase: record.isEmergencyPrescription,  
                },
            });
        }
        createReportDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
    EditReport(record: LabRequestListDto): void {
        let editReportDialog: BsModalRef;
        if (record.id) {
            editReportDialog = this._modalService.show(EditLabReportComponent, {
                class: 'modal-xl',
                initialState: {
                    id: record.id,
                    testName: record.labReportTypeName,
                    patientName: record.patientName,
                },
            });
        }
        editReportDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
    ViewLabReport(id?: number) {
        let viewReportDialog: BsModalRef;
        viewReportDialog = this._modalService.show(ViewLabReportComponent, {
            class: 'modal-lg',
            initialState: {
                id: id,
            },
        });
        // editReportDialog.content.onSave.subscribe(() => {
        //     this.refresh();
        // });
    }
    CompleteReport(id?: number) {
        if (id > 0) {
            this._prescriptionLabTests.makeCompleteReport(id).subscribe(res => {
                this.refresh();
            })
        }
    }
}
