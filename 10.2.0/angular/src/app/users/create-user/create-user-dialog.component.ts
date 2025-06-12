import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy, CreateUserDto, RoleDto, CreateUpdateDoctorDto, DoctorServiceProxy, NurseServiceProxy, LapTechnicianServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpValidationError } from '@shared/components/validation/abp-validation.api';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { EqualValidator } from '../../../shared/directives/equal-validator.directive';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { CreateDoctorComponent } from '../../doctors/create-doctor/create-doctor.component';
import { CreateNurseComponent } from '../../nurse/create-nurse/create-nurse.component';
import { CreateLabTechnicianComponent } from '../../lab-technician/create-lab-technician/create-lab-technician.component';



@Component({
  templateUrl: './create-user-dialog.component.html',
  standalone: true,
  imports: [
    FormsModule,
    AbpModalHeaderComponent,
    AbpValidationSummaryComponent,
    EqualValidator,
    AbpModalFooterComponent,
    LocalizePipe,
    CreateDoctorComponent,
    CreateNurseComponent,
    CreateLabTechnicianComponent,
    CommonModule
  ],
  providers: [DoctorServiceProxy, NurseServiceProxy, LapTechnicianServiceProxy]
})
export class CreateUserDialogComponent extends AppComponentBase implements OnInit {
  @ViewChild('createUserModal', { static: true }) createUserModal: NgForm;
  @ViewChild(CreateDoctorComponent) createDoctorComponent: CreateDoctorComponent;
  @ViewChild(CreateNurseComponent) createNurseComponent: CreateNurseComponent;
  @ViewChild(CreateLabTechnicianComponent) createLabTechnicianComponent: CreateLabTechnicianComponent;

  @Output() onSave = new EventEmitter<void>();

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

  // You can hold extra doctor data if needed
  doctorData: any;
  nurseData: any;
  technicianData: any;


  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _doctorService: DoctorServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _nurseService: NurseServiceProxy,
    private _labTechnicianService: LapTechnicianServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.user.isActive = true;
    this.loadRoles();
  }

  loadRoles(): void {
    this._userService.getRoles().subscribe(result => {
      this.roles = result.items;
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

    return mainFormValid && doctorFormValid && nurseFormValid && labTechnicianFormValid;

  }

  newlyCreatedUserId!: number;
  save(): void {
    if (!this.createUserModal.form.valid) {
      return;
    }

    // // Doctor Form validation
    // if (this.selectedRole === 'DOCTORS' && this.createDoctorComponent?.doctorForm?.invalid) {
    //   this.notify.warn(this.l('PleaseFillDoctorFormCorrectly'));
    //   return;
    // }

    // //Nurse Form validation
    // if (this.selectedRole === 'NURSE' && this.createNurseComponent?.nurseForm?.invalid) {
    //   this.notify.warn(this.l('PleaseFillNurseFormCorrectly'));
    //   return;
    // }

    this.saving = true;
    this.user.roleNames = [this.selectedRole];

    if (this.selectedRole === 'DOCTORS' && this.doctorData) {
      this.user['doctorProfile'] = this.doctorData;
    }

    if (this.selectedRole === 'NURSE' && this.nurseData) {
      this.user['nurseProfile'] = this.nurseData;
    }
    if (this.selectedRole === 'LAB TECHNICIAN' && this.technicianData) {
      this.user['technicianProfile'] = this.technicianData;
    }
    this._userService.create(this.user).subscribe({
      next: (res) => {
        this.newlyCreatedUserId = res.id;
        if (this.selectedRole === 'DOCTORS') {
          this.CreateDoctor();
        }
        if (this.selectedRole === 'NURSE') {
          this.CreateNurse();
        }
        if (this.selectedRole === 'LAB TECHNICIAN') {

          this.CreateLabTechnician();
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
}