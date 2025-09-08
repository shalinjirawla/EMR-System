import { Component, Injector, OnInit, EventEmitter, Output, ViewChild, ChangeDetectorRef } from '@angular/core';
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
import { PharmacistInventoryServiceProxy, CreateUpdatePharmacistInventoryDto, PharmacistInventoryDto } from '@shared/service-proxies/service-proxies';
import moment from 'moment';

@Component({
  selector: 'app-edit-medicine',
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
  providers: [PharmacistInventoryServiceProxy],
  standalone: true,
  templateUrl: './edit-medicine.component.html',
  styleUrls: ['./edit-medicine.component.css']
})
export class EditMedicineComponent extends AppComponentBase implements OnInit {
  @ViewChild('editInventoryModal', { static: true }) editInventoryModal: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  tomorrow!: Date;
  inventoryId!: number;

  unitTypeOptions = [
    { label: 'mg', value: 'mg' },
    { label: 'ml', value: 'ml' },
    { label: 'g', value: 'g' },
    { label: 'L', value: 'L' },
    { label: 'IU', value: 'IU' },
    { label: 'mcg', value: 'mcg' },
    { label: 'tablet', value: 'tablet' },
    { label: 'capsule', value: 'capsule' }
  ];
  _listOfMedicine: PharmacistInventoryDto[];
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
    stock: 0,
    minStock: 0,
    description: '',
    isAvailable: true
  };

  get isFormValid(): boolean {
    const mainFormValid = this.editInventoryModal?.form?.valid;
    const expiryValid = this.validateExpiryDate();
    const unitValid = this.inventory.unitValue && this.inventory.unitType;
    return mainFormValid && expiryValid && unitValid;
  }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _inventoryService: PharmacistInventoryServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.tomorrow = moment().add(1, 'day').toDate();
    this.loadInventory();
    this.GetAllListOfMedicine();
  }
  GetAllListOfMedicine() {
    this._inventoryService.getAllListOfMedicine().subscribe({
      next: (res) => {
        this._listOfMedicine = res;
      }, error: (err) => {

      }
    })
  }
  loadInventory(): void {

    this._inventoryService.get(this.inventoryId).subscribe({

      next: (result: PharmacistInventoryDto) => {
        this.inventory.id = result.id;
        this.inventory.tenantId = result.tenantId;
        this.inventory.medicineName = result.medicineName;
        this.inventory.costPrice = result.costPrice;
        this.inventory.sellingPrice = result.sellingPrice;
        this.inventory.expiryDate = result.expiryDate.toDate();
        this.inventory.purchaseDate = result.purchaseDate.toDate();

        // Split unit into value and type
        const unitParts = result.unit.split(' ');
        this.inventory.unitValue = parseFloat(unitParts[0]);
        this.inventory.unitType = unitParts[1];

        this.inventory.stock = result.stock;
        this.inventory.minStock = result.minStock;
        this.inventory.description = result.description;
        this.inventory.isAvailable = result.isAvailable;
        this.cd.detectChanges();
      },
      error: (err) => {
        this.notify.error('Could not load medicine details');
        this.bsModalRef.hide();
      }
    });
  }

  save(): void {
    // if (this.CheckExistingMedicine()) {
    //   this.message.warn("Medicine already added.");
    //   return;
    // }
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
    this._inventoryService.update(input).subscribe({
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
  CheckExistingMedicine(): boolean {
    const unitString = `${this.inventory.unitValue} ${this.inventory.unitType}`;
    const duplicate = this._listOfMedicine.find(
      x => x.medicineName === this.inventory.medicineName.trim() &&
        x.unit === unitString.trim());
    return !!duplicate;
  }
}
