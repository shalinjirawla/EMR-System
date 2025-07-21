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
import { AdmissionDto, AdmissionDtoPagedResultDto, AdmissionServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateAddmissionComponent } from './create-addmission/create-addmission.component';
import { EditAddmissionComponent } from './edit-addmission/edit-addmission.component';
@Component({
  selector: 'app-admission',
  imports: [LocalizePipe, TableModule, PaginatorModule, FormsModule, DatePipe, NgIf, PrimeTemplate, ChipModule, OverlayPanelModule, MenuModule, ButtonModule],
  animations: [appModuleAnimation()],
  providers: [AdmissionServiceProxy],
  templateUrl: './admission.component.html',
  styleUrl: './admission.component.css'
})
export class AdmissionComponent extends PagedListingComponentBase<AdmissionDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  admissions: AdmissionDto[] = [];
  keyword = '';
  status: number;
  advancedFiltersVisible = false;
 
  appointmentStatus!: any;
  constructor(
      injector: Injector,
      private _modalService: BsModalService,
      private _activatedRoute: ActivatedRoute,
      private _admissionService: AdmissionServiceProxy,
      cd: ChangeDetectorRef,
  ) {
      super(injector, cd);
      this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }
  ngOnInit(): void {
  }
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
      this._admissionService
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
          .subscribe((result: AdmissionDtoPagedResultDto) => {
              this.primengTableHelper.records = result.items;
              this.primengTableHelper.totalRecordsCount = result.totalCount;
              this.primengTableHelper.hideLoadingIndicator();
              this.cd.detectChanges();
          });
  }
  delete(admission: AdmissionDto): void {
      abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
          if (result) {
              this._admissionService.delete(admission.id).subscribe(() => {
                  abp.notify.success(this.l('SuccessfullyDeleted'));
                  this.refresh();
              });
          }
      });
  }
  createAdmission(): void {
      this.showCreateOrEditAppoinmentDialog();
  }
  editAdmission(dto: AdmissionDto): void {
      this.showCreateOrEditAppoinmentDialog(dto.id);
  }
  showCreateOrEditAppoinmentDialog(id?: number): void {
      let createOrEditUserDialog: BsModalRef;
      if (!id) {
          createOrEditUserDialog = this._modalService.show(CreateAddmissionComponent, {
              class: 'modal-lg',
          });
      }
      else {
          createOrEditUserDialog = this._modalService.show(EditAddmissionComponent, {
              class: 'modal-lg',
              initialState: {
                  id: id,
              },
          });
      }

      createOrEditUserDialog.content.onSave.subscribe(() => {
          this.refresh();
      });
  }
 
}
