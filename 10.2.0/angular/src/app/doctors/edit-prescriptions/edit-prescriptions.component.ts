import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { CommonModule } from '@angular/common';
import { AppointmentDto, AppointmentServiceProxy, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionItemDto, DoctorServiceProxy, PatientDto, PatientServiceProxy, PrescriptionServiceProxy, PharmacistInventoryServiceProxy, PatientDropDownDto, LabReportsTypeServiceProxy, DoctorDto, EmergencyProcedureServiceProxy, CreateUpdateSelectedEmergencyProceduresDto, MedicineFormMasterServiceProxy, MedicineMasterServiceProxy, MedicineSearchInputDto } from '@shared/service-proxies/service-proxies';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import moment from 'moment';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MultiSelectModule } from 'primeng/multiselect';
import { FieldsetModule } from 'primeng/fieldset';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { TooltipModule } from 'primeng/tooltip';
@Component({
  selector: 'app-edit-prescriptions',
  standalone: true,
  imports: [
    FormsModule, AbpModalHeaderComponent,
    AbpModalFooterComponent, CommonModule,
    CalendarModule, DropdownModule, CheckboxModule, FieldsetModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AutoCompleteModule, MultiSelectModule, TooltipModule
  ],
  templateUrl: './edit-prescriptions.component.html',
  styleUrls: ['./edit-prescriptions.component.css'],
  providers: [PrescriptionServiceProxy, MedicineFormMasterServiceProxy, MedicineMasterServiceProxy, EmergencyProcedureServiceProxy, DoctorServiceProxy, LabReportsTypeServiceProxy, PatientServiceProxy, AppointmentServiceProxy, PharmacistInventoryServiceProxy],
})
export class EditPrescriptionsComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @ViewChild('editPrescriptionForm', { static: true }) editPrescriptionForm: NgForm;

  medicineForms: any[] = [];
  medicineCache: { [formId: number]: any[] } = {};

  id: number;
  saving = false;
  prescription: any = {
    items: []
  };
  patients: PatientDropDownDto[] = [];
  appointmentTitle: { id: number, title: string }[] = [];
  doctorID!: number;
  labTests: any[] = [];
  selectedLabTests: any[] = [];

  // Medicine and Dosage related properties
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};
  selectedMedicineUnits: { [medicineName: string]: string } = {};
  selectedProcedures: number[] = [];
  procedures: any[] = [];


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

  symptomTimer: any;
  suggestionData: any;
  showSuggestionPopup = false;
  filteredMedicines: any[] = [];
  searchTimeout: any;
  skipCount = 0;
  maxResultCount = 20;

  frequencyInput: string = '';
  numberOfMedicine: number = 1;
  editIndex: number = -1;
  selectedMedicine: any;

  doctors!: DoctorDto[];
  isAdmin = false;
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _prescriptionService: PrescriptionServiceProxy,
    private _patientService: PatientServiceProxy,
    private _appointmentService: AppointmentServiceProxy,
    private _labService: LabReportsTypeServiceProxy,
    private _pharmacistInventoryService: PharmacistInventoryServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _procedureService: EmergencyProcedureServiceProxy,
    private _medicineFormService: MedicineFormMasterServiceProxy, // NEW
    private _medicineMasterService: MedicineMasterServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.GetLoggedInUserRole();
    this.loadInitialData();
  }
  loadInitialData(): void {
    this.LoadPatients();
    this.loadLabTestsAndPrescription();
    this.FetchDoctorID();
    this.loadProcedures();
    this.loadDoctors();
  }
  loadDoctors() {
    this._doctorService.getAllDoctors().subscribe(res => {
      this.doctors = res.items;
      this.cd.detectChanges();
    });
  }
  FetchDoctorID() {
    this._doctorService.getDoctorDetailsByAbpUserID(abp.session.userId).subscribe({
      next: (res) => {
        this.doctorID = res.id;
      }, error: (err) => {
      }
    });
  }

  // Update the loadPrescription method to handle medicine forms:
  loadPrescription(): void {
    this._prescriptionService.getPrescriptionDetailsById(this.id).subscribe({
      next: (result) => {
        this.prescription = {
          id: result.id,
          tenantId: result.tenantId,
          diagnosis: result.diagnosis,
          symptoms: result.symptoms,
          notes: result.notes,
          issueDate: result.issueDate,
          isFollowUpRequired: result.isFollowUpRequired,
          appointmentId: result.appointmentId,
          doctorId: result.doctorId,
          patientId: result.patientId,
          labTestIds: result.labTestIds || [],
          items: result.items.map(i => {

            return {
              ...new CreateUpdatePrescriptionItemDto(),
              id: i.id,
              tenantId: i.tenantId,
              medicineName: i.medicineName,
              numberOfMedicine: i.numberOfMedicine,
              medicineId: i.medicineId,
              frequency: i.frequency,
              prescriptionId: i.prescriptionId,
              filteredMedicines: [] // Initialize empty array
            };
          })
        };

        this.selectedProcedures = result.emergencyProcedures?.map(p => p.emergencyProcedureId) || [];
        this.selectedLabTests = this.prescription.labTestIds
          .map(id => this.labTests.find(test => test.id === id))
          .filter(test => test !== undefined);

        // Load medicines for each item's form and preselect the medicine
        this.prescription.items.forEach(item => {
          if (item.medicineFormId) {
            // Load medicines for this form
            this._medicineMasterService.getMedicinesByFormId(item.medicineFormId).subscribe({
              next: (res) => {
                const medicines = (res || []).map((it: any) => ({
                  label: it.medicineName,
                  value: it.id,
                  dosageOption: it.dosageOption
                }));

                // Cache the medicines for this form
                this.medicineCache[item.medicineFormId] = medicines;

                // Set the filtered medicines for this item
                item.filteredMedicines = medicines;

                // Find and select the correct medicine
                const selectedMedicine = medicines.find(m => m.value === item.medicineId);
                if (selectedMedicine) {
                  item.medicineName = selectedMedicine.label;
                  item.dosage = selectedMedicine.dosageOption;
                }

                this.cd.detectChanges();
              },
              error: (err) => {
                this.notify.error('Could not load medicines for selected type');
                console.error('Error loading medicines by form:', err);
              }
            });
          }
        });

        this.loadAppointmentDetails();
        this.cd.detectChanges();
      },
      error: (err) => {
        this.notify.error('Could not load prescription details');
      }
    });
  }

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
          this.suggestionData = res;
          this.showSuggestionPopup = true;
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
        ...new CreateUpdatePrescriptionItemDto(),
        ...item,
        id: 0, // treating as new items from suggestion
        tenantId: abp.session.tenantId,
        prescriptionId: this.id,
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
    return parseInt(duration.split(' ')[0]) || 1;
  }

  extractUnit(duration: string): string {
    if (!duration) return 'Days';
    const parts = duration.split(' ');
    return parts.length > 1 ? parts[1] : 'Days';
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

        // Auto select correct medicine
        const selected = medicines.find(m => m.value === item.medicineId);
        if (selected) {
          item.medicineName = selected.label;
          item.dosage = selected.dosageOption;
        }

        this.cd.detectChanges();
      }
    });
  }

  searchMedicine(event: any) {
    clearTimeout(this.searchTimeout);

    this.searchTimeout = setTimeout(() => {
      this.skipCount = 0;

      const searchInput = new MedicineSearchInputDto();
      searchInput.keyword = event.query;
      searchInput.skipCount = this.skipCount;
      searchInput.maxResultCount = this.maxResultCount;

      this._medicineMasterService.searchMedicinesWithPaging(searchInput).subscribe(res => {
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
      ...new CreateUpdatePrescriptionItemDto(),
      id: 0,
      tenantId: abp.session.tenantId,
      medicineId: this.selectedMedicine.id,
      medicineName: this.selectedMedicine.medicineName,
      frequency: this.frequencyInput,
      numberOfMedicine: this.numberOfMedicine,
      dosage: this.selectedMedicine.dosageOption,
      instructions: '',
      durationValue: 1,
      durationUnit: 'Days',
      prescriptionId: this.id
    };

    if (this.editIndex >= 0) {
      const existing = this.prescription.items[this.editIndex];
      item.id = existing.id; // Preserve ID if editing
      this.prescription.items[this.editIndex] = item;
      this.editIndex = -1;
    } else {
      this.prescription.items.push(item);
    }

    this.selectedMedicine = null;
    this.frequencyInput = '';
    this.numberOfMedicine = 1;
  }

  editItem(index: number) {
    const item = this.prescription.items[index];
    this.selectedMedicine = {
      id: item.medicineId,
      medicineName: item.medicineName,
      dosageOption: item.dosage
    };
    this.frequencyInput = item.frequency;
    this.numberOfMedicine = item.numberOfMedicine;
    this.editIndex = index;
  }

  deleteItem(index: number) {
    this.prescription.items.splice(index, 1);
  }
  loadLabTestsAndPrescription(): void {
    // First load all available lab tests
    this._labService.getAllTestByTenantID(abp.session.tenantId).subscribe({
      next: (labRes) => {
        // Transform lab test data to include both id and name
        this.labTests = labRes.items.map(item => ({
          id: item.id,
          name: item.reportType,
          value: item.id // Add value property for PrimeNG
        }));

        // Then load the prescription
        this.loadPrescription();
      },
      error: (labErr) => {
        this.notify.error('Could not load lab tests');
        this.loadPrescription();
      }
    });
  }



  loadAppointmentDetails(): void {
    if (!this.prescription.appointmentId) return;

    this._appointmentService.get(this.prescription.appointmentId).subscribe({
      next: (app) => {
        const selectedPatient = this.patients.find(p => p.id === this.prescription.patientId);
        const title = `${app.appointmentDate} - ${selectedPatient?.fullName || ''}`;
        this.appointmentTitle = [{ id: app.id, title: title }];
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error('Error loading appointment:', err);
      }
    });
  }
  loadProcedures() {
    this._procedureService.getEmergencyProcedureList().subscribe(res => {
      this.procedures = res.map(p => ({ label: p.name, value: p.id }));
    });
  }
  LoadPatients(): void {
    this._patientService.patientDropDown().subscribe({
      next: (res) => {
        this.patients = res;
      },
      error: (err) => {
        console.error('Error loading patients:', err);
      }
    });
  }


  isSaveDisabled(): boolean {
    if (!this.editPrescriptionForm.valid || this.saving) {
      return true;
    }
    if (!this.prescription.items || this.prescription.items.length === 0) {
      return true;
    }

  }
  isPatientAdmitted(): boolean {
    const patient = this.patients.find(p => p.id === this.prescription.patientId);
    return !!patient?.isAdmitted;
  }
  save(): void {
    if (!this.prescription.doctorId && this.isAdmin) {
      return;
    }
    this.saving = true;

    const input = new CreateUpdatePrescriptionDto();
    input.init({
      id: this.prescription.id,
      tenantId: this.prescription.tenantId,
      symptoms: this.prescription.symptoms,
      diagnosis: this.prescription.diagnosis,
      notes: this.prescription.notes,
      issueDate: this.prescription.issueDate,
      isFollowUpRequired: this.prescription.isFollowUpRequired,
      appointmentId: this.prescription.appointmentId > 0 ? this.prescription.appointmentId : undefined,
      doctorId: this.isAdmin ? this.prescription.doctorId : this.doctorID,
      patientId: this.prescription.patientId,
      labTestIds: this.selectedLabTests.map(test => test.id)
    });

    input.items = this.prescription.items.map(item => {
      const dtoItem = new CreateUpdatePrescriptionItemDto();
      dtoItem.init({
        id: item.id,
        tenantId: abp.session.tenantId,
        medicineName: item.medicineName,
        medicineId: item.medicineId,
        frequency: item.frequency,
        numberOfMedicine: item.numberOfMedicine,
        duration: `${item.durationValue} ${item.durationUnit}`,
        instructions: item.instructions,
        prescriptionId: this.prescription.id,
        isPrescribe: true
      });
      return dtoItem;
    });

    input.emergencyProcedures = this.selectedProcedures.map(procId => {
      const dto = new CreateUpdateSelectedEmergencyProceduresDto();
      dto.init({
        tenantId: this.prescription.tenantId,
        prescriptionId: this.prescription.id,
        emergencyProcedureId: procId
      });
      return dto;
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
