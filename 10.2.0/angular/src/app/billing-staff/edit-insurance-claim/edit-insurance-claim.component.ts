import { Component, Injector, OnInit, Input, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/app-component-base';
import { InsuranceClaimDto, InvoiceDto, InvoiceItemDto, ClaimStatus, InsuranceClaimServiceProxy, InvoiceServiceProxy, CreateUpdateInsuranceClaimDto } from '@shared/service-proxies/service-proxies';
import { AbpModalFooterComponent } from "@shared/components/modal/abp-modal-footer.component";
import { AbpModalHeaderComponent } from "@shared/components/modal/abp-modal-header.component";
import { FormsModule } from '@node_modules/@angular/forms';
import { CommonModule } from '@node_modules/@angular/common';
import { TableModule } from 'primeng/table';
import moment from 'moment';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';



@Component({
  selector: 'app-edit-insurance-claim',
  imports: [AbpModalFooterComponent, AbpModalHeaderComponent, FormsModule, CommonModule, TableModule, DropdownModule, CheckboxModule],
  providers: [InsuranceClaimServiceProxy, InvoiceServiceProxy],
  templateUrl: './edit-insurance-claim.component.html',
  styleUrl: './edit-insurance-claim.component.css'
})
export class EditInsuranceClaimComponent extends AppComponentBase implements OnInit {

  @Input() id: number;                // Claim Id
  @Input() insuranceName: string;
  @Input() patientName: string;
  @Input() invoiceId: number;

  @Output() onSave: EventEmitter<void> = new EventEmitter<void>();

  claim: InsuranceClaimDto;
  invoice: InvoiceDto;
  items: InvoiceItemDto[] = [];
  saving = false;

  ClaimStatus = ClaimStatus;
  statusList = [
    { label: 'Pending', value: 0 },
    { label: 'Submitted', value: 1 },
    // { label: 'Partial Approved', value:2  },
    { label: 'Approved', value:3  },
    { label: 'Rejected', value: 4 },
    { label: 'Paid', value: 5 },
  ];
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _insuranceService: InsuranceClaimServiceProxy,
    private _invoiceService: InvoiceServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadClaim();
  }

  loadClaim(): void {
    this._insuranceService.getByInvoiceId(this.invoiceId).subscribe(claimResult => {
      this.claim = claimResult;

      this._invoiceService.getInvoiceWithItems(this.invoiceId).subscribe(inv => {
        this.invoice = inv;

        // Properly map to InvoiceItemDto instances
        this.items = inv.items.map(x => {
          const itemDto = new InvoiceItemDto();
          itemDto.id = x.id;
          itemDto.invoiceId = x.invoiceId;
          itemDto.description = x.description;
          itemDto.quantity = x.quantity;
          itemDto.unitPrice = x.unitPrice;
          itemDto.entryDate = x.entryDate;
          itemDto.isCoveredByInsurance = x.isCoveredByInsurance ?? false;
          itemDto.approvedAmount = x.approvedAmount ?? 0;
          itemDto.notApprovedAmount = (x.unitPrice * x.quantity) - (x.approvedAmount ?? 0);
          return itemDto;
        });

        this.cd.detectChanges();
      });

      this.cd.detectChanges();
    });
  }


  getStatusLabel(status: ClaimStatus): string {
    switch (status) {
      case ClaimStatus._0: return 'Pending';
      case ClaimStatus._1: return 'Submitted';
      case ClaimStatus._2: return 'Partial Approved';
      case ClaimStatus._3: return 'Approved';
      case ClaimStatus._4: return 'Rejected';
      case ClaimStatus._5: return 'Paid';
      default: return '';
    }
  }
  getTotal(item: InvoiceItemDto): number {
    return item.unitPrice * item.quantity;
  }
  onApprovedChange(item: InvoiceItemDto): void {
    const total = this.getTotal(item);
    if (item.approvedAmount > total) item.approvedAmount = total;
    if (item.approvedAmount < 0 || !item.approvedAmount) item.approvedAmount = 0;

    item.notApprovedAmount = total - item.approvedAmount;
  }

  onCoveredChange(item: InvoiceItemDto): void {
    if (!item.isCoveredByInsurance) {
      item.approvedAmount = 0;
      item.notApprovedAmount = this.getTotal(item);
    } else {
      item.notApprovedAmount = this.getTotal(item) - (item.approvedAmount ?? 0);
    }
  }

  getNotApproved(item: InvoiceItemDto): number {
    if (!item.isCoveredByInsurance) return this.getTotal(item);
    return item.notApprovedAmount ?? this.getTotal(item);
  }

  isEditable(): boolean {
    return this.claim && (this.claim.status === ClaimStatus._0 || this.claim.status === ClaimStatus._1);
  }

  save(): void {
    this.saving = true;

    const input: CreateUpdateInsuranceClaimDto = new CreateUpdateInsuranceClaimDto();
    input.id = this.claim.id;
    input.status = this.claim.status;

    // Map items properly
    input.items = this.items.map(x => {
      const itemDto = new InvoiceItemDto();
      itemDto.id = x.id;
      itemDto.invoiceId = x.invoiceId;
      itemDto.description = x.description;
      itemDto.quantity = x.quantity;
      itemDto.unitPrice = x.unitPrice;
      itemDto.entryDate = x.entryDate;
      itemDto.isCoveredByInsurance = x.isCoveredByInsurance;
      itemDto.approvedAmount = x.approvedAmount;
      itemDto.notApprovedAmount = x.notApprovedAmount;
      return itemDto;
    });

    // Calculate totals
    input.amountPayByInsurance = input.items.reduce((sum, i) => sum + (i.approvedAmount ?? 0), 0);
    input.amountPayByPatient = input.items.reduce((sum, i) => sum + (i.notApprovedAmount ?? 0), 0);
    this._insuranceService.update(input)
      .pipe(finalize(() => this.saving = false))
      .subscribe(() => {
        this.notify.success(this.l('ClaimUpdatedSuccessfully'));
        this.onSave.emit();
        this.bsModalRef.hide();
      });
  }

}
