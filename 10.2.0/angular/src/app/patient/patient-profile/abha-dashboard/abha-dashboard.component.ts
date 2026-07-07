import { Component, Input, OnInit, OnChanges, SimpleChanges, Inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AbhaApiService } from '@shared/services/abha-api.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-abha-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './abha-dashboard.component.html',
  styleUrls: ['./abha-dashboard.component.css']
})
export class AbhaDashboardComponent implements OnInit, OnChanges {
  @Input() xToken: string;
  @Input() patientName: string;
  @Input() patientId: number;
  @Input() savedQrCodeBase64: string | null = null;
  @Input() savedCardBase64: string | null = null;

  cardDataUrl: SafeResourceUrl | null = null;
  qrCodeUrl: SafeResourceUrl | null = null;
  rawCardBase64: string | null = null;
  isLoading = false;

  constructor(
    @Inject(AbhaApiService) private abhaApiService: AbhaApiService,
    @Inject(DomSanitizer) private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.loadDashboard();
  }

  ngOnChanges(changes: SimpleChanges) {
    if ((changes['xToken'] && !changes['xToken'].isFirstChange() && changes['xToken'].currentValue) || 
        (changes['savedQrCodeBase64'] && changes['savedQrCodeBase64'].currentValue)) {
      this.loadDashboard();
    }
  }

  loadDashboard() {
    if (this.savedQrCodeBase64) {
      this.qrCodeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64,${this.savedQrCodeBase64}`);
    } else if (this.xToken) {
      this.isLoading = true;
      this.abhaApiService.getAbhaQrCode(this.xToken).subscribe(
        (res: any) => {
          const data = res.result || res;
          const base64 = data.base64QrCode;
          this.qrCodeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64,${base64}`);
          this.cdr.detectChanges();
        },
        (err) => console.error('getAbhaQrCode error:', err)
      );
    }

    if (this.savedCardBase64) {
      this.rawCardBase64 = this.savedCardBase64;
      if (this.rawCardBase64.startsWith('JVBERi0')) {
        this.cardDataUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`data:application/pdf;base64,${this.rawCardBase64}`);
      } else {
        this.cardDataUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64,${this.rawCardBase64}`);
      }
    } else if (this.xToken) {
      this.isLoading = true;
      this.abhaApiService.getAbhaCard(this.xToken).subscribe(
        (res: any) => {
          const data = res.result || res;
          this.rawCardBase64 = data.base64Card;
          if (this.rawCardBase64) {
            if (this.rawCardBase64.startsWith('JVBERi0')) {
              this.cardDataUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`data:application/pdf;base64,${this.rawCardBase64}`);
            } else {
              this.cardDataUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64,${this.rawCardBase64}`);
            }
          }
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        (err) => {
          console.error('getAbhaCard error:', err);
          this.isLoading = false;
        }
      );
    }
  }

  downloadCard() {
    if (!this.rawCardBase64) return;

    const isPdf = this.rawCardBase64.startsWith('JVBERi0');
    const mimeType = isPdf ? 'application/pdf' : 'image/png';
    const extension = isPdf ? 'pdf' : 'png';

    const link = document.createElement('a');
    link.href = `data:${mimeType};base64,${this.rawCardBase64}`;
    link.download = `ABHA_Card_${this.patientName || 'Patient'}.${extension}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  requestHIUConsent() {
    if (!this.patientId) {
      abp.notify.error('Patient ID is missing.');
      return;
    }
    
    abp.message.confirm(
      'This will send a consent request to the patient\'s ABHA App to fetch their external health records.',
      'Request External Health Records',
      (isConfirmed) => {
        if (isConfirmed) {
          abp.ui.setBusy();
          this.abhaApiService.initiateConsentRequest(this.patientId, 'Dr. Default').subscribe(
            (res) => {
              abp.ui.clearBusy();
              abp.notify.success('Consent Request successfully sent to ABHA Network!');
            },
            (err) => {
              abp.ui.clearBusy();
              console.error(err);
              abp.notify.error('Failed to send consent request.');
            }
          );
        }
      }
    );
  }

  externalRecords: any[] = [];

  loadExternalRecords() {
    if (!this.patientId) return;
    abp.ui.setBusy();
    this.abhaApiService.getExternalHealthRecords(this.patientId).subscribe(
      (res) => {
        abp.ui.clearBusy();
        const records = res.result || res;
        this.externalRecords = records.map((r: any) => {
           try {
             return { ...r, parsedPayload: JSON.parse(r.fhirPayload) };
           } catch {
             return { ...r, parsedPayload: null };
           }
        });
        if(this.externalRecords.length > 0) {
            abp.notify.success('External records loaded successfully!');
        } else {
            abp.notify.info('No external records found yet.');
        }
      },
      (err) => {
        abp.ui.clearBusy();
        console.error(err);
        abp.notify.error('Failed to load external records.');
      }
    );
  }
}
