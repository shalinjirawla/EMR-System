import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { DepositTransactionDto, DepositTransactionServiceProxy } from '@shared/service-proxies/service-proxies';
import { ReceiptDetailComponent } from '../receipt-detail/receipt-detail.component';
import { CommonModule } from '@node_modules/@angular/common';
import { TooltipModule } from 'primeng/tooltip';
import Swal from 'sweetalert2';
import {RefundReceiptDetailComponent} from '../refund-receipt-detail/refund-receipt-detail.component'
@Component({
  selector: 'app-patient-receipt-list',
  imports: [CommonModule,TooltipModule],
  standalone: true,
  providers: [DepositTransactionServiceProxy],
  templateUrl: './patient-receipt-list.component.html',
  styleUrl: './patient-receipt-list.component.css'
})
export class PatientReceiptListComponent implements OnInit {
  patientDepositId: number;
  patientName: string;

  receipts: DepositTransactionDto[] = [];

  constructor(
    public bsModalRef: BsModalRef,
    private _transactionService: DepositTransactionServiceProxy,
    private _modalService: BsModalService,
    public cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.reloadReceipts()
  }


  viewReceipt(receipt: DepositTransactionDto): void {
    this._modalService.show(ReceiptDetailComponent, {
      class: 'modal-md',
      initialState: {
        receipt: receipt,
        patientName: this.patientName
      }
    });
    this.close();
  }

  close(): void {
    this.bsModalRef.hide();
  }
  refund(receipt: DepositTransactionDto) {
  Swal.fire({
    title: 'Refund Confirmation',
    text: `Are you sure you want to refund ₹${receipt.remainingAmount}?`,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Yes, Refund',
    cancelButtonText: 'Cancel'
  }).then(result => {
    if (result.isConfirmed) {
      this._transactionService.createDepositRefund(receipt.id)
      .subscribe({
        next: () => {
          Swal.fire('Success', 'Refund processed successfully', 'success');
          this.reloadReceipts();
        },
        error: (err) => {
          Swal.fire('Error', err.error?.error?.message ?? 'Refund failed', 'error');
        }
      });
    }
  });
}
reloadReceipts() {
  this._transactionService.getAllByPatientDeposit(this.patientDepositId)
    .subscribe(res => {
      this.receipts = res.items ?? [];
      this.cd.detectChanges();
    });
}

  viewRefundReceipt(receipt: DepositTransactionDto) {
    this._modalService.show(RefundReceiptDetailComponent, {
      class: 'modal-md',
      initialState: {
        receipt: receipt,
        patientName: this.patientName
      }
    });
    this.close();
  }

}
