import { Component, Input, OnInit, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { AppointmentServiceProxy, AppointmentReceiptDto } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { AppSessionService } from '@shared/session/app-session.service';

@Component({
  selector: 'app-view-appointment-receipt',
  imports: [CommonModule],
  standalone: true,
  providers: [AppointmentServiceProxy],
  templateUrl: './view-appointment-receipt.component.html',
  styleUrl: './view-appointment-receipt.component.css'
})
export class ViewAppointmentReceiptComponent implements OnInit {
  @Input() appointmentId!: number;
  @Output() onClose = new EventEmitter<void>();
  hospitalName = '';

  receipt?: AppointmentReceiptDto;
  loading = false;

  constructor(
    public bsModalRef: BsModalRef,
    private _appointmentService: AppointmentServiceProxy,
    private _appSessionService: AppSessionService,
    public cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadReceipt();
    this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';
    
  }

  loadReceipt(): void {
    this.loading = true;
    this._appointmentService
      .getReceiptForAppointment(this.appointmentId)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(dto => {
        // defer so it doesn’t run mid‑check
        setTimeout(() => {
          this.receipt = dto;
          this.cdr.detectChanges();
        }, 0);
      });
  }
  

  close(): void {
    this.bsModalRef.hide();
    this.onClose.emit();
  }
}
