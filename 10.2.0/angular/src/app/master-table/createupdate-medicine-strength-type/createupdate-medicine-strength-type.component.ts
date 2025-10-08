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
import { CreateUpdateStrengthUnitMasterDto, StrengthUnitMasterDto, StrengthUnitMasterServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-createupdate-medicine-strength-type',
  standalone: true,
  imports: [CommonModule,FormsModule,InputTextModule,CheckboxModule,AbpModalHeaderComponent,
    AbpModalFooterComponent,AutoCompleteModule,],
  providers: [StrengthUnitMasterServiceProxy],
  templateUrl: './createupdate-medicine-strength-type.component.html',
  styleUrl: './createupdate-medicine-strength-type.component.css',
})
export class CreateupdateMedicineStrengthTypeComponent
  extends AppComponentBase
  implements OnInit
{
  @ViewChild('strengthTypeForm', { static: true }) strengthTypeForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  // update mode
  strengthType: Partial<StrengthUnitMasterDto> = { name: '', isActive: true };

  // bulk create mode
  selectedStrengthTypeNames: string[] = [];
  filteredStrengthTypeNames: string[] = [];
  strengthTypeNameMasterList: string[] = [];

  isActive: boolean = true;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _strengthTypeService: StrengthUnitMasterServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isFormValid(): boolean {
    if (this.id) {
      return this.strengthTypeForm?.form.valid && !!this.strengthType.name;
    } else {
      return (
        this.strengthTypeForm?.form.valid && this.selectedStrengthTypeNames.length > 0
      );
    }
  }

  ngOnInit(): void {
    if (this.id) {
      this._strengthTypeService.get(this.id).subscribe((res) => {
        this.strengthType = res;
        this.isActive = res.isActive!;
        this.cd.detectChanges();
      });
    }
  }

  filterStrengthTypeNames(event: { query: string }): void {
    const q = event.query.toLowerCase();
    this.filteredStrengthTypeNames = this.strengthTypeNameMasterList.filter((name) =>
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
      const input = new CreateUpdateStrengthUnitMasterDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.name = this.strengthType.name!;
      input.isActive = this.isActive;

      this._strengthTypeService.update(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
      });
    } else {
      // BULK CREATE
      const inputs = this.selectedStrengthTypeNames.map((name) => {
        const dto = new CreateUpdateStrengthUnitMasterDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.name = name;
        dto.isActive = this.isActive;
        return dto;
      });

      this._strengthTypeService.createBulk(inputs).subscribe({
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
