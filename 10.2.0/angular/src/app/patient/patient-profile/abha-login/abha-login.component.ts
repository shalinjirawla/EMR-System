import { Component, EventEmitter, Input, Output, Inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AbhaApiService } from '@shared/services/abha-api.service';

@Component({
  selector: 'app-abha-login',
  templateUrl: './abha-login.component.html',
  styleUrls: ['./abha-login.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class AbhaLoginComponent {
  @Input() patientId: number;
  @Output() profileLinked = new EventEmitter<any>();

  step = 1;
  loginId = '';
  otp = '';
  txnId = '';
  xToken = '';
  linkedProfile: any = null;
  isLoading = false;

  constructor(
    @Inject(AbhaApiService) private abhaApiService: AbhaApiService,
    private cdr: ChangeDetectorRef
  ) {}

  requestOtp() {
    if (!this.loginId) {
      abp.notify.error('Please enter an ABHA Number or Aadhaar Number.');
      return;
    }
    this.isLoading = true;
    this.abhaApiService.requestLoginOtp(this.loginId).subscribe(
      (res: any) => {
        const data = res.result || res;
        this.txnId = data.txnId;
        this.step = 2;
        this.isLoading = false;
        abp.notify.success('OTP sent to registered mobile number.');
        this.cdr.detectChanges();
      },
      () => { 
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    );
  }

  verifyOtp() {
    if (!this.otp || this.otp.length !== 6) {
      abp.notify.error('Please enter a valid 6-digit OTP.');
      return;
    }
    this.isLoading = true;
    this.abhaApiService.verifyLoginOtp(this.otp, this.txnId, this.loginId).subscribe(
      (res: any) => {
        const data = res.result || res;
        this.xToken = data.xToken;
        this.step = 3;
        this.isLoading = false;
        abp.notify.success('OTP verified successfully.');
        this.fetchProfile();
        this.cdr.detectChanges();
      },
      () => { 
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    );
  }

  fetchProfile() {
    this.isLoading = true;
    this.abhaApiService.fetchAndLinkProfile(this.patientId, this.xToken).subscribe(
      (res: any) => {
        const data = res.result || res;
        this.linkedProfile = data;
        // attach token so parent can display dashboard
        this.linkedProfile.xToken = this.xToken;
        this.isLoading = false;
        abp.notify.success('Profile successfully linked with EMR Patient.');
        this.profileLinked.emit(this.linkedProfile);
        this.cdr.detectChanges();
      },
      () => { 
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    );
  }
}
