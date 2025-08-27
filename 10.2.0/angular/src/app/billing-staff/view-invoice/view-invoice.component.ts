import { ChangeDetectorRef, Component, Injector, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { InvoiceDto, InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { CommonModule } from '@node_modules/@angular/common';
import { AppSessionService } from '@shared/session/app-session.service';

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
    window.print();
  }

  downloadPDF(): void {
    // Implement PDF download functionality here
    // You might use libraries like jspdf or pdfmake
    console.log('Downloading PDF...');
  }

}
