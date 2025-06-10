import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy, UserDto, RoleDto } from '@shared/service-proxies/service-proxies';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { EditDoctorComponent } from '@app/doctors/edit-doctor/edit-doctor.component';

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
        EditDoctorComponent
    ],
})
export class EditUserDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    saving = false;
    user = new UserDto();
    roles: RoleDto[] = [];
    selectedRole: string;
    id: number;

    doctorData: any = {}; 

    constructor(
        injector: Injector,
        public _userService: UserServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._userService.get(this.id).subscribe((result) => {
            this.user = result;

            if (this.user.roleNames?.length > 0) {
                this.selectedRole = this.user.roleNames[0]; 
            }

            this._userService.getRoles().subscribe((result2) => {
                this.roles = result2.items;
                this.cd.detectChanges();
            });
        });
    }

    onRoleChange(roleName: string): void {
        this.selectedRole = roleName;
        if (roleName === 'DOCTORS') {
            this.loadDoctorData();
        }
    }


    loadDoctorData(): void {
       
        this.doctorData = {
         
        };
    }

    onDoctorDataChange(data: any): void {
        this.doctorData = data;
    }

    save(): void {
        this.saving = true;

        this.user.roleNames = [this.selectedRole]; 

      
        if (this.selectedRole === 'DOCTORS') {
  
        }

        this._userService.update(this.user).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            () => {
                this.saving = false;
            }
        );
    }
}
