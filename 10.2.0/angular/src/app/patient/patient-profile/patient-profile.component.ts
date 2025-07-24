import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ButtonModule } from 'primeng/button';
import { TabsModule } from 'primeng/tabs';
import { StepperModule } from 'primeng/stepper';
import { TableModule } from 'primeng/table';
import { AvatarModule } from 'primeng/avatar';
import { ProgressBarModule } from 'primeng/progressbar';
import { AppointmentStatus, PatientDetailsAndMedicalHistoryDto, PatientPrescriptionsHistoryDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import { CommonModule } from '@node_modules/@angular/common';
import { FormsModule } from '@node_modules/@angular/forms';
import { ChipModule } from 'primeng/chip';
import { groupBy } from 'lodash';
import { TagModule } from 'primeng/tag';
@Component({
  selector: 'app-patient-profile',
  imports: [AbpModalHeaderComponent,TagModule, TableModule, ProgressBarModule, ChipModule
    , ButtonModule, TabsModule, StepperModule, AvatarModule, CommonModule, FormsModule],
  templateUrl: './patient-profile.component.html',
  styleUrl: './patient-profile.component.css',
  providers: [PatientServiceProxy]
})
export class PatientProfileComponent implements OnInit {
  patientDetailsAndMedicalHistory!: PatientDetailsAndMedicalHistoryDto;
  id!: number;
  statusOptions = [
    { label: 'Scheduled', value: AppointmentStatus._0 },
    { label: 'Rescheduled', value: AppointmentStatus._1 },
    { label: 'Checked In', value: AppointmentStatus._2 },
    { label: 'Completed', value: AppointmentStatus._3 },
    { label: 'Cancelled', value: AppointmentStatus._4 },
  ];
  constructor(public bsModalRef: BsModalRef, private _patientService: PatientServiceProxy,
    private ref: ChangeDetectorRef
  ) {
  }
  ngOnInit() {
    this.GetPatientDetailsAndMedicalHistory();
  }
  groupedPrescriptions: any[] = [];
  GetPatientDetailsAndMedicalHistory() {
    this._patientService.patientDetailsAndMedicalHistory(this.id).subscribe({
      next: (res) => {
        this.patientDetailsAndMedicalHistory = res;
        const prescriptions = this.patientDetailsAndMedicalHistory.patientPrescriptionsHistory || [];
        const grouped = new Map<number, any>();

        for (const presc of prescriptions) {
          for (const item of presc.items || []) {
            const existing = grouped.get(item.prescriptionId);
            if (!existing) {
              grouped.set(item.prescriptionId, {
                doctorName: presc.doctorName,
                doctorId: presc.doctorId,
                duration: item.duration,
                items: []
              });
            }
            grouped.get(item.prescriptionId).items.push(item);
          }
        }

        this.groupedPrescriptions = Array.from(grouped.values());
        this.ref.detectChanges();
      },
      error: (err) => {
      }
    });
  }
  calculateAgeDisplay(dob: string | Date): string {
    const birthDate = new Date(dob);
    const today = new Date();

    const diffInMs = today.getTime() - birthDate.getTime();
    const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

    const years = today.getFullYear() - birthDate.getFullYear();
    const months =
      today.getMonth() - birthDate.getMonth() +
      years * 12 -
      (today.getDate() < birthDate.getDate() ? 1 : 0);

    if (years >= 1) {
      return `${years} year${years > 1 ? 's' : ''}`;
    } else if (months >= 1) {
      return `${months} month${months > 1 ? 's' : ''}`;
    } else {
      return `${diffInDays} day${diffInDays > 1 ? 's' : ''}`;
    }
  }

  getStatusLabel(value: number): string {
    const status = this.statusOptions.find(s => s.value === value);
    return status ? status.label : '';
  }
  getStatusClass(value: number): string {
    switch (value) {
      case AppointmentStatus._0: return 'status-scheduled';    // Scheduled
      case AppointmentStatus._1: return 'status-rescheduled';  // Rescheduled
      case AppointmentStatus._2: return 'status-checkedin';    // Checked In
      case AppointmentStatus._3: return 'status-completed';    // Completed
      case AppointmentStatus._4: return 'status-cancelled';    // Cancelled
      default: return '';
    }
  }

  getStatusSeverity(value: number): 'info' | 'warn' | 'success' | 'danger' | 'secondary' | 'contrast' {
    switch (value) {
      case AppointmentStatus._0: return 'info';        // Scheduled
      case AppointmentStatus._1: return 'secondary';   // Rescheduled
      case AppointmentStatus._2: return 'success';     // Checked In
      case AppointmentStatus._3: return 'success';     // Completed
      case AppointmentStatus._4: return 'danger';      // Cancelled
      default: return 'contrast';
    }
  }
}
