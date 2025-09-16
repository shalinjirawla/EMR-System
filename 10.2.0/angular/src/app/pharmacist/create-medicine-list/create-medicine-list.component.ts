import { Component, Injector, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
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
import { CreateUpdateMedicineMasterDto, MedicineFormMasterDto, MedicineFormMasterServiceProxy, MedicineMasterServiceProxy, StrengthUnitMasterDto, StrengthUnitMasterServiceProxy } from '@shared/service-proxies/service-proxies';


@Component({
  selector: 'app-create-medicine-list',
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
  providers:[MedicineMasterServiceProxy,MedicineFormMasterServiceProxy,StrengthUnitMasterServiceProxy],
  templateUrl: './create-medicine-list.component.html',
  styleUrl: './create-medicine-list.component.css'
})
export class CreateMedicineListComponent extends AppComponentBase implements OnInit {
  @ViewChild('createMedicineModal', { static: true }) createMedicineModal: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;

  medicine: CreateUpdateMedicineMasterDto = new CreateUpdateMedicineMasterDto();

  formOptions: MedicineFormMasterDto[] = [];
  strengthUnitOptions: StrengthUnitMasterDto[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _medicineService: MedicineMasterServiceProxy,
    private _formService: MedicineFormMasterServiceProxy,
    private _strengthService: StrengthUnitMasterServiceProxy
  ) {
    super(injector);
    this.medicine.tenantId = abp.session.tenantId;
    this.medicine.isAvailable = true;
  }

  ngOnInit(): void {
    this.loadDropdownData();
  }

    loadDropdownData() {
    this._formService.getAlldicineFormByTenantId(abp.session.tenantId).subscribe(res => {
      this.formOptions = res.items;
    });
    this._strengthService.getAllStrengthUnitsByTenantId(abp.session.tenantId).subscribe(res => {
      this.strengthUnitOptions = res.items;
    });
  }


  get isFormValid(): boolean {
    return this.createMedicineModal?.form?.valid;
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn("Please complete the form properly.");
      return;
    }

    this.saving = true;
    this._medicineService.create(this.medicine).subscribe({
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