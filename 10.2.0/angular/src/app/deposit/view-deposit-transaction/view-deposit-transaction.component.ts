import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { DepositTransactionDto, DepositTransactionServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { TableModule } from "primeng/table";
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { CommonModule } from '@node_modules/@angular/common';
import { TagModule } from 'primeng/tag';
@Component({
  selector: 'app-view-deposit-transaction',
  imports: [TableModule,AbpModalHeaderComponent,CommonModule,TagModule],
  animations: [appModuleAnimation()],
  providers:[DepositTransactionServiceProxy],
  templateUrl: './view-deposit-transaction.component.html',
  styleUrl: './view-deposit-transaction.component.css'
})
export class ViewDepositTransactionComponent implements OnInit {
  depositId: number;
  patientName: string;

  transactions: DepositTransactionDto[] = [];
  loading = false;

  constructor(
    injector: Injector,
    private _modalRef: BsModalRef,
    private _depositService: DepositTransactionServiceProxy,
    public cd:ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.loading = true;
    this._depositService
      .getAllByPatientDepositTransaction(this.depositId) // ✅ ye backend method call karega
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((res) => {
        this.transactions = res.items || [];
        this.cd.detectChanges();
      });
  }

  close(): void {
    this._modalRef.hide();
  }

  getTransactionTypeLabel(type: number): string {
    return type === 0 ? 'Credit' : 'Debit';
  }
  get totalCredit(): number {
  return this.transactions
    .filter(t => t.transactionType === 0) // credit
    .reduce((sum, t) => sum + t.amount, 0);
}

get totalDebit(): number {
  return this.transactions
    .filter(t => t.transactionType === 1) // debit
    .reduce((sum, t) => sum + t.amount, 0);
}

get netBalance(): number {
  return this.totalCredit - this.totalDebit;
}

}