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
    window.print();
  }

  close(): void {
    this.bsModalRef.hide();
    this.onClose.emit();
  }
}
