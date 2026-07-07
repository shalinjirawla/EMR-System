import { Component, EventEmitter, Input, Output, Inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AbhaApiService } from '@shared/services/abha-api.service';

@Component({
  selector: 'app-abha-creation',
  templateUrl: './abha-creation.component.html',
  styleUrls: ['./abha-creation.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class AbhaCreationComponent {
  @Input() patientId: number;
  @Output() abhaCreated = new EventEmitter<any>();

  step = 1;
  aadhaarNumber = '';
  otp = '';
  mobileNumber = '';
  txnId = '';
  profile: any = null;
  suggestions: string[] = [];
  selectedAddress = '';
  customAddress = '';
  isLoading = false;

  constructor(
    @Inject(AbhaApiService) private abhaApiService: AbhaApiService,
    private cdr: ChangeDetectorRef
  ) { }

  requestOtp() {
    if (!this.aadhaarNumber || this.aadhaarNumber.length !== 12) {
      abp.notify.error('Please enter a valid 12-digit Aadhaar number.');
      return;
    }
    this.isLoading = true;
    this.abhaApiService.requestAadhaarOtp(this.aadhaarNumber).subscribe(
      (res: any) => {
        const data = res.result || res;
        this.txnId = data.txnId;
        this.step = 2;
        this.isLoading = false;
        abp.notify.success('OTP sent to Aadhaar registered mobile.');
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
    if (!this.mobileNumber || this.mobileNumber.length !== 10) {
      abp.notify.error('Please enter a valid 10-digit mobile number.');
      return;
    }
    this.isLoading = true;
    this.abhaApiService.verifyAadhaarOtp(this.otp, this.txnId, this.mobileNumber).subscribe(
      (res: any) => {
        const data = res.result || res;
        this.txnId = data.txnId;
        
        const p = data.ABHAProfile || data.profile;
        if (p) {
          this.profile = {
            name: p.name || `${p.firstName || ''} ${p.lastName || ''}`.trim(),
            gender: p.gender,
            yearOfBirth: p.yearOfBirth || p.dob
          };
        }

        this.step = 3;
        this.isLoading = false;
        if (data.isNew === false) {
           abp.notify.info('Account already exists. Profile fetched.');
        } else {
           abp.notify.success('Aadhaar verified. Profile fetched successfully.');
        }
        
        this.loadSuggestions();
        this.cdr.detectChanges();
      },
      () => { 
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    );
  }

  loadSuggestions() {
    this.abhaApiService.suggestAddresses(this.txnId).subscribe(
      (res: any) => {
        const data = res.result || res;
        this.suggestions = data;
        this.cdr.detectChanges();
      });
  }

  createAbha() {
    const finalAddress = this.customAddress ? this.customAddress : this.selectedAddress;
    if (!finalAddress) {
      abp.notify.error('Please select or enter an ABHA address.');
      return;
    }

    this.isLoading = true;
    this.abhaApiService.createAbhaAddress(this.patientId, finalAddress, this.txnId).subscribe(
      (res: any) => {
        this.isLoading = false;
        abp.notify.success('ABHA successfully created and linked to patient!');
        this.abhaCreated.emit(res.result);
      },
      () => { this.isLoading = false; }
    );
  }
}
