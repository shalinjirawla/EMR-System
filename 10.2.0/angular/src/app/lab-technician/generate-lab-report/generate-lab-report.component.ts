import { ChangeDetectorRef, Component, Injector, OnInit, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CreateUpdateLabReportResultItemDto, LabReportResultItemServiceProxy, LabReportsTypeServiceProxy } from '@shared/service-proxies/service-proxies';
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
  selector: 'app-generate-lab-report',
  imports: [
    FormsModule, CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent,
    MultiSelectModule, TagModule
  ],
  templateUrl: './generate-lab-report.component.html',
  styleUrl: './generate-lab-report.component.css',
  providers: [LabReportResultItemServiceProxy, LabReportsTypeServiceProxy]
})
export class GenerateLabReportComponent extends AppComponentBase implements OnInit {
  id!: number;
  isEmergencyCase: boolean;
  emergencyCaseId: number;
  labReportsTypeId!: number;
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
    public cd: ChangeDetectorRef,
    private _labreportService: LabReportResultItemServiceProxy,
    private _reportTypeService: LabReportsTypeServiceProxy,
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.getlabtestitems();
  }

  getlabtestitems() {
    this._reportTypeService
      .getReportDetailsWithTests(this.labReportsTypeId)
      .subscribe(res => {
        this.testName = res.reportName;
        this.labReportItems = res.labTests.map(t => ({
          test: t.labTestName,
          minValue: t.minRange,
          maxValue: t.maxRange,
          unit: t.measureUnitName,
          result: '',
          flag: ''
        }));
        this.cd.detectChanges();
      });

  }

  onResultChange(item: any) {
    const parsed = parseFloat(item.result);
    if (isNaN(parsed)) {
      item.flag = undefined;
      return;
    }
    // use minValue/maxValue instead of minRange/maxRange
    if (item.minValue == null || item.maxValue == null) {
      item.flag = 'NA';
    } else if (parsed < item.minValue) {
      item.flag = 'Low';
    } else if (parsed > item.maxValue) {
      item.flag = 'High';
    } else {
      item.flag = 'Normal';
    }
  }


  isSaveDisabled(): boolean {
    if (!this.labReportItems || this.labReportItems.length === 0) {
      return true;
    }

    return this.labReportItems.some((item, index) => {
      const isNumericTest = item.minValue != null && item.maxValue != null;

      return (
        !item.test?.trim() ||
        item.result == null || item.result.toString().trim() === '' ||
        (isNumericTest && !item.unit?.trim()) || // Require unit only for numeric
        (isNumericTest && this.isInvalidRange(item, index))
      );
    });
  }


  isInvalidRange(item: any, index: number): boolean {
    return this.hasEditedMaxMap[index]
      && item.minValue != null
      && item.maxValue != null
      && item.minValue >= item.maxValue;
  }
  markMaxEdited(index: number): void {
    this.hasEditedMaxMap[index] = true;
  }
  addItem() {
    this.labReportItems.push({
      test: '',
      result: null,
      minValue: null,
      maxValue: null,
      unit: '',
      flag: '',
      id: 0,
      prescriptionLabTestId: 0,
    });
  }
  removeItem(index: number) {
    this.labReportItems.splice(index, 1);
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

  getSeverity(flag: string): string {
    switch (flag) {
      case 'High': return 'danger';
      case 'Low': return 'warn';
      case 'Normal': return 'success';
      default: return 'info';
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
  save() {
    this.labReportItems.forEach(item => {
      item.prescriptionLabTestId = this.id;
    });
    if(this.emergencyCaseId==null){
      this.emergencyCaseId=undefined;
    }
    this._labreportService.addLabReportResultItem(this.emergencyCaseId,this.isEmergencyCase,this.labReportItems).subscribe({
      next: (res) => {
        this.bsModalRef.hide();
        this.onSave.emit();
      }, error: (err) => {
      }
    })
  }
}