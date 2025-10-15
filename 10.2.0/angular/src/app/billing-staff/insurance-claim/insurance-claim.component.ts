import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { CommonModule, DatePipe, NgIf } from '@node_modules/@angular/common';
import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { MenuModule } from 'primeng/menu';
import { InsuranceClaimDto, InsuranceClaimDtoPagedResultDto, InsuranceClaimServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ViewInsuranceClaimComponent } from '../view-insurance-claim/view-insurance-claim.component';
import { EditInsuranceClaimComponent } from '../edit-insurance-claim/edit-insurance-claim.component';


@Component({
  selector: 'app-insurance-claim',
  imports: [FormsModule, TableModule, CardModule, MenuModule, BreadcrumbModule, InputTextModule, TooltipModule,
    PrimeTemplate, NgIf, PaginatorModule, OverlayPanelModule, ButtonModule, CommonModule, LocalizePipe],
  animations: [appModuleAnimation()],
  providers: [InsuranceClaimServiceProxy],
  templateUrl: './insurance-claim.component.html',
  styleUrl: './insurance-claim.component.css'
})
export class InsuranceClaimComponent extends PagedListingComponentBase<InsuranceClaimDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  items: MenuItem[];
  selectedRecord: InsuranceClaimDto;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _claimService: InsuranceClaimServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Insurance Claims' },
    ];
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();

    this._claimService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: InsuranceClaimDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  // ----------------------------
  // 🎯 Conditional Action Menus
  // ----------------------------
  getMenuForStatus(status: number): MenuItem[] {
    switch (status) {
      case 0: // Pending
        return [
          {
            label: 'Submit Application',
            icon: 'pi pi-upload',
            command: () => this.submitClaim(this.selectedRecord),
          },
        ];

      case 1: // Submitted
        return [
          {
            label: 'Edit Claim',
            icon: 'pi pi-pencil',
            command: () => this.editClaim(this.selectedRecord),
          },
        ];

      case 2: // PartialApproved
      case 3: // Approved
      case 4: // Rejected
      case 5: // Paid
        return [
          {
            label: 'View Claim',
            icon: 'pi pi-eye',
            command: () => this.viewClaim(this.selectedRecord),
          },
        ];

      default:
        return [];
    }
  }

  submitClaim(claim: InsuranceClaimDto): void {
    abp.message.confirm(
      `Are you sure you want to submit the insurance claim for ${claim.patientName}?`,
      'Confirm',
      (result: boolean) => {
        if (result) {
          this._claimService.submitClaim(claim.id).subscribe(() => {
            this.notify.success('Claim submitted successfully.');
            this.refresh();
          });
        }
      }
    );
  }
  delete(claim: InsuranceClaimDto): void {
    abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
      if (result) {
        this._claimService.delete(claim.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
  editClaim(claim: InsuranceClaimDto): void {
    const editDialog: BsModalRef = this._modalService.show(EditInsuranceClaimComponent, {
      class: 'modal-xl',
      initialState: { id: claim.id, insuranceName: claim.insuranceName, patientName: claim.patientName, invoiceId: claim.invoiceId },
    });
    editDialog.content.onSave.subscribe(() => this.refresh());
  }

  viewClaim(claim: InsuranceClaimDto): void {
    this._modalService.show(ViewInsuranceClaimComponent, {
      class: 'modal-lg',
      initialState: { invoiceId: claim.invoiceId },

    });
  }

  getStatusText(status: number): string {
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Submitted';
      case 2: return 'Partial Approved';
      case 3: return 'Approved';
      case 4: return 'Rejected';
      case 5: return 'Paid';
      default: return '';
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case 0: return 'badge-soft-warning p-1 rounded';
      case 1: return 'badge-soft-primary p-1 rounded';
      case 2: return 'badge-soft-primary p-1 rounded';
      case 3: return 'badge-soft-success p-1 rounded';
      case 4: return 'badge-soft-danger p-1 rounded';
      case 5: return 'badge-soft-secondary p-1 rounded';
      default: return 'badge-soft-teal p-1 rounded';
    }
  }
}