import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy, UserDto, RoleDto, DoctorServiceProxy, LapTechnicianServiceProxy, NurseServiceProxy, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { EditDoctorComponent } from '@app/doctors/edit-doctor/edit-doctor.component';
import { EditNurseComponent } from '@app/nurse/edit-nurse/edit-nurse.component';
import { EditLabTechnicianComponent } from '@app/lab-technician/edit-lab-technician/edit-lab-technician.component';
import { EditPatientsComponent } from '../../patient/edit-patients/edit-patients.component'
import { forkJoin } from 'rxjs';



@Component({
    templateUrl: './edit-user-dialog.component.html',
    standalone: true,
    imports: [
        FormsModule,
        AbpModalHeaderComponent,
        AbpValidationSummaryComponent,
        AbpModalFooterComponent,
        CommonModule,
        LocalizePipe,
        EditDoctorComponent,
        EditNurseComponent,
        EditLabTechnicianComponent,
        EditPatientsComponent
    ],
    providers: [DoctorServiceProxy,NurseServiceProxy,LapTechnicianServiceProxy,PatientServiceProxy]
})
export class EditUserDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    @ViewChild('editUserForm', { static: true }) editUserForm: NgForm;
    @ViewChild(EditDoctorComponent) editDoctorComponent: EditDoctorComponent;
    @ViewChild(EditNurseComponent) editNurseComponent: EditNurseComponent;
    @ViewChild(EditLabTechnicianComponent) editLabTechnicianComponent: EditLabTechnicianComponent;
    @ViewChild(EditPatientsComponent) editPatientsComponent: EditPatientsComponent;


    saving = false;
    id: number;
    user = new UserDto();
    roles: RoleDto[] = [];
    selectedRole: string;

    doctorData: any = {};
    nurseData: any = {};
    technicianData: any = {};
    patientData: any = {};

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _doctorService: DoctorServiceProxy,
        private _nurseService: NurseServiceProxy,
        private _labTechnicianService: LapTechnicianServiceProxy,
        private _patientService: PatientServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        forkJoin({
            rolesRes: this._userService.getRoles(),
            userRes: this._userService.getUserDetailsById(this.id),
        }).subscribe(({ rolesRes, userRes }) => {
            this.roles = rolesRes.items;
            this.fillUserInfo(userRes);
            this.cd.detectChanges();

            console.log(userRes)
        });
    }

    private fillUserInfo(data: any): void {
        const { userName, name, surname, emailAddress, isActive } = data;

        this.user.userName = userName;
        this.user.name = name;
        this.user.surname = surname;
        this.user.emailAddress = emailAddress;
        this.user.isActive = isActive;
        this.user.id = data.id


        const roleMap = data.roles?.[0];
        const role = this.roles.find(r => r.id === roleMap?.roleId);

        if (role) {
            this.selectedRole = role.normalizedName;
            this.user.roleNames = [role.normalizedName];
        }

        switch (this.selectedRole) {
            case 'DOCTORS':
                this.setDoctorData(data);
                break;
            case 'NURSE':
                this.setNurseData(data);
                break;
            case 'LAB TECHNICIAN':
                this.setLabTechnicianData(data);
                break;
            case 'PATIENT':
                this.setPatientData(data);
                break;
        }
    }


    private setDoctorData(data: any): void {
        const doc = data.doctors?.[0];
        if (!doc) return;

        this.doctorData = {
            gender: doc.gender,
            specialization: doc.specialization,
            qualification: doc.qualification,
            yearsOfExperience: doc.yearsOfExperience,
            department: doc.department,
            registrationNumber: doc.registrationNumber,
            dateOfBirth: doc.dateOfBirth?.format('YYYY-MM-DD') ?? null,
            abpUserId: doc.abpUserId,
            id: doc.id,
            tenantId: data.tenantId

        };
    }

    private setNurseData(data: any): void {
        const nurse = data.nurses?.[0];
        if (!nurse) return;

        this.nurseData = {
            gender: nurse.gender,
            shiftTiming: nurse.shiftTiming,
            department: nurse.department,
            qualification: nurse.qualification,
            yearsOfExperience: nurse.yearsOfExperience,
            dateOfBirth: nurse.dateOfBirth?.format('YYYY-MM-DD') ?? null,
            abpUserId: nurse.abpUserId,
            id: nurse.id,
            tenantId: data.tenantId
        };
    }

    private setLabTechnicianData(data: any): void {
        debugger
        const tech = data.labTechnicians?.[0];
        if (!tech) return;

        this.technicianData = {
            gender: tech.gender,
            qualification:tech.qualification,
            certificationNumber:tech.certificationNumber,
            department: tech.department,
            yearsOfExperience: tech.yearsOfExperience,
            dateOfBirth: tech.dateOfBirth?.format('YYYY-MM-DD') ?? null,
            abpUserId: tech.abpUserId,
            id: tech.id,
            tenantId: data.tenantId
        };
    }

    private setPatientData(data: any): void {
        const patient = data.patients?.[0];
        if (!patient) return;


        this.patientData = {
            gender: patient.gender,
            dateOfBirth: patient.dateOfBirth?.format('YYYY-MM-DD') ?? null,
            address: patient.address,
            bloodGroup: patient.bloodGroup,
            emergencyContactName: patient.emergencyContactName,
            emergencyContactNumber: patient.emergencyContactNumber,
            assignedNurseId: patient.assignedNurseId,
            isAdmitted: patient.isAdmitted,
            admissionDate: patient.admissionDate?.format('YYYY-MM-DD') ?? null,
            dischargeDate: patient.dischargeDate?.format('YYYY-MM-DD') ?? null,
            insuranceProvider: patient.insuranceProvider,
            insurancePolicyNumber: patient.insurancePolicyNumber,
            assignedDoctorId: patient.assignedDoctorId,
            abpUserId: patient.abpUserId,
            id: patient.id,
            tenantId: data.tenantId
        }
    }


    onRoleChange(roleName: string): void {
        this.selectedRole = roleName;
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
        const isMainValid = this.editUserForm?.form?.valid ?? false;
        const isDoctorValid = this.selectedRole === 'DOCTORS' ? this.editDoctorComponent?.doctorForm?.valid : true;
        const isNurseValid = this.selectedRole === 'NURSE' ? this.editNurseComponent?.nurseForm?.valid : true;
        const isTechValid = this.selectedRole === 'LAB TECHNICIAN' ? this.editLabTechnicianComponent?.labTechnicianForm?.valid : true;
        const isPatientValid = this.selectedRole === 'PATIENT' ? this.editPatientsComponent?.patientForm?.valid : true;

        return isMainValid && isDoctorValid && isNurseValid && isTechValid && isPatientValid;
    }

    save(): void {
        if (!this.isFormValid) {
            this.notify.warn(this.l('FormIsInvalid'));
            return;
        }
        this.saving = true;
        this.user.roleNames = [this.selectedRole];
        this.user.id = this.id;

        this._userService.update(this.user).subscribe({
            next: () => {
                switch (this.selectedRole) {
                    case 'DOCTORS':
                        this.updateDoctor();
                        break;
                    case 'NURSE':
                        this.updateNurse();
                        break;
                    case 'LAB TECHNICIAN':
                        this.updateTechnician();
                        break;
                    case 'PATIENT':
                        this.updatePatient();
                        break;
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

    updateDoctor(): void {
        this.doctorData.fullName = `${this.user.name} ${this.user.surname}`;
        this._doctorService.update(this.doctorData).subscribe();
    }
    updateNurse(): void {
        this.nurseData.fullName = `${this.user.name} ${this.user.surname}`;
        this._nurseService.update(this.nurseData).subscribe();
    }

    updateTechnician(): void {
        this.technicianData.fullName = `${this.user.name} ${this.user.surname}`;
        this._labTechnicianService.update(this.technicianData).subscribe();
    }

    updatePatient(): void {
         this.patientData.fullName = `${this.user.name} ${this.user.surname}`;
        this._patientService.update(this.patientData).subscribe();
    }
}
