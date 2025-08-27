import { ChangeDetectorRef, Component, Injector, OnInit, Output, ViewChild, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { AppointmentServiceProxy, CreateUpdatePrescriptionItemDto, LabReportsTypeServiceProxy, PharmacistInventoryDtoPagedResultDto, PharmacistInventoryServiceProxy, PrescriptionItemDto, PrescriptionServiceProxy, PatientDropDownDto, EmergencyServiceProxy, EmergencyCaseDto } from '@shared/service-proxies/service-proxies';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { AppointmentDto, CreateUpdatePrescriptionDto, DoctorDto, DoctorServiceProxy, PatientDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { TextareaModule } from 'primeng/textarea';
import { AppSessionService } from '@shared/session/app-session.service';
import { MultiSelectModule } from 'primeng/multiselect';
import { PermissionCheckerService } from '@node_modules/abp-ng2-module';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
@Component({
  selector: 'app-create-update-emergency-prescriptions',
  standalone: true,
  imports: [
    FormsModule, CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent, MultiSelectModule
  ],
  templateUrl: './create-update-emergency-prescriptions.component.html',
  styleUrl: './create-update-emergency-prescriptions.component.css',
  providers: [DoctorServiceProxy, PatientServiceProxy, PharmacistInventoryServiceProxy, LabReportsTypeServiceProxy, AppointmentServiceProxy, AppSessionService, PrescriptionServiceProxy, EmergencyServiceProxy]
})
export class CreateUpdateEmergencyPrescriptionsComponent extends AppComponentBase implements OnInit {
  @ViewChild('emergencyPrescriptionForm', { static: true }) emergencyPrescriptionForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  patients!: PatientDropDownDto[];
  emergencyCase!: EmergencyCaseDto[];
  labTests: any[] = [];
  selectedLabTests: any[] = [];
  doctorID!: number;
  showAddPatientButton = false;
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};
  selectedMedicineUnits: { [medicineName: string]: string } = {};
  id: any;
  frequencyOptions = [
    { label: 'Once a day', value: 'Once a day' },
    { label: 'Twice a day', value: 'Twice a day' },
    { label: 'Three times a day', value: 'Three times a day' },
    { label: 'Four times a day', value: 'Four times a day' },
    { label: 'Every 6 hours', value: 'Every 6 hours' },
    { label: 'Every 8 hours', value: 'Every 8 hours' },
    { label: 'Every 12 hours', value: 'Every 12 hours' },
    { label: 'As needed', value: 'As needed' }
  ];
  durationUnits = [
    { label: 'Days', value: 'Days' },
    { label: 'Weeks', value: 'Weeks' },
    { label: 'Months', value: 'Months' }
  ];
  prescription: any = {
    id: 0,
    tenantId: abp.session.tenantId,
    diagnosis: '',
    notes: '',
    issueDate: moment(),
    isFollowUpRequired: false,
    appointmentId: 0,
    doctorId: 0,
    patientId: 0,
    items: [],
    labTestIds: [],
    isEmergencyPrescription: true,
    emergencyCaseId: 0,
  };
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _doctorService: DoctorServiceProxy,
    private _patientService: PatientServiceProxy,
    private _sessionService: AppSessionService,
    private _prescriptionService: PrescriptionServiceProxy,
    private _labService: LabReportsTypeServiceProxy,
    private _pharmacistInventoryService: PharmacistInventoryServiceProxy,
    private permissionChecker: PermissionCheckerService,
    private _modalService: BsModalService,
    private _emergencyService: EmergencyServiceProxy,
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.showAddPatientButton = this.permissionChecker.isGranted('Pages.Users');
    this.FetchDoctorID();
    this.LoadLabReports();
    this.loadMedicines();
    this.LoadEmergencyCases();
    if (this.id > 0) {
      this.loadLabTestsAndPrescription();
    }
  }
  loadMedicines() {
    // Call getAll() with default parameters to get all available medicines
    this._pharmacistInventoryService.getAll(
      undefined,  // keyword
      undefined,  // sorting
      undefined,  // minStock
      undefined,  // maxStock
      undefined,  // fromExpiryDate
      true,       // isAvailable (only get available medicines)
      undefined,  // skipCount
      undefined   // maxResultCount
    ).subscribe({
      next: (res) => {
        if (res.items && res.items.length > 0) {
          this.medicineOptions = res.items.map(item => ({
            label: item.medicineName,
            value: item.id, // Use medicineId as value
            name: item.medicineName // Store name separately
          }));


          // Prepare dosage options for each medicine
          res.items.forEach(medicine => {
            const unit = medicine.unit;
            if (unit) {
              // Split units if they are comma separated (e.g., "200 mg, 500 mg")
              const units = unit.split(',').map(u => u.trim());
              this.medicineDosageOptions[medicine.medicineName] = units;
              this.selectedMedicineUnits[medicine.medicineName] = units[0];
            }
          });
        }
      },
      error: (err) => {
        this.notify.error('Could not load medicines');
        console.error('Error loading medicines:', err);
      }
    });
  }
  getDosageOptions(medicineName: string): any[] {
    if (!medicineName || !this.medicineDosageOptions[medicineName]) return [];

    return this.medicineDosageOptions[medicineName].map(unit => ({
      label: unit,
      value: unit
    }));
  }
  onMedicineChange(item: any, index: number) {
    const selected = this.medicineOptions.find(m => m.value === item.medicineId);
    if (selected) {
      item.medicineName = selected.name;

      // Set default dosage
      if (this.medicineDosageOptions[selected.name]) {
        item.dosage = this.selectedMedicineUnits[selected.name];
      } else {
        item.dosage = '';
      }
    }
  }
  LoadEmergencyCases() {
    this._emergencyService.getAll('', 0, 1000).subscribe({
      next: (res) => {
        this.emergencyCase = res.items;
      },
      error: (err) => {
      }
    });
  }
  LoadLabReports() {
    this._labService.getAllTestByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.labTests = res.items.map(item => ({
          id: item.id,
          name: item.reportType
        }));
      },
      error: (err) => {
        this.notify.error('Could not load lab tests');
      }
    });
  }
  addItem(): void {
    const item = new CreateUpdatePrescriptionItemDto();
    item.init({
      id: 0,
      tenantId: abp.session.tenantId,
      medicineName: '',
      dosage: '',
      frequency: '',
      duration: '',
      instructions: '',
      prescriptionId: 0,
      medicineId: 0
    });

    // Add our custom properties
    (item as any).durationValue = 1;
    (item as any).durationUnit = 'Days';

    if (!this.prescription.items) {
      this.prescription.items = [];
    }

    this.prescription.items.push(item);
  }
  removeItem(index: number): void {
    this.prescription.items.splice(index, 1);
  }
  FetchDoctorID() {
    this._doctorService.getDoctorDetailsByAbpUserID(abp.session.userId).subscribe({
      next: (res) => {
        this.doctorID = res.id;
      }, error: (err) => {
      }
    });
  }
  isSaveDisabled(): boolean {
    if (!this.emergencyPrescriptionForm.valid || this.saving) {
      return true;
    }
    return this.prescription.items.some(item =>
      !item.medicineName?.trim() ||
      !item.dosage?.trim() ||
      !item.frequency?.trim() ||
      !item.instructions?.trim()
    );
  }
  save(): void {
    if (this.id > 0) {
      this.Edit();
    }else{
      this.Create();
    }
  }
  loadPrescription(): void {
    this._prescriptionService.getPrescriptionDetailsById(this.id).subscribe({
      next: (result) => {
        this.prescription = {
          id: result.id,
          tenantId: result.tenantId,
          diagnosis: result.diagnosis,
          notes: result.notes,
          issueDate: result.issueDate,
          isFollowUpRequired: result.isFollowUpRequired,
          emergencyCaseId: result.emergencyCaseId,
          doctorId: result.doctorId,
          patientId: result.patientId,
          labTestIds: result.labTestIds || [],
          items: result.items.map(i => {
            const durationParts = i.duration?.split(' ') || ['', ''];
            return {
              ...new CreateUpdatePrescriptionItemDto(),
              id: i.id,
              tenantId: i.tenantId,
              medicineName: i.medicineName,
              medicineId: i.medicineId,
              dosage: i.dosage,
              frequency: i.frequency,
              duration: i.duration,
              durationValue: durationParts[0] ? parseInt(durationParts[0]) : 1,
              durationUnit: durationParts[1] || 'Days',
              instructions: i.instructions,
              prescriptionId: i.prescriptionId
            };
          })
        };

        this.selectedLabTests = this.prescription.labTestIds
          .map(id => this.labTests.find(test => test.id === id))
          .filter(test => test !== undefined);

        this.LoadEmergencyCases();
      },
      error: (err) => {
        this.notify.error('Could not load prescription details');
      }
    });
  }
  loadLabTestsAndPrescription(): void {
    this._labService.getAllTestByTenantID(abp.session.tenantId).subscribe({
      next: (labRes) => {
        this.labTests = labRes.items.map(item => ({
          id: item.id,
          name: item.reportType,
          value: item.id // Add value property for PrimeNG
        }));
        this.loadPrescription();
      },
      error: (labErr) => {
        this.notify.error('Could not load lab tests');
        this.loadPrescription();
      }
    });
  }
  Edit() {
    this.saving = true;

    const input = new CreateUpdatePrescriptionDto();
    input.init({
      id: this.prescription.id,
      tenantId: this.prescription.tenantId,
      diagnosis: this.prescription.diagnosis,
      notes: this.prescription.notes,
      issueDate: this.prescription.issueDate,
      isFollowUpRequired: this.prescription.isFollowUpRequired,
      appointmentId: null,
      doctorId: this.doctorID ? this.doctorID : null,
      patientId: this.prescription.patientId > 0 ? this.prescription.patientId : null,
      labTestIds: this.selectedLabTests.map(test => test.id),
      emergencyCaseId: this.prescription.emergencyCaseId,
      isEmergencyPrescription:true
    });

    input.items = this.prescription.items.map(item => {
      const dtoItem = new CreateUpdatePrescriptionItemDto();
      dtoItem.init({
        id: item.id,
        tenantId: abp.session.tenantId,
        medicineName: item.medicineName,
        medicineId: item.medicineId,
        dosage: item.dosage,
        frequency: item.frequency,
        duration: `${item.durationValue} ${item.durationUnit}`,
        instructions: item.instructions,
        prescriptionId: this.prescription.id
      });
      return dtoItem;
    });
    this._prescriptionService.updatePrescriptionWithItem(input).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (err) => {
        this.saving = false;
        this.notify.error('Could not update prescription');
      }
    });
  }
  Create() {
    this.saving = true;
    const input = new CreateUpdatePrescriptionDto();
    input.init({
      tenantId: this.prescription.tenantId,
      diagnosis: this.prescription.diagnosis,
      notes: this.prescription.notes,
      issueDate: this.prescription.issueDate,
      isFollowUpRequired: this.prescription.isFollowUpRequired,
      appointmentId: null,
      doctorId: this.doctorID ? this.doctorID : null,
      patientId: this.prescription.patientId > 0 ? this.prescription.patientId : null,
      labTestIds: this.selectedLabTests.map(test => test.id || test),
      emergencyCaseId: this.prescription.emergencyCaseId,
      isEmergencyPrescription:true
    });
    input.items = this.prescription.items.map(item => {
      const dtoItem = new CreateUpdatePrescriptionItemDto();
      dtoItem.init({
        ...item,
        duration: `${(item as any).durationValue} ${(item as any).durationUnit}`,
        medicineId: item.medicineId // <-- Make sure this is included
      });
      return dtoItem;
    });
    this._prescriptionService.createPrescriptionWithItem(input).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (err) => {
        this.saving = false;
        this.notify.error('Could not save prescription');
      },
      complete: () => {
        this.saving = false;
      }
    });
  }
}