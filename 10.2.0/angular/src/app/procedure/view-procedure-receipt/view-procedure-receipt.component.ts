import { Component, Input, OnInit, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppSessionService } from '@shared/session/app-session.service';
import { ProcedureReceiptServiceProxy, ViewProcedureReceiptDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-view-procedure-receipt',
  standalone: true,
  imports: [CommonModule],
  providers: [ProcedureReceiptServiceProxy],
  templateUrl: './view-procedure-receipt.component.html',
  styleUrl: './view-procedure-receipt.component.css'
})
export class ViewProcedureReceiptComponent implements OnInit {
  @Input() receiptId!: number;
  @Output() onClose = new EventEmitter<void>();

  hospitalName = '';
  receipt?: ViewProcedureReceiptDto;
  loading = false;

  constructor(
    public bsModalRef: BsModalRef,
    private _procedureReceiptService: ProcedureReceiptServiceProxy,
    private _appSessionService: AppSessionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';
    this.loadReceipt();
  }

  loadReceipt(): void {
    this.loading = true;
    this._procedureReceiptService
      .getReceiptWithProcedures(this.receiptId) // 👈 backend method
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(dto => {
        setTimeout(() => {
          this.receipt = dto;
          this.cdr.detectChanges();
        }, 0);
      });
  }

  print(): void {
    const printContents = document.querySelector('.procedure-receipt-wrapper')?.innerHTML;
    const styles = document.head.querySelectorAll('style, link[rel="stylesheet"]');

    const win = window.open('', '', 'width=900,height=1100');

    win.document.write(`
    <html>
    <head>
      <title>Print Procedure Payment Receipt</title>
      ${Array.from(styles).map(s => s.outerHTML).join('')}
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
    this.onClose.emit();
  }
}
