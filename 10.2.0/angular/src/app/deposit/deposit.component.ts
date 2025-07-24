import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { DepositDto, DepositDtoPagedResultDto, DepositServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateDepositComponent } from './create-deposit/create-deposit.component';
import { EditDepositComponent } from './edit-deposit/edit-deposit.component';

@Component({
  selector: 'app-deposit',
  imports: [LocalizePipe, TableModule, PaginatorModule, FormsModule, DatePipe, NgIf, PrimeTemplate, ChipModule, OverlayPanelModule, MenuModule, ButtonModule],
  animations: [appModuleAnimation()],
  providers: [DepositServiceProxy],
  templateUrl: './deposit.component.html',
  styleUrl: './deposit.component.css'
})
export class DepositComponent extends PagedListingComponentBase<DepositDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  deposits: DepositDto[] = [];
  keyword = '';
  advancedFiltersVisible = false;

  constructor(
      injector: Injector,
      private _modalService: BsModalService,
      private _activatedRoute: ActivatedRoute,
      private _depositService: DepositServiceProxy,
      cd: ChangeDetectorRef,
  ) {
      super(injector, cd);
      this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  ngOnInit(): void {}

  clearFilters(): void {
      this.keyword = '';
      this.list();
  }

  list(event?: LazyLoadEvent): void {
      if (this.primengTableHelper.shouldResetPaging(event)) {
          this.paginator.changePage(0);

          if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
              return;
          }
      }
      this.primengTableHelper.showLoadingIndicator();
      this._depositService
          .getAll(
              this.primengTableHelper.getSorting(this.dataTable),
              this.primengTableHelper.getSkipCount(this.paginator, event),
              this.primengTableHelper.getMaxResultCount(this.paginator, event)
          )
          .pipe(
              finalize(() => {
                  this.primengTableHelper.hideLoadingIndicator();
              })
          )
          .subscribe((result: DepositDtoPagedResultDto) => {
            
              this.primengTableHelper.records = result.items;
              this.primengTableHelper.totalRecordsCount = result.totalCount;
              this.primengTableHelper.hideLoadingIndicator();
              this.cd.detectChanges();
          });
  }

  delete(deposit: DepositDto): void {
      abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
          if (result) {
              this._depositService.delete(deposit.id).subscribe(() => {
                  abp.notify.success(this.l('SuccessfullyDeleted'));
                  this.refresh();
              });
          }
      });
  }

  createDeposit(): void {
      this.showCreateOrEditDepositDialog();
  }
  editDeposit(dto: DepositDto): void {
      this.showCreateOrEditDepositDialog(dto.id);
  }
  showCreateOrEditDepositDialog(id?: number): void {
      let createOrEditDepositDialog: BsModalRef;
      if (!id) {
          createOrEditDepositDialog = this._modalService.show(CreateDepositComponent, {
              class: 'modal-lg',
          });
      }
      else {
          createOrEditDepositDialog = this._modalService.show(EditDepositComponent, {
              class: 'modal-lg',
              initialState: {
                  id: id,
              },
          });
      }
      createOrEditDepositDialog.content.onSave.subscribe(() => {
          this.refresh();
      });
  }

  getPaymentMethodLabel(paymentMethod: number): string {
    switch (paymentMethod) {
      case 1: return 'Card';
      case 0: return 'Cash';
      default: return 'Unknown';
    }
  }
}
