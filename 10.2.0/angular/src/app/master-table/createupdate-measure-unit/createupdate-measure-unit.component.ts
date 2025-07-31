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
  MeasureUnitServiceProxy,
  MeasureUnitDto,
  CreateUpdateMeasureUnitDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-createupdate-measure-unit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    CheckboxModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    AutoCompleteModule
  ],
  providers: [MeasureUnitServiceProxy],
  templateUrl: './createupdate-measure-unit.component.html',
  styleUrl: './createupdate-measure-unit.component.css'
})
export class CreateupdateMeasureUnitComponent extends AppComponentBase implements OnInit {
  @ViewChild('unitForm', { static: true }) unitForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  // update mode
  unit: Partial<MeasureUnitDto> = { name: '', isActive: true };

  // bulk create mode
  selectedUnitNames: string[] = [];
  filteredUnitNames: string[] = [];
  unitNameMasterList: string[] = [];

  isActive: boolean = true;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _unitService: MeasureUnitServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isFormValid(): boolean {
    if (this.id) {
      return this.unitForm?.form.valid && !!this.unit.name;
    } else {
      return this.unitForm?.form.valid && this.selectedUnitNames.length > 0;
    }
  }

  ngOnInit(): void {
    if (this.id) {
      this._unitService.get(this.id).subscribe((res) => {
        this.unit = res;
        this.isActive = res.isActive!;
        this.cd.detectChanges();
      });
    }
  }

  filterUnitNames(event: { query: string }): void {
    const q = event.query.toLowerCase();
    this.filteredUnitNames = this.unitNameMasterList.filter((name) =>
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
      const input = new CreateUpdateMeasureUnitDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.name = this.unit.name!;
      input.isActive = this.isActive;

      this._unitService.update(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
      });
    } else {
      // BULK CREATE
      const inputs = this.selectedUnitNames.map((name) => {
        const dto = new CreateUpdateMeasureUnitDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.name = name;
        dto.isActive = this.isActive;
        return dto;
      });
debugger
      this._unitService.createBulk(inputs).subscribe({
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

