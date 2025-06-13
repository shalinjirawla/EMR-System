import {
  ChangeDetectorRef,
  Component,
  Injector,
  OnInit,
  ViewChild
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BsModalRef } from 'ngx-bootstrap/modal';

import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
@Component({
  selector: 'app-create-prescriptions',
  standalone: true,
  imports: [
    FormsModule,
    CalendarModule,
    DropdownModule,
    CheckboxModule,
    InputTextModule,
    ButtonModule,
    CommonModule,
    SelectModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent
  ],
  templateUrl: './create-prescriptions.component.html',
  styleUrls: ['./create-prescriptions.component.css']
})
export class CreatePrescriptionsComponent extends AppComponentBase implements OnInit {
  @ViewChild('prescriptionForm', { static: true }) prescriptionForm: NgForm;

  saving = false;

  prescription: {
    patientId: number | null;
    doctorId: number | null;
    appointmentId: number | null;
    diagnosis: string;
    notes: string;
    issueDate: Date;
    isFollowUpRequired: boolean;
    items: {
      name: string;
      dosage: string;
    }[];
  } = {
    patientId: null,
    doctorId: null,
    appointmentId: null,
    diagnosis: '',
    notes: '',
    issueDate: new Date(),
    isFollowUpRequired: false,
    items: []
  };

  patients: { id: number; name: string }[] = [];
  appointments: { id: number; title: string }[] = [];
  doctors: { id: number; fullName: string }[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.patients = [
      { id: 1, name: 'John Doe' },
      { id: 2, name: 'Jane Smith' },
      { id: 3, name: 'Rahul Kumar' }
    ];

    this.appointments = [
      { id: 1001, title: 'John Doe - 10 June 2025, 10:00 AM' },
      { id: 1002, title: 'Jane Smith - 12 June 2025, 2:00 PM' },
      { id: 1003, title: 'Rahul Kumar - 14 June 2025, 5:30 PM' }
    ];

    this.doctors = [
      { id: 101, fullName: 'Dr. Amit Verma' },
      { id: 102, fullName: 'Dr. Priya Sharma' },
      { id: 103, fullName: 'Dr. Neeraj Mehta' }
    ];
  }

  addItem(): void {
    this.prescription.items.push({
      name: '',
      dosage: ''
    });
  }

  removeItem(index: number): void {
    this.prescription.items.splice(index, 1);
  }

  save(): void {
    if (!this.prescriptionForm.valid) {
      this.notify.warn('Please fill all required fields');
      return;
    }

    this.saving = true;

    // Simulate a save API call (you can replace with actual service)
    setTimeout(() => {
      console.log('Prescription Saved:', this.prescription);

      this.notify.success('Prescription saved successfully!');
      this.saving = false;
      this.bsModalRef.hide();
    }, 1000);
  }
}
