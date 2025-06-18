import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@node_modules/@angular/common';
import { FormsModule, NgForm } from '@node_modules/@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { PatientDto, NurseDto, PatientServiceProxy, NurseServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-vitals',
  imports: [CommonModule, FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent, SelectModule, DatePickerModule, InputTextModule, TextareaModule],
  providers: [PatientServiceProxy, NurseServiceProxy],
  templateUrl: './create-vitals.component.html',
  styleUrl: './create-vitals.component.css'
})
export class CreateVitalsComponent implements OnInit {
  @ViewChild('createVitalForm', { static: true }) createVitalForm: NgForm;
  saving = false;

  patients: PatientDto[] = [];
  nurses: NurseDto[] = [];

  vital: any = {
    patientId: null,
    nurseId: null,
    bloodPressure: '',
    heartRate: '',
    respirationRate: null,
    temperature: null,
    oxygenSaturation: null,
    height: '',
    weight: '',
    bmi: '',
    notes: ''
  };

  constructor(
    public bsModalRef: BsModalRef,
    private _patientService: PatientServiceProxy,
    private _nurseService: NurseServiceProxy
  ) { }

  ngOnInit(): void {
    this.loadPatients();
    this.loadNurses();
  }

  loadPatients() {
    this._patientService.getAllPatientByTenantID(abp.session.tenantId).subscribe({
      next: res => this.patients = res.items,
      error: err => console.error(err)
    });
  }

  loadNurses() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
      next: res => this.nurses = res.items,
      error: err => console.error(err)
    });
  }

  save() {
    if (this.createVitalForm.invalid) {
      console.warn("Form is invalid");
      return;
    }

    this.saving = true;

    // Call your service here to save the vital data
    console.log('Saving Vital Record:', this.vital);

    // Simulate saving done
    setTimeout(() => {
      this.saving = false;
      this.bsModalRef.hide();
    }, 1000);
  }
}
