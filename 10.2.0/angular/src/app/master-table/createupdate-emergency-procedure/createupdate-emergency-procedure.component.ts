import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { InputSwitchModule } from 'primeng/inputswitch';
import { AbpModalFooterComponent } from "@shared/components/modal/abp-modal-footer.component";
import { AbpModalHeaderComponent } from "@shared/components/modal/abp-modal-header.component";
import {
  EmergencyProcedureServiceProxy,
  CreateUpdateEmergencyProcedureDto,
  ProcedureCategory
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-createupdate-emergency-procedure',
  standalone: true,
  imports: [FormsModule,CommonModule,DropdownModule,InputSwitchModule,AbpModalFooterComponent,AbpModalHeaderComponent],
  providers: [EmergencyProcedureServiceProxy],
  templateUrl: './createupdate-emergency-procedure.component.html',
  styleUrl: './createupdate-emergency-procedure.component.css'
})
export class CreateupdateEmergencyProcedureComponent extends AppComponentBase implements OnInit {
  @Input() id?: number;
  @ViewChild('procedureForm', { static: true }) procedureForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  procedureCategories = [
    { label: 'Minor', value: ProcedureCategory._0 },
    { label: 'Major', value: ProcedureCategory._1 },
    { label: 'Life Saving', value: ProcedureCategory._2 }
  ];
  selectedCategory?: ProcedureCategory;
  isActive: boolean = true;
  procedures: any[] = [];
  private counter = 1;

  constructor(
    injector: Injector,
    public modalRef: BsModalRef,
    private procedureService: EmergencyProcedureServiceProxy,
    private cdr: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.id) {
      this.loadForEdit(this.id);
    }
  }

  loadForEdit(id: number): void {
    this.procedureService.get(id).subscribe((dto) => {
      this.procedures = [{
        id: dto.id,
        tenantId: dto.tenantId,
        category: dto.category,
        name: dto.name,
        defaultCharge: dto.defaultCharge,
        isActive: dto.isActive
      }];
      this.isActive = dto.isActive;
      this.cdr.detectChanges();
    });
  }

  addProcedure(): void {
    if (this.selectedCategory === undefined) {
      this.notify.warn("Please select a procedure category first");
      return;
    }
    this.procedures.unshift({
      id: this.counter++, // unique id
      tenantId: this.appSession.tenantId,
      category: this.selectedCategory,
      name: '',
      defaultCharge: null,
    });
  }

  getCategoryLabel(value: ProcedureCategory): string {
    const found = this.procedureCategories.find(x => x.value === value);
    return found ? found.label : '';
  }

  removeProcedure(id: number): void {
    this.procedures = this.procedures.filter(item => item.id !== id);
  }

   trackByFn(index: number, item: any): number {
    return item.id;
  }

  save(): void {
    if (!this.procedureForm.valid || !this.procedures.length) {
      this.notify.warn('Please fill all required fields and add at least one procedure.');
      return;
    }
    this.saving = true;
    if (this.id) {
      // Edit mode: single update
      const item = this.procedures[0];
      const dto = new CreateUpdateEmergencyProcedureDto();
      dto.id = this.id;
      dto.tenantId = this.appSession.tenantId;
      dto.category = item.category;
      dto.name = item.name;
      dto.defaultCharge = item.defaultCharge;
      dto.isActive = this.isActive;
      this.procedureService.update(dto).subscribe({
        next: () => {
          this.notify.success(this.l('UpdatedSuccessfully'));
          this.modalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.notify.error(this.l('UpdateFailed'));
          this.saving = false;
        },
        complete: () => (this.saving = false),
      });
    } else {
      // Create mode: bulk insert
      const input: CreateUpdateEmergencyProcedureDto[] = this.procedures.map(item => {
        const dto = new CreateUpdateEmergencyProcedureDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.category = item.category;
        dto.name = item.name;
        dto.defaultCharge = item.defaultCharge;
        dto.isActive = this.isActive;
        return dto;
      });
      this.procedureService.createBulk(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.modalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.notify.error(this.l('SaveFailed'));
          this.saving = false;
        },
        complete: () => (this.saving = false),
      });
    }
  }
}
