import { ChangeDetectorRef, Component, Injector, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { InvoiceDto, InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { CommonModule } from '@node_modules/@angular/common';
import { AppSessionService } from '@shared/session/app-session.service';
import html2pdf from 'html2pdf.js';


@Component({
  selector: 'app-view-invoice',
  imports: [CommonModule],
  providers: [InvoiceServiceProxy],
  templateUrl: './view-invoice.component.html',
  styleUrl: './view-invoice.component.css'
})
export class ViewInvoiceComponent implements OnInit {
  @Input() id: number;
  invoice: InvoiceDto;
  hospitalName = '';

  constructor(
    private _invoiceService: InvoiceServiceProxy,
    public bsModalRef: BsModalRef,
    public cd: ChangeDetectorRef,
    private _appSessionService: AppSessionService

  ) { }

  ngOnInit(): void {
    this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';

    if (this.id) {
      this._invoiceService.getInvoiceWithItems(this.id).subscribe(res => {
        this.invoice = res;
        this.cd.detectChanges();
      });
    }
  }
  printInvoice(): void {
  const printContents = document.querySelector('.invoice-wrapper')?.innerHTML;
  const styles = document.head.querySelectorAll('style, link[rel="stylesheet"]');

  const win = window.open('', '', 'width=900,height=1100');

  win.document.write(`
    <html>
    <head>
      <title>Print Invoice</title>
      ${Array.from(styles).map(s => s.outerHTML).join('')}
      <style>
        @page { size: A4; margin: 10mm; }
      </style>
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
downloadPDF(): void {
  const element = document.getElementById('invoiceToPrint');

  const opt: any = {
  margin: 5,
  filename: `${this.invoice.invoiceNo}.pdf`,
  image: { type: 'jpeg' as 'jpeg', quality: 1 },
  html2canvas: { scale: 4, useCORS: true },
  jsPDF: {
    unit: 'mm',
    format: 'a4',
    orientation: 'portrait' as 'portrait'
  }
};
html2pdf()
  .from(element)
  .set(opt)
  .save();

}
}
