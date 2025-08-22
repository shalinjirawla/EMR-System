import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { DepositTransactionDto } from '@shared/service-proxies/service-proxies';
import { CommonModule, formatDate } from '@node_modules/@angular/common';
import { AppSessionService } from '@shared/session/app-session.service';

@Component({
  selector: 'app-receipt-detail',
  imports: [CommonModule],
  templateUrl: './receipt-detail.component.html',
  styleUrl: './receipt-detail.component.css'
})
export class ReceiptDetailComponent implements OnInit {
  receipt!: DepositTransactionDto;
  patientName!: string;
  hospitalName = '';

  constructor(
    public bsModalRef: BsModalRef,
     private cd: ChangeDetectorRef,
     private _appSessionService: AppSessionService

  ) {}

  ngOnInit(): void {
     this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';
    this.cd.detectChanges();
  }
print(): void {
  window.print();
}
  close(): void {
    this.bsModalRef.hide();
  }

  format(dt: string | Date | null): string {
    if (!dt) return '';
    try {
      return formatDate(dt, 'dd-MMM-yyyy hh:mm a', 'en-IN');
    } catch {
      return String(dt);
    }
  }
}
