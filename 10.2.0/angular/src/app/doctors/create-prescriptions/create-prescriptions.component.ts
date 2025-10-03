import { ChangeDetectorRef, Component, Injector, OnInit, Output, ViewChild, EventEmitter, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { AppointmentServiceProxy, CreateUpdatePrescriptionItemDto, LabReportsTypeServiceProxy, PharmacistInventoryDtoPagedResultDto, PharmacistInventoryServiceProxy, PrescriptionItemDto, PrescriptionServiceProxy, PatientDropDownDto, EmergencyProcedureServiceProxy, EmergencyProcedureDto, CreateUpdateSelectedEmergencyProceduresDto, MedicineFormMasterServiceProxy, MedicineMasterServiceProxy } from '@shared/service-proxies/service-proxies';
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
@Component({
  selector: 'app-create-prescriptions',
  standalone: true,
  imports: [
    FormsModule, CalendarModule, DropdownModule, InputText, FieldsetModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent, MultiSelectModule
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
  // Medicine and Dosage related properties
  medicineForms: any[] = [];      // [{ label, value }]
  medicineDosageOptions: any = {}; // quick lookup
  medicineOptions: any[] = [];    // all medicines (not mandatory, per item filter hoga)

  // selectedMedicineUnits: { [medicineName: string]: string } = {};
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
    private _medicineFormService: MedicineFormMasterServiceProxy, // NEW
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
    this.loadMedicineForms();
    // this.loadMedicines();
    this.loadProcedures();
    if (this.selectedPatient) {
      this.prescription.patientId = this.selectedPatient.id;

      if (this.selectedPatient.isAdmitted) {
        this.prescription.appointmentId = null;   // no appointment
      } else {
        this.LoadAppoinments();
      }
    }
    if (this.dischargePatientID > 0) {
      this.prescription.patientId = this.dischargePatientID;
      this.prescription.appointmentId = null;
      this.showAddPatientButton = false;
    }
  }
  loadMedicineForms() {
    this._medicineFormService.getAlldicineFormByTenantId(abp.session.tenantId).subscribe({
      next: (res: any) => {
        // res.items will always exist in ListResultDto
        const items: any[] = res.items || [];
        this.medicineForms = items.map((m: any) => ({
          label: m.name,
          value: m.id
        }));
        this.cd.detectChanges();
      },
      error: (err) => {
        this.notify.error('Could not load medicine forms');
        console.error('Error loading medicine forms:', err);
      }
    });
  }

  onMedicineFormChange(item: any, index: number) {
    const formId = item.medicineFormId;
    if (!formId) {
      item.filteredMedicines = [];
      item.medicineId = 0;
      item.medicineName = '';
      item.dosage = '';
      return;
    }

    this._medicineMasterService.getMedicinesByFormId(formId).subscribe({
      next: (res) => {
        const medicines = (res || []).map((it: any) => ({
          label: it.medicineName,
          value: it.id,
          dosageOption: it.dosageOption
        }));

        item.filteredMedicines = medicines;

        // reset medicine & dosage
        item.medicineId = 0;
        item.medicineName = '';
        item.dosage = '';

        this.cd.detectChanges();
      },
      error: (err) => {
        this.notify.error('Could not load medicines for selected type');
        console.error('Error loading medicines by form:', err);
      }
    });
  }

  onMedicineChange(item: any, index: number) {
    const selected = (item.filteredMedicines || []).find((m: any) => m.value === item.medicineId);
    if (selected) {
      item.medicineName = selected.label;
      item.dosage = selected.dosageOption; // ✅ auto-fill dosage
    } else {
      item.medicineName = '';
      item.dosage = '';
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
  // loadMedicines() {
  //   // Call getAll() with default parameters to get all available medicines
  //   this._pharmacistInventoryService.getAll(
  //     undefined,  // keyword
  //     undefined,  // sorting
  //     undefined,  // minStock
  //     undefined,  // maxStock
  //     undefined,  // fromExpiryDate
  //     true,       // isAvailable (only get available medicines)
  //     undefined,  // skipCount
  //     undefined   // maxResultCount
  //   ).subscribe({
  //     next: (res) => {
  //       if (res.items && res.items.length > 0) {
  //         this.medicineOptions = res.items.map(item => ({
  //           label: item.medicineName,
  //           value: item.id, // Use medicineId as value
  //           name: item.medicineName // Store name separately
  //         }));


  //         // Prepare dosage options for each medicine
  //         res.items.forEach(medicine => {
  //           const unit = medicine.unit;
  //           if (unit) {
  //             // Split units if they are comma separated (e.g., "200 mg, 500 mg")
  //             const units = unit.split(',').map(u => u.trim());
  //             this.medicineDosageOptions[medicine.medicineName] = units;
  //             this.selectedMedicineUnits[medicine.medicineName] = units[0];
  //           }
  //         });
  //       }
  //     },
  //     error: (err) => {
  //       this.notify.error('Could not load medicines');
  //       console.error('Error loading medicines:', err);
  //     }
  //   });
  // }
  getDosageOptions(medicineName: string): any[] {
    const dosage = this.medicineDosageOptions[medicineName];
    if (!dosage) return [];
    return [{ label: dosage, value: dosage }];
  }

  // onMedicineChange(item: any, index: number) {
  //   const selected = this.medicineOptions.find(m => m.value === item.medicineId);
  //   if (selected) {
  //     item.medicineName = selected.label;

  //     // Set default dosage directly from dosageOption (single strength)
  //     if (selected.dosageOption) {
  //       item.dosage = selected.dosageOption;
  //     } else {
  //       item.dosage = '';
  //     }
  //   } else {
  //     item.medicineName = '';
  //     item.dosage = '';
  //   }
  // }


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

  LoadAppoinments() {
    const patientId = this.prescription.patientId;
    const doctorId = this.doctorID;

    if (!patientId) return;

    this._appointmentService.getPatientAppointment(patientId, doctorId).subscribe({
      next: (res) => {
        // Filter out completed (status == 2)
        this.appointments = res.items.filter(app => app.status == 0 || app.status == 1 || app.status == 2) ;

      },
      error: (err) => {
        // Handle error if needed
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

    if (!this.prescriptionForm.valid || this.saving) {
      return true;
    }

    if (!this.prescription.items || this.prescription.items.length === 0) {
      return true;
    }

    return this.prescription.items.some(item =>
      !item.medicineName?.trim() ||
      !item.dosage?.trim() ||
      !item.frequency?.trim() ||
      // !item.durationValue ||
      !item.instructions?.trim()
    );
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
        const dtoItem = new CreateUpdatePrescriptionItemDto();
        dtoItem.init({
          ...item,
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
}