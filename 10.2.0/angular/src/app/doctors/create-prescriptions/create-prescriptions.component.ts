import { ChangeDetectorRef, Component, Injector, OnInit, Output, ViewChild, EventEmitter, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { AppointmentServiceProxy, CreateUpdatePrescriptionItemDto, LabReportsTypeServiceProxy, PharmacistInventoryDtoPagedResultDto, PharmacistInventoryServiceProxy, PrescriptionItemDto, PrescriptionServiceProxy, PatientDropDownDto, EmergencyProcedureServiceProxy, EmergencyProcedureDto, CreateUpdateSelectedEmergencyProceduresDto, MedicineFormMasterServiceProxy, MedicineMasterServiceProxy, MedicineSearchInputDto } from '@shared/service-proxies/service-proxies';
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
import { InputText } from 'primeng/inputtext';
import { AppSessionService } from '@shared/session/app-session.service';
import { MultiSelectModule } from 'primeng/multiselect';
import { PermissionCheckerService } from '@node_modules/abp-ng2-module';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { FieldsetModule } from 'primeng/fieldset';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { TooltipModule } from 'primeng/tooltip';
@Component({
  selector: 'app-create-prescriptions',
  standalone: true,
  imports: [
    FormsModule, CalendarModule, DropdownModule, FieldsetModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AutoCompleteModule, AbpModalHeaderComponent, AbpModalFooterComponent, MultiSelectModule,
    TooltipModule
  ],
  templateUrl: './create-prescriptions.component.html',
  styleUrls: ['./create-prescriptions.component.css'],
  providers: [DoctorServiceProxy, EmergencyProcedureServiceProxy, MedicineMasterServiceProxy, MedicineFormMasterServiceProxy, PatientServiceProxy, PharmacistInventoryServiceProxy, LabReportsTypeServiceProxy, AppointmentServiceProxy, AppSessionService, PrescriptionServiceProxy]
})
export class CreatePrescriptionsComponent extends AppComponentBase implements OnInit {
  @ViewChild('prescriptionForm', { static: true }) prescriptionForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  @Input() selectedPatient: PatientDto | undefined;   // <-- patient preselect
  @Input() dischargePatientID: number = 0;

  saving = false;
  doctorID!: number;
  showAddPatientButton = false;
  isAdmin = false;

  patients!: PatientDropDownDto[];
  appointments!: AppointmentDto[];
  doctors!: DoctorDto[];

  procedures: any[] = [];
  selectedProcedures: number[] = [];

  labTests: any[] = [];
  selectedLabTests: any[] = [];
  selectedMedicine: any;
  filteredMedicines: any[] = [];
  frequencyInput: string = '';
  numberOfMedicine: number = 1;
  searchTimeout: any;
  skipCount = 0;
  maxResultCount = 20;
  lastQuery = '';
  loading = false;
  editIndex: number = -1;
  // Medicine and Dosage related properties
  medicineForms: any[] = [];      // [{ label, value }]
  medicineDosageOptions: any = {}; // quick lookup
  medicineOptions: any[] = [];    // all medicines (not mandatory, per item filter hoga)

  // selectedMedicineUnits: { [medicineName: string]: string } = {};


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
    labTestIds: []
  };

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _doctorService: DoctorServiceProxy,
    private _procedureService: EmergencyProcedureServiceProxy,
    private _patientService: PatientServiceProxy,
    private _appointmentService: AppointmentServiceProxy,
    private _sessionService: AppSessionService,
    private _prescriptionService: PrescriptionServiceProxy,
    private _labService: LabReportsTypeServiceProxy,
    private _pharmacistInventoryService: PharmacistInventoryServiceProxy,
    private permissionChecker: PermissionCheckerService,
    private _modalService: BsModalService,
    private _medicineMasterService: MedicineMasterServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.showAddPatientButton = this.permissionChecker.isGranted('Pages.Users');
    this.GetLoggedInUserRole();
    this.FetchDoctorID();
    this.loadDoctors();
    this.LoadPatients();
    this.LoadLabReports();
    this.searchMedicine(event);
    //this.loadMedicineForms();
    // this.loadMedicines();
    this.loadProcedures();
    if (this.selectedPatient) {
      this.prescription.patientId = this.selectedPatient.id;

      if (this.selectedPatient.isAdmitted) {
        this.prescription.appointmentId = null;   // no appointment
      }
    }
    if (this.dischargePatientID > 0) {
      this.prescription.patientId = this.dischargePatientID;
      this.prescription.appointmentId = null;
      this.showAddPatientButton = false;
    }
  }


  loadProcedures() {
    this._procedureService.getEmergencyProcedureList().subscribe(res => {
      this.procedures = res.map(x => ({
        label: x.name,
        value: x.id
      }));
    });
  }
  loadDoctors() {
    this._doctorService.getAllDoctors().subscribe(res => {
      this.doctors = res.items;
      this.cd.detectChanges();
    });
  }


  LoadPatients() {
    this._patientService.patientDropDown().subscribe({
      next: (res) => {
        this.patients = res;
      }, error: (err) => {
      }
    });
  }
  isPatientAdmitted(): boolean {
    if (!this.prescription.patientId) return false;
    const patient = this.patients?.find(p => p.id === this.prescription.patientId);
    return patient?.isAdmitted === true;
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

  searchMedicine(event: any) {
    clearTimeout(this.searchTimeout);

    this.searchTimeout = setTimeout(() => {

      this.skipCount = 0;
      this.lastQuery = event.query;

      const input = new MedicineSearchInputDto();
      input.keyword = event.query;
      input.skipCount = this.skipCount;
      input.maxResultCount = this.maxResultCount;

      this._medicineMasterService
        .searchMedicinesWithPaging(input)
        .subscribe(res => {
          this.filteredMedicines = res.items || [];
        });

    }, 300);
  }

  formatFrequency() {
    if (!this.frequencyInput) return;

    // 1. Remove all non-digits (old hyphens etc.)
    let val = this.frequencyInput.replace(/[^0-9]/g, '');

    // 2. Limit to max 4 digits
    val = val.substring(0, 4);

    // 3. Format with hyphens (12 -> 1-2)
    this.frequencyInput = val.split('').join('-');
  }

  addMedicineToList() {

    if (!this.selectedMedicine || !this.frequencyInput || !this.numberOfMedicine) {
      this.notify.warn("Fill all fields");
      return;
    }

    const item = {
      medicineId: this.selectedMedicine.id,
      medicineName: this.selectedMedicine.medicineName,
      frequency: this.frequencyInput,
      numberOfMedicine: this.numberOfMedicine,
      dosage: this.selectedMedicine.dosageOption,
      instructions: '',
      duration: '1 Days'
    };

    // Edit mode
    if (this.editIndex >= 0) {
      this.prescription.items[this.editIndex] = item;
      this.editIndex = -1;
    } else {
      this.prescription.items.push(item);
    }

    // reset
    this.selectedMedicine = null;
    this.frequencyInput = '';
    this.numberOfMedicine = 1;
  }

  editItem(index: number) {
    const item = this.prescription.items[index];

    this.selectedMedicine = {
      id: item.medicineId,
      medicineName: item.medicineName
    };

    this.frequencyInput = item.frequency;
    this.numberOfMedicine = item.numberOfMedicine;

    this.editIndex = index;
  }

  deleteItem(index: number) {
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

    if (!this.prescriptionForm.valid || this.saving) {
      return true;
    }

    // if (!this.prescription.items || this.prescription.items.length === 0) {
    //   return true;
    // }

  }

  save(): void {
    if (!this.prescription.doctorId && this.isAdmin) {
      return;
    }
    this.saving = true;

    const input = new CreateUpdatePrescriptionDto();
    input.init({
      tenantId: this.prescription.tenantId,
      diagnosis: this.prescription.diagnosis,
      symptoms: this.prescription.symptoms,
      notes: this.prescription.notes,
      issueDate: this.prescription.issueDate,
      isFollowUpRequired: this.prescription.isFollowUpRequired,
      appointmentId: this.prescription.appointmentId > 0 ? this.prescription.appointmentId : undefined,
      doctorId: this.isAdmin
        ? (this.prescription.doctorId > 0 ? this.prescription.doctorId : undefined)
        : this.doctorID,
      patientId: this.prescription.patientId > 0 ? this.prescription.patientId : undefined,
      labTestIds: this.selectedLabTests
        .filter(test => (test.id || test) > 0) // sirf valid id bhej
        .map(test => test.id || test)
    });

    // Items mapping
    input.items = this.prescription.items
      .filter(item => item.medicineId > 0) // sirf medicineId > 0 hone wale bhej
      .map(item => {
        debugger
        const dtoItem = new CreateUpdatePrescriptionItemDto();
        dtoItem.init({
          ...item,
          numberOfMedicine: item.numberOfMedicine,
          duration: `${(item as any).durationValue} ${(item as any).durationUnit}`,
          medicineId: item.medicineId,
          isPrescribe: true
        });
        return dtoItem;
      });

    // Emergency Procedures mapping
    input.emergencyProcedures = this.selectedProcedures
      .filter(procId => procId > 0) // sirf valid id bhej
      .map(procId => {
        const dto = new CreateUpdateSelectedEmergencyProceduresDto();
        dto.init({
          tenantId: this.prescription.tenantId,
          prescriptionId: 0, // backend set karega
          emergencyProcedureId: procId
        });
        return dto;
      });
    debugger
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

  showCreatePatientDialog(id?: number): void {
    let createOrEditPatientDialog: BsModalRef;
    createOrEditPatientDialog = this._modalService.show(CreateUserDialogComponent, {
      class: 'modal-lg',
      initialState: {
        defaultRole: 'Patient',
        disableRoleSelection: true
      }
    });
    createOrEditPatientDialog.content.onSave.subscribe(() => {
      this.LoadPatients();
    });
  }
  GetLoggedInUserRole() {
    this._prescriptionService.getCurrentUserRoles().subscribe(res => {
      if (res && Array.isArray(res)) {
        if (res.includes('Admin')) {
          this.isAdmin = true;
        }
      }
      this.cd.detectChanges();
    });
  }

  symptomTimer: any;
  suggestionData: any;
  showSuggestionPopup = false;

  onSymptomsChange() {
    clearTimeout(this.symptomTimer);

    this.symptomTimer = setTimeout(() => {
      if (this.prescription.symptoms?.length > 3) {
        this.getSmartSuggestion();
      }
    }, 600);
  }

  getSmartSuggestion() {
    this._prescriptionService.getSmartSuggestionBySymptoms(this.prescription.symptoms)
      .subscribe(res => {
        if (res) {
          this.suggestionData = res
          if (this.suggestionData.length > 0) {
            this.showSuggestionPopup = true;
          }
        }
      });
  }
  applySuggestion() {

    if (!confirm('This will replace current data. Continue?')) {
      return;
    }
    const data = this.suggestionData;

    // Diagnosis & Notes
    this.prescription.symptoms = data.symptoms;
    this.prescription.diagnosis = data.diagnosis;
    this.prescription.notes = data.notes;

    // Lab Tests
    this.selectedLabTests = data.labTestIds;

    // Procedures
    this.selectedProcedures = data.emergencyProcedures.map(x => x.emergencyProcedureId);

    // Medicines
    this.prescription.items = data.items.map(item => {
      return {
        ...item,
        durationValue: this.extractNumber(item.duration),
        durationUnit: this.extractUnit(item.duration),
        filteredMedicines: []
      };
    });
    this.prescription.items.forEach((item, index) => {
      this.loadMedicinesForItem(item, index);
    });
    this.showSuggestionPopup = false;
  }

  extractNumber(duration: string): number {
    if (!duration) return 1;
    return parseInt(duration.split(' ')[0]);
  }

  extractUnit(duration: string): string {
    if (!duration) return 'Days';
    return duration.split(' ')[1];
  }

  loadMedicinesForItem(item: any, index: number) {

    if (!item.medicineFormId) return;

    this._medicineMasterService.getMedicinesByFormId(item.medicineFormId).subscribe({
      next: (res) => {

        const medicines = (res || []).map((it: any) => ({
          label: it.medicineName,
          value: it.id,
          dosageOption: it.dosageOption
        }));

        item.filteredMedicines = medicines;

        // 🔥 Auto select correct medicine
        const selected = medicines.find(m => m.value === item.medicineId);
        if (selected) {
          item.medicineName = selected.label;
          item.dosage = selected.dosageOption;
        }

        this.cd.detectChanges();
      }
    });
  }
}