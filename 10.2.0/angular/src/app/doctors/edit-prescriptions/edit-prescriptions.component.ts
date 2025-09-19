import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { CommonModule } from '@angular/common';
import { AppointmentDto, AppointmentServiceProxy, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionItemDto, DoctorServiceProxy, PatientDto, PatientServiceProxy, PrescriptionServiceProxy, PharmacistInventoryServiceProxy, PatientDropDownDto, LabReportsTypeServiceProxy, DoctorDto, EmergencyProcedureServiceProxy, CreateUpdateSelectedEmergencyProceduresDto, MedicineFormMasterServiceProxy, MedicineMasterServiceProxy } from '@shared/service-proxies/service-proxies';
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

@Component({
  selector: 'app-edit-prescriptions',
  standalone: true,
  imports: [
    FormsModule, AbpModalHeaderComponent,
    AbpModalFooterComponent, CommonModule,
    CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent,
    MultiSelectModule
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
    this.loadMedicineForms();
  }
  loadInitialData(): void {
    this.LoadPatients();
    this.loadLabTestsAndPrescription();
    this.FetchDoctorID();
    this.loadProcedures();
    this.loadDoctors();
  }
  loadDoctors() {
    this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId).subscribe(res => {
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
  loadMedicineForms() {
    this._medicineFormService.getAlldicineFormByTenantId(abp.session.tenantId).subscribe({
      next: (res: any) => {
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

  // Add these methods for medicine form handling
  onMedicineFormChange(item: any, index: number) {
    const formId = item.medicineFormId;
    if (!formId) {
      item.filteredMedicines = [];
      item.medicineId = 0;
      item.medicineName = '';
      item.dosage = '';
      return;
    }

    // Check cache first
    if (this.medicineCache[formId]) {
      item.filteredMedicines = this.medicineCache[formId];
      this.cd.detectChanges();
      return;
    }

    this._medicineMasterService.getMedicinesByFormId(formId).subscribe({
      next: (res) => {
        const medicines = (res || []).map((it: any) => ({
          label: it.medicineName,
          value: it.id,
          dosageOption: it.dosageOption
        }));

        this.medicineCache[formId] = medicines;
        item.filteredMedicines = medicines;

        // Reset medicine & dosage
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
      item.dosage = selected.dosageOption;
    } else {
      item.medicineName = '';
      item.dosage = '';
    }
  }

  // Update the loadPrescription method to handle medicine forms:
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
        appointmentId: result.appointmentId,
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
            medicineFormId: i.medicineFormId,
            dosage: i.dosage,
            frequency: i.frequency,
            duration: i.duration,
            durationValue: durationParts[0] ? parseInt(durationParts[0]) : 1,
            durationUnit: durationParts[1] || 'Days',
            instructions: i.instructions,
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

  //   loadMedicines(): void {
  //     this._pharmacistInventoryService.getAll(
  //       undefined, undefined, undefined, undefined,
  //       undefined,  true, undefined, undefined
  //     ).subscribe({
  //       next: (res) => {
  //         if (res.items && res.items.length > 0) {
  //           this.medicineOptions = res.items.map(item => ({
  //             label: item.medicineName,
  //             value: item.id, // Use medicineId as value
  //             name: item.medicineName // Store name separately
  //           }));


  //           res.items.forEach(medicine => {
  //             if (medicine.unit) {
  //               const units = medicine.unit.split(',').map(u => u.trim());
  //               this.medicineDosageOptions[medicine.medicineName] = units;
  //               this.selectedMedicineUnits[medicine.medicineName] = units[0];
  //             }
  //           });
  //         }
  //       },
  //       error: (err) => {
  //         this.notify.error('Could not load medicines');
  //       }
  //     });
  //   }

  //   getDosageOptions(medicineName: string): any[] {
  //     if (!medicineName || !this.medicineDosageOptions[medicineName]) return [];
  //     return this.medicineDosageOptions[medicineName].map(unit => ({
  //       label: unit,
  //       value: unit
  //     }));
  //   }

  //   // onMedicineChange(item: any, index: number): void {
  //   //   const selectedMedicine = item.medicineName;
  //   //   if (selectedMedicine && this.medicineDosageOptions[selectedMedicine]) {
  //   //     item.dosage = this.selectedMedicineUnits[selectedMedicine];
  //   //   } else {
  //   //     item.dosage = '';
  //   //   }
  //   // }
  //   onMedicineChange(item: any, index: number) {
  //   const selected = this.medicineOptions.find(m => m.value === item.medicineId);
  //   if (selected) {
  //     item.medicineName = selected.name;

  //     // Set default dosage
  //     if (this.medicineDosageOptions[selected.name]) {
  //       item.dosage = this.selectedMedicineUnits[selected.name];
  //     } else {
  //       item.dosage = '';
  //     }
  //   }
  // }

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



  addItem(): void {
    const item: any = {
      ...new CreateUpdatePrescriptionItemDto(),
      id: 0,
      tenantId: abp.session.tenantId,
      medicineName: '',
      medicineId: 0,
      medicineFormId: 0,
      dosage: '',
      frequency: '',
      duration: '',
      durationValue: 1,
      durationUnit: 'Days',
      instructions: '',
      prescriptionId: this.id,
      filteredMedicines: []
    };

    if (!this.prescription.items) {
      this.prescription.items = [];
    }

    this.prescription.items.push(item);
  }

  removeItem(index: number): void {
    this.prescription.items.splice(index, 1);
  }

  isSaveDisabled(): boolean {
    if (!this.editPrescriptionForm.valid || this.saving) {
      return true;
    }
    // if (!this.prescription.items || this.prescription.items.length === 0) {
    //   return true;
    // }

    return this.prescription.items.some(item =>
      !item.medicineName?.trim() ||
      !item.dosage?.trim() ||
      !item.frequency?.trim() ||
      !item.instructions?.trim()
    );
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
        medicineFormId: item.medicineFormId,
        dosage: item.dosage,
        frequency: item.frequency,
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
