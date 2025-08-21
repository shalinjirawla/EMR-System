import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { CreateUpdateTriageDto, EmergencyCaseDto, EmergencyServiceProxy, TriageServiceProxy } from '@shared/service-proxies/service-proxies';
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
  providers: [TriageServiceProxy, EmergencyServiceProxy],
  templateUrl: './create-update-triage.component.html',
  styleUrl: './create-update-triage.component.css'
})
export class CreateUpdateTriageComponent extends AppComponentBase implements OnInit {
  @ViewChild('createTriageForm', { static: true }) createTriageForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
uiAssessmentTime: Date | null = null;
  saving = false;
  emergencyCases: EmergencyCaseDto[] = [];
id:number;
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

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _triageService: TriageServiceProxy,
    private _emergencyService: EmergencyServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadEmergencyCases();

    if (this.id) {
      this.loadTriage(this.id);
    }
  }
loadTriage(id: number) {
    this._triageService.get(id).subscribe(res => {
      this.triage = {
        tenantId: res.tenantId,
        emergencyCaseId: res.emergencyCaseId,
        temperature: res.temperature,
        pulse: res.pulse,
        respiratoryRate: res.respiratoryRate,
        bloodPressureSystolic: res.bloodPressureSystolic,
        bloodPressureDiastolic: res.bloodPressureDiastolic,
        notes: res.notes,
        assessmentTime: res.assessmentTime
      };
      this.uiAssessmentTime=this.toDate(res.assessmentTime);
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
  input.id = this.id; // will be undefined for create
  input.tenantId = abp.session.tenantId;
  input.emergencyCaseId = this.triage.emergencyCaseId;
  input.temperature = this.triage.temperature;
  input.pulse = this.triage.pulse;
  input.respiratoryRate = this.triage.respiratoryRate;
  input.bloodPressureSystolic = this.triage.bloodPressureSystolic;
  input.bloodPressureDiastolic = this.triage.bloodPressureDiastolic;
  input.notes = this.triage.notes;
  input.assessmentTime = this.uiAssessmentTime ? moment(this.uiAssessmentTime) : undefined;
debugger
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