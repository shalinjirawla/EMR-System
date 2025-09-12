import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy, CreateUserDto, RoleDto, CreateUpdateDoctorDto, DoctorServiceProxy, NurseServiceProxy, LapTechnicianServiceProxy, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpValidationError } from '@shared/components/validation/abp-validation.api';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { EqualValidator } from '../../../shared/directives/equal-validator.directive';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { SelectDoctorRoleComponent } from '../select-doctor-role/select-doctor-role.component'
import { SelectNurseRoleComponent } from '../select-nurse-role/select-nurse-role.component'
import { SelectLabtechnicianRoleComponent } from '../select-labtechnician-role/select-labtechnician-role.component'
import { SelectPatientRoleComponent } from '../select-patient-role/select-patient-role.component';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
@Component({
    templateUrl: './create-user-dialog.component.html',
    standalone: true,
    imports: [CheckboxModule,InputTextModule,SelectModule,
        FormsModule,
        AbpModalHeaderComponent,
        AbpValidationSummaryComponent,
        EqualValidator,
        AbpModalFooterComponent,
        LocalizePipe,
        SelectDoctorRoleComponent,
        SelectNurseRoleComponent,
        CommonModule,
        SelectLabtechnicianRoleComponent,
        SelectPatientRoleComponent
    ],
    providers: [DoctorServiceProxy, NurseServiceProxy, LapTechnicianServiceProxy, PatientServiceProxy]
})
export class CreateUserDialogComponent extends AppComponentBase implements OnInit {
    @ViewChild('createUserModal', { static: true }) createUserModal: NgForm;
    @ViewChild(SelectDoctorRoleComponent) createDoctorComponent: SelectDoctorRoleComponent;
    @ViewChild(SelectNurseRoleComponent) createNurseComponent: SelectNurseRoleComponent;
    @ViewChild(SelectLabtechnicianRoleComponent) createLabTechnicianComponent: SelectLabtechnicianRoleComponent;
    @ViewChild(SelectPatientRoleComponent) createPatientComponent: SelectPatientRoleComponent;


    @Output() onSave = new EventEmitter<void>();
    @Input() defaultRole: string = '';
    @Input() disableRoleSelection: boolean = false;
    user: CreateUserDto = new CreateUserDto();
    roles: RoleDto[] = [];
    selectedRole: string = '';
    confirmPassword: string = '';
    saving = false;

    passwordValidationErrors: Partial<AbpValidationError>[] = [
        {
            name: 'pattern',
            localizationKey: 'PasswordsMustBeAtLeast8CharactersContainLowercaseUppercaseNumber',
        }
    ];

    confirmPasswordValidationErrors: Partial<AbpValidationError>[] = [
        {
            name: 'validateEqual',
            localizationKey: 'PasswordsDoNotMatch',
        }
    ];

    doctorData: any;
    nurseData: any;
    technicianData: any;
    patientData: any;



    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _doctorService: DoctorServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef,
        private _nurseService: NurseServiceProxy,
        private _labTechnicianService: LapTechnicianServiceProxy,
        private _patientService: PatientServiceProxy,

    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.user.isActive = true;
        this.loadRoles();
        if (this.defaultRole) {
            this.selectedRole = 'PATIENT';
        }
    }

    loadRoles(): void {
        this._userService.getRoles().subscribe(result => {
            this.roles = result.items;
            debugger
            this.cd.detectChanges();
        });
    }
    onRoleChange() {
        console.log('Selected Role:', this.selectedRole);
        this.cd.detectChanges();
    }

    onDoctorDataChange(data: any): void {
        this.doctorData = data;

    }
    onNurseDataChange(data: any): void {
        this.nurseData = data;
    }
    onLabTechnicianDataChange(data: any): void {
        this.technicianData = data;
    }
    onPatientDataChange(data: any): void {
        this.patientData = data;
    }

    get isFormValid(): boolean {

        const mainFormValid = this.createUserModal?.form?.valid;
        const doctorFormValid = this.selectedRole === 'DOCTORS'
            ? this.createDoctorComponent?.doctorForm?.valid
            : true;

        const nurseFormValid = this.selectedRole === 'NURSE'
            ? this.createNurseComponent?.nurseForm?.valid
            : true;

        const labTechnicianFormValid = this.selectedRole === 'LAB TECHNICIAN'
            ? this.createLabTechnicianComponent?.labTechnicianForm?.valid
            : true;

        const patientFormValid = this.selectedRole === 'PATIENT'
            ? this.createPatientComponent?.patientForm?.valid
            : true;

        return mainFormValid && doctorFormValid && nurseFormValid && labTechnicianFormValid && patientFormValid;

    }

    newlyCreatedUserId!: number;
    save(): void {
        if (!this.createUserModal.form.valid) {
            return;
        }
        this.saving = true;
        this.user.roleNames = [this.selectedRole];

        if (this.selectedRole === 'DOCTOR' && this.doctorData) {
            this.user['doctorProfile'] = this.doctorData;
        }

        if (this.selectedRole === 'NURSE' && this.nurseData) {
            this.user['nurseProfile'] = this.nurseData;
        }
        if (this.selectedRole === 'LAB TECHNICIAN' && this.technicianData) {
            this.user['technicianProfile'] = this.technicianData;
        }
        if (this.selectedRole === 'PATIENT' && this.patientData) {

            this.user['patientProfile'] = this.patientData;
        }
        this._userService.create(this.user).subscribe({
            next: (res) => {
                this.newlyCreatedUserId = res.id;
                if (this.selectedRole === 'DOCTOR') {
                    this.CreateDoctor();
                }
                if (this.selectedRole === 'NURSE') {
                    this.CreateNurse();
                }
                if (this.selectedRole === 'LAB TECHNICIAN') {

                    this.CreateLabTechnician();
                }
                if (this.selectedRole === 'PATIENT') {

                    this.CreatePatient();
                }
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            error: () => {
                this.saving = false;
            }
        });
    }

    CreateDoctor() {
        this.doctorData.fullName = this.user.name + " " + this.user.surname;
        this.doctorData.abpUserId = this.newlyCreatedUserId;
        this._doctorService.create(this.doctorData).subscribe({
            next: (res) => {
                this.newlyCreatedUserId = 0;
            },
            error: (err) => {
            }
        })
    }

    CreateNurse() {
        this.nurseData.fullName = this.user.name + " " + this.user.surname;
        this.nurseData.abpUserId = this.newlyCreatedUserId;
        this._nurseService.create(this.nurseData).subscribe({
            next: (res) => {
                this.newlyCreatedUserId = 0;
            },
            error: (err) => {
            }
        })
    }

    CreateLabTechnician() {
        this.technicianData.fullName = this.user.name + " " + this.user.surname;
        this.technicianData.abpUserId = this.newlyCreatedUserId;
        this._labTechnicianService.create(this.technicianData).subscribe({
            next: (res) => {
                this.newlyCreatedUserId = 0;
            },
            error: (err) => {
            }
        })
    }

    CreatePatient() {
  this.patientData.fullName = this.user.name + " " + this.user.surname;
  this.patientData.abpUserId = this.newlyCreatedUserId;

    this._patientService.create(this.patientData).subscribe({
      next: () => {
        this.notify.success('Patient created successfully');
        this.newlyCreatedUserId = 0;
      },
      error: (err) => {
        this.notify.error('Patient creation failed');
      }
    });
  }


}
