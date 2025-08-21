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
import { EmergencyCaseDto, EmergencyCaseDtoPagedResultDto, EmergencyServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateEmergencyCaseComponent } from '../create-emergency-case/create-emergency-case.component';
import { EditEmergencyCaseComponent } from '../edit-emergency-case/edit-emergency-case.component';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-emergency-case',
  imports: [LocalizePipe, FormsModule, DatePipe,TagModule, NgIf, ButtonModule,PaginatorModule,TableModule,OverlayPanelModule],
  animations: [appModuleAnimation()],
  providers: [EmergencyServiceProxy],
  templateUrl: './emergency-case.component.html',
  styleUrl: './emergency-case.component.css'
})
export class EmergencyCaseComponent extends PagedListingComponentBase<EmergencyCaseDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  emergencyCases: EmergencyCaseDto[] = [];
  keyword = '';
  advancedFiltersVisible = false;
   modeOfArrival?: number;
  severity?: number;
  status?: number;

   modeOfArrivalOptions = [
    { label: 'Walk-In', value: 0 },
    { label: 'Ambulance', value: 1 },
    { label: 'Police', value: 2 },
    { label: 'Unknown', value: 3 }
  ];

  severityOptions = [
    { label: 'Critical', value: 0 },
    { label: 'Serious', value: 1 },
    { label: 'Stable', value: 2 }
  ];

  statusOptions = [
    { label: 'Ongoing', value: 0 },
    { label: 'Discharged', value: 1 },
    { label: 'Admitted', value: 2 },
    { label: 'Expired', value: 3 }
  ];

  constructor(
      injector: Injector,
      private _modalService: BsModalService,
      private _activatedRoute: ActivatedRoute,
      private _emergencyService: EmergencyServiceProxy,
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
      this._emergencyService
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
          .subscribe((result: EmergencyCaseDtoPagedResultDto) => {
              this.primengTableHelper.records = result.items;
              this.primengTableHelper.totalRecordsCount = result.totalCount;
              this.primengTableHelper.hideLoadingIndicator();
              this.cd.detectChanges();
          });
  }

  delete(emergencyCase: EmergencyCaseDto): void {
      abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
          if (result) {
              this._emergencyService.delete(emergencyCase.id).subscribe(() => {
                  abp.notify.success(this.l('SuccessfullyDeleted'));
                  this.refresh();
              });
          }
      });
  }

  createEmergencyCase(): void {
      this.showCreateOrEditEmergencyDialog();
  }

  editEmergencyCase(dto: EmergencyCaseDto): void {
      this.showCreateOrEditEmergencyDialog(dto.id);
  }

  showCreateOrEditEmergencyDialog(id?: number): void {
      let createOrEditDialog: BsModalRef;
      if (!id) {
          createOrEditDialog = this._modalService.show(CreateEmergencyCaseComponent, {
              class: 'modal-lg',
          });
      }
      else {
          createOrEditDialog = this._modalService.show(EditEmergencyCaseComponent, {
              class: 'modal-lg',
              initialState: {
                  id: id,
              },
          });
      }

      createOrEditDialog.content.onSave.subscribe(() => {
          this.refresh();
      });
  }
    getModeOfArrivalLabel(value: number): string {
    return this.modeOfArrivalOptions.find(x => x.value === value)?.label || '';
  }

  getSeverityLabel(value: number): string {
    return this.severityOptions.find(x => x.value === value)?.label || '';
  }

  getStatusLabel(value: number): string {
    return this.statusOptions.find(x => x.value === value)?.label || '';
  }

  getSeverityTag(value: number): 'danger' | 'warn' | 'success' {
    switch (value) {
      case 0: return 'danger';   // Critical
      case 1: return 'warn';     // Serious
      case 2: return 'success';  // Stable
      default: return 'warn';
    }
  }

  getStatusTag(value: number): 'info' | 'success' | 'secondary' | 'danger' {
    switch (value) {
      case 0: return 'info';       // Ongoing
      case 1: return 'success';    // Discharged
      case 2: return 'secondary';  // Admitted
      case 3: return 'danger';     // Expired
      default: return 'info';
    }
  }
}