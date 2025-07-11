import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Injector,
  Input,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import {
  CreateUpdateMedicineOrderDto,
  CreateUpdateMedicineOrderItemDto,
  MedicineOrderServiceProxy,
  NurseDto,
  NurseServiceProxy,
  PatientDropDownDto,
  PatientServiceProxy,
  PharmacistInventoryServiceProxy
} from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';

@Component({
  selector: 'app-edit-order-medicine',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    DropdownModule,
    InputTextModule,
    ButtonModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent
  ],
  providers: [
    PatientServiceProxy,
    NurseServiceProxy,
    PharmacistInventoryServiceProxy,
    MedicineOrderServiceProxy
  ],
  templateUrl: './edit-order-medicine.component.html',
  styleUrl: './edit-order-medicine.component.css'
})
export class EditOrderMedicineComponent extends AppComponentBase implements OnInit {
  @Input() orderId: number;
  @ViewChild('editOrderForm', { static: true }) editOrderForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  

  saving = false;

  order: CreateUpdateMedicineOrderDto = new CreateUpdateMedicineOrderDto();
  nurses: NurseDto[] = [];
  patients: PatientDropDownDto[] = [];
  medicineOptions: any[] = [];
  medicineDosageOptions: { [medicineId: number]: string[] } = {};

  priorities = [
    { label: 'Low', value: 'Low' },
    { label: 'Medium', value: 'Medium' },
    { label: 'High', value: 'High' }
  ];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _nurseService: NurseServiceProxy,
    private _patientService: PatientServiceProxy,
    private _orderMedicineService: MedicineOrderServiceProxy,
    private _pharmaService: PharmacistInventoryServiceProxy,
    private cdr: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadDropdowns();

    if (this.orderId) {
      this._orderMedicineService.getOrderDetailsById(this.orderId).subscribe({
        next: (res) => {
          this.order = new CreateUpdateMedicineOrderDto({
            ...res,
            nurseId: Number(res.nurseId),
            patientId: Number(res.patientId),
            priority: res.priority,
            items: res.items.map((item) =>
              new CreateUpdateMedicineOrderItemDto({
                ...item,
                medicineId: Number(item.medicineId)
              })
            )
          });

          this.cdr.detectChanges();
        },
        error: (err) => console.error(err)
      });
    }
  }

  loadDropdowns(): void {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe((res) => {
      this.nurses = res.items;
    });

    this._patientService.patientDropDown().subscribe((res) => {
      this.patients = res;
    });

    this._pharmaService
      .getAll(undefined, undefined, undefined, undefined, undefined, true, undefined, undefined)
      .subscribe((res) => {
        this.medicineOptions = res.items.map((x) => ({
          label: x.medicineName,
          value: x.id,
          name: x.medicineName,
          unit: x.unit
        }));

        res.items.forEach((med) => {
          if (med.unit) {
            const units = med.unit.split(',').map((u) => u.trim());
            this.medicineDosageOptions[med.id] = units;
          }
        });
      });
  }

  addItem(): void {
    const newItem = new CreateUpdateMedicineOrderItemDto();
    newItem.medicineId = null;
    newItem.dosage = null;
    newItem.quantity = 1;

    this.order.items.push(newItem);
    this.cdr.detectChanges();
  }

  removeItem(index: number): void {
    this.order.items.splice(index, 1);
    this.cdr.detectChanges();
  }

  onMedicineChange(item: any): void {
    item.dosage = null;
    this.cdr.detectChanges();
  }

  getDosageOptions(medicineId: number): any[] {
    const units = this.medicineDosageOptions[medicineId] || [];
    return units.map((u) => ({ label: u, value: u }));
  }

  save(): void {
    if (this.editOrderForm.invalid) {
      return;
    }

    this.saving = true;
    const input = new CreateUpdateMedicineOrderDto();
    input.init({
      id:this.order.id,
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

    this._orderMedicineService.updateMedicineOrderWithItem(input).subscribe({
      next: () => {
        this.notify.info(this.l('UpdatedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (err) => {
        console.error(err);
        this.saving = false;
      }
    });
  }
}

