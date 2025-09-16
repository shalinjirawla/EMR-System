import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  ViewChild,
  ChangeDetectorRef,
} from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { AutoCompleteModule } from 'primeng/autocomplete';
import {
  MedicineFormMasterServiceProxy,
  MedicineFormMasterDto,
  CreateUpdateMedicineFormMasterDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-createupdate-medicine-form-type',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    CheckboxModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    AutoCompleteModule,
  ],
  providers: [MedicineFormMasterServiceProxy],
  templateUrl: './createupdate-medicine-form-type.component.html',
  styleUrl: './createupdate-medicine-form-type.component.css',
})
export class CreateupdateMedicineFormTypeComponent
  extends AppComponentBase
  implements OnInit
{
  @ViewChild('formTypeForm', { static: true }) formTypeForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  // update mode
  formType: Partial<MedicineFormMasterDto> = { name: '', isActive: true };

  // bulk create mode
  selectedFormTypeNames: string[] = [];
  filteredFormTypeNames: string[] = [];
  formTypeNameMasterList: string[] = [];

  isActive: boolean = true;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _formTypeService: MedicineFormMasterServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isFormValid(): boolean {
    if (this.id) {
      return this.formTypeForm?.form.valid && !!this.formType.name;
    } else {
      return (
        this.formTypeForm?.form.valid && this.selectedFormTypeNames.length > 0
      );
    }
  }

  ngOnInit(): void {
    if (this.id) {
      this._formTypeService.get(this.id).subscribe((res) => {
        this.formType = res;
        this.isActive = res.isActive!;
        this.cd.detectChanges();
      });
    }
  }

  filterFormTypeNames(event: { query: string }): void {
    const q = event.query.toLowerCase();
    this.filteredFormTypeNames = this.formTypeNameMasterList.filter((name) =>
      name.toLowerCase().includes(q)
    );
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please fill in required fields.');
      return;
    }

    this.saving = true;

    if (this.id) {
      // UPDATE
      const input = new CreateUpdateMedicineFormMasterDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.name = this.formType.name!;
      input.isActive = this.isActive;

      this._formTypeService.update(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
      });
    } else {
      // BULK CREATE
      const inputs = this.selectedFormTypeNames.map((name) => {
        const dto = new CreateUpdateMedicineFormMasterDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.name = name;
        dto.isActive = this.isActive;
        return dto;
      });

      this._formTypeService.createBulk(inputs).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
        complete: () => (this.saving = false),
      });
    }
  }
}
