import { ChangeDetectorRef, Component, Injector, OnInit, Output, ViewChild, EventEmitter } from '@angular/core';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { FormsModule, NgForm } from '@angular/forms';
import { CreateUpdateMedicineOrderDto, CreateUpdateMedicineOrderItemDto, MedicineOrderServiceProxy, NurseDto, NurseServiceProxy, PatientDropDownDto, PatientServiceProxy, PharmacistInventoryServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';


@Component({
  selector: 'app-create-order-medicine',
  imports: [
    FormsModule, CommonModule,
    DropdownModule, InputTextModule, ButtonModule,
    AbpModalHeaderComponent, AbpModalFooterComponent
  ],
  providers: [PatientServiceProxy, NurseServiceProxy, PharmacistInventoryServiceProxy,MedicineOrderServiceProxy],
  templateUrl: './create-order-medicine.component.html',
  styleUrl: './create-order-medicine.component.css'
})
export class CreateOrderMedicineComponent extends AppComponentBase implements OnInit {
  @ViewChild('orderForm', { static: true }) orderForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;

  nurses: NurseDto[] = [];
  patients: PatientDropDownDto[] = [];
  priorities = [
    { label: 'Low', value: 'Low' },
    { label: 'Medium', value: 'Medium' },
    { label: 'High', value: 'High' }
  ];

  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineName: string]: string[] } = {};

  order: any = {
    tenantId: abp.session.tenantId,
    nurseId: null,
    patientId: null,
    priority: null,
    items: []
  };

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _nurseService: NurseServiceProxy,
    private _patientService: PatientServiceProxy,
    private _orderService: MedicineOrderServiceProxy,
    private _pharmaService: PharmacistInventoryServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadNurses();
    this.loadPatients();
    this.loadMedicines();
    this.addItem(); // start with one item
  }

  loadNurses() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(res => {
      this.nurses = res.items;
    });
  }

  loadPatients() {
    this._patientService.patientDropDown().subscribe(res => {
      this.patients = res;
    });
  }

  loadMedicines() {
    this._pharmaService.getAll(undefined, undefined, undefined, undefined, undefined, true,undefined,undefined)
      .subscribe(res => {
        this.medicineOptions = res.items.map(x => ({
          label: x.medicineName,
          value: x.id,
          name: x.medicineName,
          unit: x.unit
        }));

        res.items.forEach(med => {
          if (med.unit) {
            const units = med.unit.split(',').map(u => u.trim());
            this.medicineDosageOptions[med.medicineName] = units;
          }
        });
      });
  }

  addItem() {
    this.order.items.push({
      medicineId: null,
      medicineName: '',
      dosage: '',
      quantity: 1
    });
  }

  removeItem(index: number) {
    this.order.items.splice(index, 1);
  }

  onMedicineChange(item: any) {
    const selected = this.medicineOptions.find(x => x.value === item.medicineId);
    if (selected) {
      item.medicineName = selected.name;
      const dosageOptions = this.medicineDosageOptions[selected.name];
      item.dosage = dosageOptions?.[0] || '';
    }
  }

  getDosageOptions(medicineName: string): any[] {
    return (this.medicineDosageOptions[medicineName] || []).map(u => ({ label: u, value: u }));
  }

  save() {
    if (!this.orderForm.valid) {
      this.message.warn("Please complete the form properly.");
      return;
    }

    if (this.order.items.length === 0) {
      this.message.warn("Please add at least one item.");
      return;
    }
const input = new CreateUpdateMedicineOrderDto();
    input.init({
      tenantId: this.order.tenantId,
      patientId: this.order.patientId,
      nurseId: this.order.nurseId,
      priority:this.order.priority,
    });

    // Prepare items properly
    input.items = this.order.items.map(item => {
  const dtoItem = new CreateUpdateMedicineOrderItemDto();
  dtoItem.init({
    ...item,
    dosage:item.dosage,
    quantity:item.quantity,
    medicineId: item.medicineId // <-- Make sure this is included
  });
  return dtoItem;
});
debugger
    this._orderService.createMedicineOrderWithItem(input).subscribe({
      next: (res) => {
       console.log("Saved order:", this.order);
    this.notify.success("Order placed");
    this.bsModalRef.hide();
    this.onSave.emit();
      },
      error: (err) => {
        this.saving = false;
        this.notify.error('Could not save prescription');
      },
      complete: () => {
        this.saving = false;
      }
    });
  }
}

