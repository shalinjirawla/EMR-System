import { Component, Injector, OnInit, ChangeDetectorRef } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { PrescriptionDto, PrescriptionServiceProxy, PrescriptionViewDto } from "../../../shared/service-proxies/service-proxies";
import { CommonModule, DatePipe } from "@angular/common";
import * as moment from "moment";
import { AppSessionService } from "@shared/session/app-session.service";

@Component({
  selector: 'app-view-prescription',
  imports: [DatePipe,CommonModule],
  providers: [PrescriptionServiceProxy],
  templateUrl: './view-prescription.component.html',
  styleUrl: './view-prescription.component.css'
})
export class ViewPrescriptionComponent implements OnInit {
  id: number;
  prescription: PrescriptionViewDto;
  currentYear: number = new Date().getFullYear();
  additionalInstructions = false;
  hospitalName = '';


  constructor(
    injector: Injector,
    private _prescriptionService: PrescriptionServiceProxy,
    public bsModalRef: BsModalRef,
    private _appSessionService: AppSessionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadPrescription();
    this.hospitalName = this._appSessionService.tenant?.tenancyName ?? 'Default';
  }

  calculateAge(dob: string): number {
    if (!dob) return 0;
    const birthDate = new Date(dob);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  }

  printPrescription(): void {
    window.print();
  }

  private loadPrescription(): void {
    this._prescriptionService.getPrescriptionForView(this.id).subscribe(result => {
      this.prescription = result;
      this.cdr.detectChanges();
    });
  }
  downloadAsPDF(): void {
    alert("This functionality is in progress...")
   
    
  }
  
}