import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { DepositTransactionDto, DepositTransactionServiceProxy } from '@shared/service-proxies/service-proxies';
import { ReceiptDetailComponent } from '../receipt-detail/receipt-detail.component';
import { CommonModule } from '@node_modules/@angular/common';

@Component({
  selector: 'app-patient-receipt-list',
  imports: [CommonModule],
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
    if (this.patientDepositId) {
      this._transactionService.getAllByPatientDeposit(this.patientDepositId).subscribe(res => {
     
        this.receipts = res.items ?? [];
        this.cd.detectChanges(); 
      });
    }
  }


  viewReceipt(receipt: DepositTransactionDto): void {
    debugger
    this._modalService.show(ReceiptDetailComponent, {
      class: 'modal-md',
      initialState: {
        receipt: receipt,
        patientName:this.patientName
      }
    });
    this.close();
  }

  close(): void {
    this.bsModalRef.hide();
  }
}
