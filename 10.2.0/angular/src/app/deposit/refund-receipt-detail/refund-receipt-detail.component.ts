import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppSessionService } from '../../../shared/session/app-session.service';
import { DepositTransactionDto } from '../../../shared/service-proxies/service-proxies';
import { CommonModule, formatDate } from '@angular/common';

@Component({
  selector: 'app-refund-receipt-detail',
  imports: [CommonModule],
  templateUrl: './refund-receipt-detail.component.html',
  styleUrl: './refund-receipt-detail.component.css'
})
export class RefundReceiptDetailComponent implements OnInit {
  receipt!: DepositTransactionDto;
  patientName!: string;
  hospitalName = '';

  constructor(
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _appSessionService: AppSessionService

  ) { }

  ngOnInit(): void {
    this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';
    this.cd.detectChanges();
  }
  print(): void {
    const printContents = document.querySelector('.receipt-wrapper')?.innerHTML;
    const styles = document.head.querySelectorAll('style, link[rel="stylesheet"]');
    const win = window.open('', '', 'width=900,height=1100');
    win.document.write(`
      <html>
        <head>
          <title>Print Deposit Receipt</title>
          ${Array.from(styles).map((s) => s.outerHTML).join('')}
          <style>@page { size: A4; margin: 10mm; }</style>
        </head>
        <body>${printContents}</body>
      </html>
    `);
    setTimeout(() => {
      win.document.close();
      win.focus();
      win.print();
      win.close();
    }, 300);
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
