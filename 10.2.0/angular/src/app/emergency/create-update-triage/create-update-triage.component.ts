import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { CreateUpdateTriageDto, EmergencyCaseDto, EmergencyServiceProxy, EmergencySeverity, NurseDto, NurseServiceProxy, TriageServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
@Component({
  selector: 'app-create-update-triage',
  imports: [
    FormsModule,
    CommonModule,
    SelectModule,
    DatePickerModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent
  ],
  providers: [TriageServiceProxy, EmergencyServiceProxy,NurseServiceProxy],
  templateUrl: './create-update-triage.component.html',
  styleUrl: './create-update-triage.component.css'
})
export class CreateUpdateTriageComponent extends AppComponentBase implements OnInit {
  @ViewChild('createTriageForm', { static: true }) createTriageForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  uiAssessmentTime: Date | null = null;
  saving = false;
  emergencyCases: EmergencyCaseDto[] = [];
  id: number;
  triage: any = {
    tenantId: abp.session.tenantId,
    emergencyCaseId: null,
    temperature: null,
    pulse: null,
    respiratoryRate: null,
    bloodPressureSystolic: null,
    bloodPressureDiastolic: null,
    notes: '',
    assessmentTime: new Date()
  };
  severityOptions = [
    { label: 'Critical', value: EmergencySeverity._0 },
    { label: 'Serious', value: EmergencySeverity._1 },
    { label: 'Stable', value: EmergencySeverity._2 }
  ];
  nurses: NurseDto[] = [];
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _triageService: TriageServiceProxy,
    private _emergencyService: EmergencyServiceProxy,
    private _nurseService:NurseServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadEmergencyCases();
    this.loadNurses();
    if (this.id) {
      this.loadTriage(this.id);
    }
  }
  loadNurses() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(res => {
      this.nurses = res.items;
      this.cd.detectChanges();
    });
  }
  loadTriage(id: number) {
    this._triageService.get(id).subscribe(res => {
      this.triage = {
        tenantId: res.tenantId,
        emergencyCaseId: res.emergencyCaseId,
        temperature: res.temperature,
        respiratoryRate: res.respiratoryRate,
        bloodPressureSystolic: res.bloodPressureSystolic,
        bloodPressureDiastolic: res.bloodPressureDiastolic,
        notes: res.notes,
        assessmentTime: res.time,
        heartRate:res.heartRate,
        oxygenSaturation:res.oxygenSaturation,
        nurseId:res.nurseId,
        severity:res.severity
      };
      this.uiAssessmentTime = this.toDate(res.time);
      this.cd.detectChanges();
    });
  }
  loadEmergencyCases() {
    this._emergencyService.getAll('', 0, 1000).subscribe(res => {
      this.emergencyCases = res.items;
      this.cd.detectChanges();
    });
  }
  private toDate(val: any): Date | null {
    if (!val) return null;
    // ABP proxies usually give moment.Moment; but handle ISO string too
    return moment.isMoment(val) ? val.toDate() : new Date(val);
  }

  save() {
    if (!this.createTriageForm?.form?.valid) {
      this.message.warn('Please complete the form properly.');
      return;
    }

    this.saving = true;
    const input = new CreateUpdateTriageDto();
    input.id = this.id;
    input.tenantId = abp.session.tenantId;
    input.emergencyCaseId = this.triage.emergencyCaseId;
    input.notes = this.triage.notes;
    input.temperature = this.triage.temperature;
    input.respiratoryRate = this.triage.respiratoryRate;
    input.bloodPressureSystolic = this.triage.bloodPressureSystolic;
    input.bloodPressureDiastolic = this.triage.bloodPressureDiastolic;
    input.heartRate = this.triage.heartRate;
    input.oxygenSaturation = this.triage.oxygenSaturation;
    input.nurseId = this.triage.nurseId;
    input.severity = this.triage.severity;
    if (this.id) {
      // Update existing triage
      this._triageService.update(input).subscribe({
        next: () => {
          this.notify.info(this.l('UpdatedSuccessfully'));
          this.saving = false;
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.saving = false;
        }
      });
    } else {
      // Create new triage
      this._triageService.create(input).subscribe({
        next: () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.saving = false;
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.saving = false;
        }
      });
    }
  }

}