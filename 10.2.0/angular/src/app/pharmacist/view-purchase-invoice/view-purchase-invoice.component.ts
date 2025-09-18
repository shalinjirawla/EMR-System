import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@node_modules/@angular/common';
import { AppComponentBase } from '@shared/app-component-base';
import { PurchaseInvoiceServiceProxy, PurchaseInvoiceDto } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-view-purchase-invoice',
  imports: [CommonModule],
  providers:[PurchaseInvoiceServiceProxy],
  templateUrl: './view-purchase-invoice.component.html',
  styleUrl: './view-purchase-invoice.component.css'
})
export class ViewPurchaseInvoiceComponent extends AppComponentBase implements OnInit {
  invoiceId: number;
  invoice: PurchaseInvoiceDto;

  constructor(
    injector: Injector,
    private _purchaseInvoiceService: PurchaseInvoiceServiceProxy,
    public bsModalRef: BsModalRef,
    public cd:ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.invoiceId) {
      this.loadInvoice(this.invoiceId);
    }
  }
printInvoice(){
  window.print();
}
  loadInvoice(id: number): void {
    this._purchaseInvoiceService.get(id).subscribe((result: PurchaseInvoiceDto) => {
      this.invoice = result;
      this.cd.detectChanges();
    });
  }

  close(): void {
    this.bsModalRef.hide();
  }
}
