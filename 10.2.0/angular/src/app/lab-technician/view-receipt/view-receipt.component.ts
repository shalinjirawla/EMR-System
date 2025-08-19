import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule, formatDate } from '@angular/common';
import { LabTestReceiptServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppSessionService } from '@shared/session/app-session.service';

@Component({
  selector: 'app-view-receipt',
  imports: [CommonModule],
  providers:[LabTestReceiptServiceProxy],
  templateUrl: './view-receipt.component.html',
  styleUrl: './view-receipt.component.css'
})
export class ViewReceiptComponent implements OnInit {
  // set by initialState when the modal opens
  labReceiptId!: number;
  hospitalName = '';
  loading = false;
  receiptDetail: any = null; // LabTestReceiptDisplayDto

  constructor(
    public bsModalRef: BsModalRef,
    private receiptService: LabTestReceiptServiceProxy,
    public cd: ChangeDetectorRef,
    private _appSessionService: AppSessionService
  ) {}

  ngOnInit(): void {
    this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';
    if (!this.labReceiptId) {
      console.error('ViewReceiptComponent: labReceiptId not provided.');
      return;
    }
    this.loadReceipt();
  }

  close() {
    this.bsModalRef.hide();
  }

  private loadReceipt() {
    this.loading = true;
    // NOTE: change method name to match generated proxy if different
    this.receiptService.getReceiptDisplay(this.labReceiptId).subscribe({
      next: (res) => {
        this.receiptDetail = res;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }
print(): void {
    // print only the modal content (the CSS has @media print styles)
    window.print();
  }
   format(dt: string | Date | null): string {
    if (!dt) return '';
    try {
      return formatDate(dt, 'dd-MMM-yyyy hh:mm a', 'en-IN');
    } catch {
      return String(dt);
    }
  }
  // helper (if needed) to format price numbers (template uses currency pipe)
  getTotal() {
    return this.receiptDetail?.totalFee ?? 0;
  }
}