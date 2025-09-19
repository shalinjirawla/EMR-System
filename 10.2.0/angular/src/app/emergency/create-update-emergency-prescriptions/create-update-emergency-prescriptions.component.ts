import { ChangeDetectorRef, Component, Injector, OnInit, Output, ViewChild, EventEmitter } from '@angular/core';
import { NgForm } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { AppointmentServiceProxy, CreateUpdatePrescriptionItemDto, LabReportsTypeServiceProxy, PharmacistInventoryDtoPagedResultDto, PharmacistInventoryServiceProxy, PrescriptionItemDto, PrescriptionServiceProxy, PatientDropDownDto, EmergencyServiceProxy, EmergencyCaseDto, DepartmentServiceProxy, DepartmentDto, ProcedureCategory, EmergencyProcedureServiceProxy, EmergencyProcedureDto, CreateUpdateEmergencyProcedureDto, CreateUpdateSelectedEmergencyProceduresDto, CreateUpdateConsultationRequestsDto, Status } from '@shared/service-proxies/service-proxies';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CreateUpdatePrescriptionDto, DoctorDto, DoctorServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { TextareaModule } from 'primeng/textarea';
import { AppSessionService } from '@shared/session/app-session.service';
import { MultiSelectModule } from 'primeng/multiselect';
import { PermissionCheckerService } from '@node_modules/abp-ng2-module';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
@Component({
  selector: 'app-create-update-emergency-prescriptions',
  standalone: true,
  imports: [
    FormsModule, CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent, MultiSelectModule
  ],
  templateUrl: './create-update-emergency-prescriptions.component.html',
  styleUrl: './create-update-emergency-prescriptions.component.css',
  providers: [DoctorServiceProxy, DepartmentServiceProxy, PharmacistInventoryServiceProxy, LabReportsTypeServiceProxy, AppointmentServiceProxy, AppSessionService, PrescriptionServiceProxy, EmergencyServiceProxy, EmergencyProcedureServiceProxy]
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
    specialistDoctorId: 0,
    isSpecialAdviceRequired: false,
    departmentId: 0,
    emergencyProcedures: [],
    createUpdateConsultationRequests: {
    id: 0,
    tenantId: abp.session.tenantId,
    patientId: 0,
    requestingDoctorId: 0,
    requestedSpecialistId: 0,
    status: 0,
    notes: '',
    adviceResponse: ''
  }
  };
  doctors!: DoctorDto[];
  isAdmin!: boolean;
  departmentList!: DepartmentDto[];
  _isSpecialAdviceRequired = false;
  _isSpecialDepartMentSelect = false;
  totalDoctorList!: DoctorDto[];
  departmentWiseDoctor!: DoctorDto[];
  _procedures!: EmergencyProcedureDto[];
  selectedProcedures: number[] = [];
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _doctorService: DoctorServiceProxy,
    private _departmentService: DepartmentServiceProxy,
    private _sessionService: AppSessionService,
    private _prescriptionService: PrescriptionServiceProxy,
    private _labService: LabReportsTypeServiceProxy,
    private _pharmacistInventoryService: PharmacistInventoryServiceProxy,
    private permissionChecker: PermissionCheckerService,
    private _modalService: BsModalService,
    private _emergencyService: EmergencyServiceProxy,
    private _procedureService: EmergencyProcedureServiceProxy,
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.showAddPatientButton = this.permissionChecker.isGranted('Pages.Users');
    this.GetLoggedInUserRole();
    this.loadEmergencyDoctors();
    this.loadDoctors();
    this.FetchDoctorID();
    this.LoadLabReports();
    this.loadMedicines();
    this.LoadEmergencyCases();
    if (this.id > 0) {
      this.loadLabTestsAndPrescription();
    }
    this.loadDepartments();
    this.loadProcedures();
  }
  loadEmergencyDoctors() {
    this._doctorService.getAllEmergencyDoctorsByTenantID(abp.session.tenantId).subscribe(res => {
      this.doctors = res.items;
      this.cd.detectChanges();
    });
  }
  loadDoctors(){
    this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId).subscribe(res=>{
      this.totalDoctorList = res.items;
      this.cd.detectChanges();
    })
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
    } else {
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
          isEmergencyPrescription: result.isEmergencyPrescription,
          departmentId: result.departmentId,
          specialistDoctorId: result.specialistDoctorId,
          isSpecialAdviceRequired: result.isSpecialAdviceRequired,
          createUpdateConsultationRequests:result.createUpdateConsultationRequests?result.createUpdateConsultationRequests:new CreateUpdateConsultationRequestsDto(),
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

        if (this.prescription.isSpecialAdviceRequired) {
          this._isSpecialAdviceRequired = true;
          if (this.prescription.departmentId) {
            this._isSpecialDepartMentSelect = true;

            this.departmentWiseDoctor = this.totalDoctorList.filter(
              x => x.department && x.department.id === this.prescription.departmentId
            );
          }
        }
        this.LoadEmergencyCases();
        this.loadProcedures();
        this.selectedProcedures = result.emergencyProcedures?.map(p => p.emergencyProcedureId) || [];
        this.cd.detectChanges();
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
    if (!this.prescription.doctorId && this.isAdmin) {
      return;
    }
    this.saving = true;
    var fetchPatientId = this.emergencyCase.find(x => x.id === this.prescription.emergencyCaseId)?.patientId;
    const input = new CreateUpdatePrescriptionDto();
    input.init({
      id: this.prescription.id,
      tenantId: this.prescription.tenantId,
      diagnosis: this.prescription.diagnosis,
      notes: this.prescription.notes,
      issueDate: this.prescription.issueDate,
      isFollowUpRequired: this.prescription.isFollowUpRequired,
      appointmentId: null,
      doctorId: this.isAdmin ? this.prescription.doctorId : this.doctorID,
      patientId: fetchPatientId,
      labTestIds: this.selectedLabTests.map(test => test.id),
      emergencyCaseId: this.prescription.emergencyCaseId,
      isEmergencyPrescription: true,
      departmentId: this.prescription.departmentId > 0 ? this.prescription.departmentId : null,
      specialistDoctorId: this.prescription.specialistDoctorId > 0 ? this.prescription.specialistDoctorId : null,
      isSpecialAdviceRequired: this.prescription.isSpecialAdviceRequired,
      emergencyProcedures: this.selectedProcedures.map(id => ({
        emergencyProcedureId: id,
        prescriptionId: this.prescription.id, // only if needed
        tenantId: this.prescription.tenantId
      })),
    });
    if (this.prescription.isSpecialAdviceRequired && this.prescription.specialistDoctorId > 0) {
      const consultationRequests = new CreateUpdateConsultationRequestsDto();
      consultationRequests.id = this.prescription.createUpdateConsultationRequests.id;
      consultationRequests.tenantId = abp.session.tenantId;
      consultationRequests.prescriptionId = this.prescription.id;
      consultationRequests.requestingDoctorId = this.isAdmin ? this.prescription.doctorId : this.doctorID;
      consultationRequests.requestedSpecialistId = this.prescription.specialistDoctorId > 0 ? this.prescription.specialistDoctorId : null;
      consultationRequests.status = Status._0;
      consultationRequests.notes = this.prescription.createUpdateConsultationRequests.notes;
      consultationRequests.adviceResponse = this.prescription.createUpdateConsultationRequests.adviceResponse;

      input.createUpdateConsultationRequests = consultationRequests;
    }
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
        prescriptionId: this.prescription.id,
        isPrescribe:true
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
    if (!this.prescription.doctorId && this.isAdmin) {
      return;
    }
    this.saving = true;
    var fetchPatientId = this.emergencyCase.find(x => x.id === this.prescription.emergencyCaseId)?.patientId;
    const input = new CreateUpdatePrescriptionDto();
    input.init({
      tenantId: this.prescription.tenantId,
      diagnosis: this.prescription.diagnosis,
      notes: this.prescription.notes,
      issueDate: this.prescription.issueDate,
      isFollowUpRequired: this.prescription.isFollowUpRequired,
      appointmentId: null,
      doctorId: this.isAdmin ? this.prescription.doctorId : this.doctorID,
      patientId: fetchPatientId,
      labTestIds: this.selectedLabTests.map(test => test.id || test),
      emergencyCaseId: this.prescription.emergencyCaseId,
      isEmergencyPrescription: true,
      specialistDoctorId: this.prescription.specialistDoctorId > 0 ? this.prescription.specialistDoctorId : null,
      isSpecialAdviceRequired: this.prescription.isSpecialAdviceRequired,
      departmentId: this.prescription.departmentId > 0 ? this.prescription.departmentId : null,
      emergencyProcedures: this.selectedProcedures.map(id => ({
        emergencyProcedureId: id,
        prescriptionId: this.prescription.id, // only if needed
        tenantId: this.prescription.tenantId
      })),
    });
    if (this.prescription.isSpecialAdviceRequired && this.prescription.specialistDoctorId > 0) {
      const consultationRequests = new CreateUpdateConsultationRequestsDto();
      consultationRequests.id = 0;
      consultationRequests.tenantId = abp.session.tenantId;
      consultationRequests.prescriptionId = this.prescription.id;
      consultationRequests.requestingDoctorId = this.isAdmin ? this.prescription.doctorId : this.doctorID;
      consultationRequests.requestedSpecialistId = this.prescription.specialistDoctorId > 0 ? this.prescription.specialistDoctorId : null;
      consultationRequests.status = Status._0;
      consultationRequests.notes = this.prescription.createUpdateConsultationRequests.notes;
      consultationRequests.adviceResponse = this.prescription.createUpdateConsultationRequests.adviceResponse;

      input.createUpdateConsultationRequests = consultationRequests;
    }

    input.items = this.prescription.items.map(item => {
      const dtoItem = new CreateUpdatePrescriptionItemDto();
      dtoItem.init({
        ...item,
        duration: `${(item as any).durationValue} ${(item as any).durationUnit}`,
        medicineId: item.medicineId, // <-- Make sure this is included
        isPrescribe:true
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
  loadDepartments(): void {
    this._departmentService.getAllDepartmentForDoctor().subscribe({
      next: (res: any) => {
        this.departmentList = res.items;
        this.cd.detectChanges();
      },
      error: () => {

      }
    });
  }
  onChangeSpecialAdvice(event: any) {
    this._isSpecialAdviceRequired = event.checked;
    this._isSpecialDepartMentSelect = false;
    this.prescription.departmentId = null;
    this.prescription.specialistDoctorId = null;
  }
  OnSelectDepartMent(event: any) {
    this._isSpecialDepartMentSelect = true;
    this.prescription.specialistDoctorId = 0;
    const departmentId = event.value;
    if (departmentId > 0) {
      this.departmentWiseDoctor = this.totalDoctorList.filter(
        x => x.department && x.department.id === departmentId
      );
    }
  }
  loadProcedures() {
    this._procedureService.getEmergencyProcedureList().subscribe({
      next: (res) => {
        this._procedures = res;
      },
      error: (err) => {

      }
    })
  }
}
