import { ChangeDetectorRef, Component, Injector, OnInit, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CreateUpdateLabReportResultItemDto, LabReportResultItemServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { CommonModule } from '@angular/common';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { TextareaModule } from 'primeng/textarea';
import { MultiSelectModule } from 'primeng/multiselect';
import { TagModule } from 'primeng/tag';
@Component({
  selector: 'app-edit-lab-report',
  imports: [
    FormsModule, CalendarModule, DropdownModule, CheckboxModule, InputTextModule,
    TextareaModule, ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent,
    AbpModalFooterComponent, MultiSelectModule, TagModule
  ],
  templateUrl: './edit-lab-report.component.html',
  styleUrl: './edit-lab-report.component.css',
  providers: [LabReportResultItemServiceProxy]
})
export class EditLabReportComponent extends AppComponentBase implements OnInit {
  id!: number;
  testName!: string;
  patientName!: string;
  saving = false;
  labReportItems: any[] = [];
  @Output() onSave = new EventEmitter<void>();
  unitOptions = [
    { label: 'g/dL', value: 'g/dL' },
    { label: 'mg/dL', value: 'mg/dL' },
    { label: 'mmol/L', value: 'mmol/L' },
    { label: '%', value: '%' },
    { label: 'IU/L', value: 'IU/L' },
    { label: 'ng/mL', value: 'ng/mL' },
    { label: 'pg/mL', value: 'pg/mL' },
    { label: 'U/L', value: 'U/L' },
    { label: 'μg/dL', value: 'μg/dL' },
    { label: 'mEq/L', value: 'mEq/L' },
  ];
  hasEditedMaxMap: { [index: number]: boolean } = {};
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _labreportService: LabReportResultItemServiceProxy,
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.GetLabReportResultItems();
  }
  GetLabReportResultItems() {
    this._labreportService.getLabReportResultItemsById(this.id).subscribe({
      next: (res) => {
        this.labReportItems = res;
        this.cd.detectChanges();
      }, error: (err) => {
      }
    })
  }
  isSaveDisabled(): boolean {
    if (!this.labReportItems || this.labReportItems.length === 0) {
      return true;
    }

    return this.labReportItems.some(item => item.result == null);
  }
  isInvalidRange(item: any, index: number): boolean {
    return this.hasEditedMaxMap[index]
      && item.minValue != null
      && item.maxValue != null
      && item.minValue >= item.maxValue;
  }
  getSeverity(flag: string | undefined): string {
    switch (flag) {
      case 'Normal': return 'success';
      case 'High': return 'danger';
      case 'Low': return 'warn';
      default: return 'secondary'; // For 'Unset' or empty
    }
  }
  calculateFlag(item: CreateUpdateLabReportResultItemDto) {
  // Try to convert result to a number
  const numericResult = parseFloat(item.result);

  // If result is numeric and min/max are set, compare
  if (!isNaN(numericResult) && item.minValue != null && item.maxValue != null) {
    if (numericResult < item.minValue) {
      item.flag = 'Low';
    } else if (numericResult > item.maxValue) {
      item.flag = 'High';
    } else {
      item.flag = 'Normal';
    }
  } else {
    // For qualitative results like "Positive", "Negative"
    item.flag = 'Unset';
  }
}

  getFlag(result: string, reference: string): string {
    const parsedResult = parseFloat(result);
    const [min, max] = reference.split('-').map(v => parseFloat(v));
    if (isNaN(parsedResult) || isNaN(min) || isNaN(max)) return 'Unknown';

    if (parsedResult < min) return 'Low';
    if (parsedResult > max) return 'High';
    return 'Normal';
  }
  markMaxEdited(index: number): void {
    this.hasEditedMaxMap[index] = true;
  }
  save() {
    this.labReportItems.forEach(item => {
      item.prescriptionLabTestId = this.id;
    });
    this._labreportService.editLabReportResultItem(this.labReportItems).subscribe({
      next: (res) => {
        this.bsModalRef.hide();
        this.onSave.emit();
      }, error: (err) => {
      }
    })
  }
}
