import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { InsuranceClaimDto, InvoiceDto, InvoiceItemDto, InsuranceClaimServiceProxy, InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from "@shared/components/modal/abp-modal-header.component";
import { CommonModule } from '@node_modules/@angular/common';
@Component({
  selector: 'app-view-insurance-claim',
  imports: [AbpModalHeaderComponent,CommonModule],
  providers: [InvoiceServiceProxy],
  templateUrl: './view-insurance-claim.component.html',
  styleUrl: './view-insurance-claim.component.css'
})
export class ViewInsuranceClaimComponent extends AppComponentBase implements OnInit {
  id: number;
  insuranceName: string;
  invoiceId: number;
  status: number;

  claim: InsuranceClaimDto[] = [];
  invoice: InvoiceDto = new InvoiceDto();
  items: InvoiceItemDto[] = [];

  loading = false;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _invoiceService: InvoiceServiceProxy,
    public cd: ChangeDetectorRef

  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.invoiceId) {
      this.loadClaimDetails(this.invoiceId);
    }
  }

  loadClaimDetails(id: number): void {
    this.loading = true;
    this._invoiceService
      .getInvoiceWithItems(id)
      .subscribe((result) => {
        this.loading = false;
        this.invoice = result;
        this.claim = result.claims;
        console.log(this.claim)
        this.items = result.items || [];
        this.cd.detectChanges();
      });
  }
getTotal(item: InvoiceItemDto): number {
    return item.unitPrice * item.quantity;
  }
  getStatusLabel(status: number): string {
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Submitted';
      case 2: return 'Partially Approved';
      case 3: return 'Approved';
      case 4: return 'Rejected';
      case 5: return 'Paid';
      default: return '-';
    }
  }

  close(): void {
    this.bsModalRef.hide();
  }
}
