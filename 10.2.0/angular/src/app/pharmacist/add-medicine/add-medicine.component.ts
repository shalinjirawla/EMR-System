import { Component, Injector, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { CommonModule } from '@angular/common';
import { PharmacistInventoryServiceProxy, CreateUpdatePharmacistInventoryDto } from '@shared/service-proxies/service-proxies';
import moment from 'moment';

@Component({
  selector: 'app-add-medicine',
   imports: [
    FormsModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    CommonModule, 
    DatePickerModule, 
    TextareaModule,
    InputNumberModule,
    DropdownModule,
    InputTextModule,
    CheckboxModule
  ],
  providers:[PharmacistInventoryServiceProxy],
  templateUrl: './add-medicine.component.html',
  styleUrl: './add-medicine.component.css'
})
export class AddMedicineComponent extends AppComponentBase implements OnInit {
 @ViewChild('createInventoryModal', { static: true }) createInventoryModal: NgForm;
  @Output() onSave = new EventEmitter<void>();
  
  saving = false;
  tomorrow!: Date;
  
  unitTypeOptions = [
    { label: 'mg', value: 'mg' },
    { label: 'ml', value: 'ml' },
    // { label: 'g', value: 'g' },
    // { label: 'L', value: 'L' },
    // { label: 'IU', value: 'IU' },
    // { label: 'mcg', value: 'mcg' },
    // { label: 'tablet', value: 'tablet' },
    // { label: 'capsule', value: 'capsule' }
  ];
  
  inventory: any = {
    id: 0,
    tenantId: 0,
    medicineName: '',
    costPrice: 0,
    sellingPrice: 0,
    expiryDate: null,
    purchaseDate: null,
    unitValue: null,
    unitType: null,
    stock: 1,
    minStock: 1,
    description: '',
    isAvailable: true
  };

  get isFormValid(): boolean {
    const mainFormValid = this.createInventoryModal?.form?.valid;
    const expiryValid = this.validateExpiryDate();
    const unitValid = this.inventory.unitValue && this.inventory.unitType;
    return mainFormValid && expiryValid && unitValid;
  }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _inventoryService: PharmacistInventoryServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.tomorrow = moment().add(1, 'day').toDate();
    this.inventory.tenantId = abp.session.tenantId;
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn("Please complete the form properly.");
      return;
    }

    if (!this.validateExpiryDate()) {
      return;
    }

    const input = new CreateUpdatePharmacistInventoryDto();
    input.id = this.inventory.id;
    input.tenantId = this.inventory.tenantId;
    input.medicineName = this.inventory.medicineName;
    input.costPrice = this.inventory.costPrice;
    input.sellingPrice = this.inventory.sellingPrice;
    input.expiryDate = moment(this.inventory.expiryDate);
    input.purchaseDate = moment(this.inventory.purchaseDate);
    input.unit = `${this.inventory.unitValue} ${this.inventory.unitType}`;
    input.stock = this.inventory.stock;
    input.minStock = this.inventory.minStock;
    input.description = this.inventory.description;
    input.isAvailable = this.inventory.isAvailable;

    this.saving = true;
    this._inventoryService.create(input).subscribe({
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

  validateExpiryDate(): boolean {
    if (!this.inventory.expiryDate || !this.inventory.purchaseDate) {
      return false;
    }

    const expiryDate = moment(this.inventory.expiryDate);
    const purchaseDate = moment(this.inventory.purchaseDate);

    if (expiryDate.isBefore(purchaseDate)) {
      this.message.warn("Expiry date must be after purchase date.");
      return false;
    }

    return true;
  }
}