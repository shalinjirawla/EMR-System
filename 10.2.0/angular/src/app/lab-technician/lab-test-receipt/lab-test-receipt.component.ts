import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { LabTestReceiptServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { AppSessionService } from '../../../shared/session/app-session.service';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-lab-test-receipt',
  imports: [CommonModule],
  providers:[LabTestReceiptServiceProxy],
  templateUrl: './lab-test-receipt.component.html',
  styleUrl: './lab-test-receipt.component.css'
})
export class LabTestReceiptComponent implements OnInit {

  @Input() prescriptionLabTestId!: number;
  @Output() onClose = new EventEmitter<void>();

  hospitalName = '';
  receipt?: any;
  // receipt?: ViewLabTestReceiptDto;
  loading = false;

  constructor(
    public bsModalRef: BsModalRef,
    private _labTestReceiptService: LabTestReceiptServiceProxy,
    private _appSessionService: AppSessionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';
    //this.loadReceipt();
  }

  // loadReceipt(): void {
  //   debugger
  //   this.loading = true;
  //   this._labTestReceiptService
  //     .getViewByPrescriptionLabTestId(this.prescriptionLabTestId)
  //     .pipe(finalize(() => (this.loading = false)))
  //     .subscribe(dto => {
  //       setTimeout(() => {
  //         this.receipt = dto;
  //         this.cdr.detectChanges();
  //       }, 0);
  //     });
  // }

  close(): void {
    this.bsModalRef.hide();
    this.onClose.emit();
  }

  getPrintStyles(): string {
  return `
    ${require('./lab-test-receipt.component.css').default}
  `;
}

print(): void {
  // 1. Grab just the receipt markup:
  const receiptEl = document.querySelector('.medical-receipt') as HTMLElement;
  if (!receiptEl) { return; }
  const html = receiptEl.outerHTML;

  // 2. Open a new window:
  const popup = window.open('', '_blank', 'width=800,height=600');
  if (!popup) { return; }

  // 3. Write the full HTML + inline CSS:
  popup.document.open();
  popup.document.write(`
    <html>
      <head>
        <title>Lab Test Payment Receipt</title>
        <style>
          ${this._printCss}
        </style>
      </head>
      <body onload="window.print(); window.close();">
        ${html}
      </body>
    </html>
  `);
  popup.document.close();
}


private readonly _printCss = `
  .medical-receipt {
    width: 100%;
    max-width: 800px;
    margin: 0 auto;
    background: #fff;
    box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
    font-family: 'Arial', sans-serif;
    color: #333;
  }

  .receipt-header {
    padding: 20px;
    border-bottom: 2px dashed #e0e0e0;
    background: linear-gradient(to right, #f8f9ff, #eef0fb);
  }

  .clinic-brand {
    display: flex;
    align-items: center;
    margin-bottom: 15px;
  }

  .clinic-logo {
    width: 60px;
    height: 60px;
    margin-right: 15px;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .clinic-logo img {
  width: 100%;
  height: auto;
  object-fit: scale-down;
}

  .clinic-details h1 {
    margin: 0;
    font-size: 22px;
    color: #3f51b5;
  }

  .clinic-details p {
    margin: 5px 0 0;
    font-size: 13px;
    color: #666;
  }

  .receipt-title {
    text-align: center;
    margin-top: 10px;
  }

  .receipt-title h2 {
    margin: 0;
    color: #333;
    font-size: 18px;
    text-transform: uppercase;
    letter-spacing: 1px;
  }

  .receipt-number {
    margin-top: 5px;
    font-weight: bold;
    color: #3f51b5;
  }

  .generated-on {
    text-align: right;
    font-size: 12px;
    color: #888;
    margin-top: 5px;
  }

  .receipt-body {
    padding: 20px;
  }

  .patient-details {
    margin-bottom: 20px;
  }

  .detail-row {
    display: flex;
    margin-bottom: 8px;
  }

  .label {
    font-weight: bold;
    width: 150px;
    color: #555;
  }

  .value {
    flex: 1;
  }

  .status-paid {
    color: #4caf50;
    font-weight: bold;
  }

  /* === Updated Payment Table Section === */
  .payment-table {
    margin: 25px 0;
    border: 1px solid #e0e0e0;
    border-radius: 5px;
    overflow: hidden;
  }

  .payment-table::before {
    content: "";
    display: block;
    border-top: 1px dotted #ccc;
    margin-bottom: 20px;
  }

  .table-header,
  .table-row,
  .table-footer {
    display: grid;
    grid-template-columns: 70% 30%;
    /* Description | Amount */
    padding: 12px 15px;
  }

  .table-header {
    background: #f5f5f5;
    font-weight: bold;
    border-bottom: 1px solid #e0e0e0;
  }

  .table-row {
    border-bottom: 1px dashed #e0e0e0;
  }

  .table-row:last-child {
    border-bottom: none;
  }

  .table-footer {
    background: #f9f9f9;
    font-weight: bold;
  }

  .header-item,
  .row-item,
  .footer-item {
    font-size: 14px;
  }

  .header-item:last-child,
  .row-item:last-child,
  .footer-item:last-child {
    text-align: right;
    font-family: 'Courier New', monospace;
    font-weight: bold;
  }

  /* === End Payment Table Section === */

  .payment-details {
    margin-top: 25px;
    padding-top: 15px;
    border-top: 1px dashed #e0e0e0;
  }

  .receipt-footer {
    display: flex;
    justify-content: flex-end;
    align-items: flex-end;
    margin-top: 30px;
    padding-top: 20px;
    /* border-top: 1px dashed #e0e0e0; */
  }

  .thank-you-note {
    color: #666;
    font-size: 14px;
  }

  .thank-you-note p {
    margin: 0 0 5px 0;
    font-weight: bold;
  }

  .clinic-stamp {
    text-align: center;
  }

  .stamp {
    width: 80px;
    height: 80px;
    border: 2px solid #4caf50;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #4caf50;
    font-weight: bold;
    margin: 0 auto 5px;
    transform: rotate(15deg);
    box-shadow: 0 0 8px rgba(76, 175, 80, 0.4);
  }

  .signature-line {
    margin-top: 40px;
    text-align: right;
    font-size: 13px;
    color: #555;
    border-top: 1px solid #aaa;
    width: 200px;
    float: right;
    padding-top: 5px;
  }

  .receipt-actions {
    display: flex;
    justify-content: center;
    gap: 15px;
    padding: 15px 20px;
    background: #f5f5f5;
    border-top: 1px solid #e0e0e0;
  }

  .btn {
    padding: 10px 20px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 8px;
    font-weight: bold;
  }

  .print-btn {
    background: #3f51b5;
    color: white;
  }

  .print-btn:hover {
    background: #2c3ea8;
  }

  .close-btn {
    background: #f44336;
    color: white;
  }

  .close-btn:hover {
    background: #d32f2f;
  }

  @media print {
    body {
      background: white;
      -webkit-print-color-adjust: exact;
      print-color-adjust: exact;
    }

    .receipt-actions {
      display: none !important;
    }

    .medical-receipt {
      box-shadow: none;
      margin: 0;
      width: 100%;
      font-size: 12px;
    }

    .stamp {
      color: #4caf50 !important;
      border-color: #4caf50 !important;
    }

    .receipt-header,
    .receipt-footer {
      page-break-inside: avoid;
    }
  }
  
`;

}