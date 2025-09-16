import { Component, Injector, OnInit, EventEmitter, Output, ViewChild, ChangeDetectorRef } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import {
  CreateUpdateMedicineMasterDto,
  MedicineFormMasterDto,
  MedicineFormMasterServiceProxy,
  MedicineMasterServiceProxy,
  StrengthUnitMasterDto,
  StrengthUnitMasterServiceProxy
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-edit-medicine-list',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    CheckboxModule,
    TextareaModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent
  ],
  providers: [MedicineMasterServiceProxy, MedicineFormMasterServiceProxy, StrengthUnitMasterServiceProxy],
  templateUrl: './edit-medicine-list.component.html',
  styleUrl: './edit-medicine-list.component.css'
})
export class EditMedicineListComponent extends AppComponentBase implements OnInit {
  @ViewChild('editMedicineModal', { static: true }) editMedicineModal: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  medicineId!: number; // injected via initialState
  medicine: CreateUpdateMedicineMasterDto = new CreateUpdateMedicineMasterDto();

  formOptions: MedicineFormMasterDto[] = [];
  strengthUnitOptions: StrengthUnitMasterDto[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _medicineService: MedicineMasterServiceProxy,
    private _formService: MedicineFormMasterServiceProxy,
    private _strengthService: StrengthUnitMasterServiceProxy,
    public cd:ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadDropdownData();
    if (this.medicineId) {
      this._medicineService.get(this.medicineId).subscribe(res => {
        this.medicine = res;
        this.cd.detectChanges();
      });
    }
  }

  loadDropdownData() {
    this._formService.getAlldicineFormByTenantId(abp.session.tenantId).subscribe(res => {
      this.formOptions = res.items;
        this.cd.detectChanges();

    });
    this._strengthService.getAllStrengthUnitsByTenantId(abp.session.tenantId).subscribe(res => {
      this.strengthUnitOptions = res.items;
        this.cd.detectChanges();

    });
  }

  get isFormValid(): boolean {
    return this.editMedicineModal?.form?.valid;
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please complete the form properly.');
      return;
    }

    this.saving = true;
    this._medicineService.update(this.medicine).subscribe({
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
