import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy, CreateUserDto, RoleDto } from '@shared/service-proxies/service-proxies';
import { AbpValidationError } from '@shared/components/validation/abp-validation.api';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { EqualValidator } from '../../../shared/directives/equal-validator.directive';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { CreateDoctorComponent } from '../../doctors/create-doctor/create-doctor.component';



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
        CommonModule
    ],
})
export class CreateUserDialogComponent extends AppComponentBase implements OnInit {
  @ViewChild('createUserModal', { static: true }) createUserModal: NgForm;
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

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.user.isActive = true;
    this.loadRoles();
  }

  loadRoles(): void {
    this._userService.getRoles().subscribe(result => {
        debugger
      this.roles = result.items;
      this.cd.detectChanges();
    });
  }
  onRoleChange() {
     console.log('Selected Role:', this.selectedRole);
  this.cd.detectChanges();  // force change detection on role change
}

  onDoctorDataChange(data: any): void {
    this.doctorData = data;
    // If needed, you can map this.doctorData to user.extraProperties or a dedicated DTO
  }

  save(): void {
    if (!this.createUserModal.form.valid) {
      return;
    }

    this.saving = true;

    // Assign selected role
    this.user.roleNames = [this.selectedRole];

    // Optionally: assign doctor data if role is doctor
    if (this.selectedRole == 'DOCTORS' && this.doctorData) {
        debugger
        console.log(this.doctorData)
        
      // Example: you may merge doctorData into user.extraProperties or similar
      this.user['doctorProfile'] = this.doctorData;
      console.log(this.user)
    }

    this._userService.create(this.user).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}