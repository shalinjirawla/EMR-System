import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { AppointmentTypeDto, AppointmentTypeDtoPagedResultDto, AppointmentTypeServiceProxy, RoomFacilityMasterDto, RoomFacilityMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateupdateAppointmentTypesComponent } from '../createupdate-appointment-types/createupdate-appointment-types.component';
import { MenuItem } from 'primeng/api';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { MenuModule } from 'primeng/menu';
@Component({
  selector: 'app-appointment-types',
  imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, 
    CardModule, BreadcrumbModule, TooltipModule, MenuModule,],
  providers: [AppointmentTypeServiceProxy],
  animations: [appModuleAnimation()],
  standalone: true,
  templateUrl: './appointment-types.component.html',
  styleUrl: './appointment-types.component.css'
})
export class AppointmentTypesComponent extends PagedListingComponentBase<AppointmentTypeDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  appointmentTypes: AppointmentTypeDto[] = [];
    selectedRecord: AppointmentTypeDto;
  items: MenuItem[];
  editDeleteMenus: MenuItem[];
  constructor(
      injector: Injector,
      private _modalService: BsModalService,
      private _appointmentTypesService: AppointmentTypeServiceProxy,
      private _activatedRoute: ActivatedRoute,
      cd: ChangeDetectorRef
  ) {
      super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [{ label: 'Home', routerLink: '/' }, { label: 'Appointment Types' }];

    this.editDeleteMenus = [
      { label: 'Edit', icon: 'pi pi-pencil', command: () => this.selectedRecord && this.editAppointmentType(this.selectedRecord) },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.selectedRecord && this.deleteAppointmentType(this.selectedRecord) }
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
      this._appointmentTypesService
          .getAll(
              this.primengTableHelper.getSkipCount(this.paginator, event),
              this.primengTableHelper.getMaxResultCount(this.paginator, event)
          )
          .pipe(
              finalize(() => {
                  this.primengTableHelper.hideLoadingIndicator();
              })
          )
            .subscribe((result: AppointmentTypeDtoPagedResultDto) => {
              this.primengTableHelper.records = result.items;
              this.primengTableHelper.totalRecordsCount = result.totalCount;
              this.primengTableHelper.hideLoadingIndicator();
              this.cd.detectChanges();
          });
  }

  createAppointmentType(): void {
    let createDialog: BsModalRef = this._modalService.show(CreateupdateAppointmentTypesComponent, { class: 'modal-lg' });
    createDialog.content.onSave.subscribe(() => {
      this.list();
    });
  }

  editAppointmentType(appointmentType: AppointmentTypeDto): void {
    let editDialog: BsModalRef = this._modalService.show(CreateupdateAppointmentTypesComponent, { class: 'modal-lg', initialState: { id: appointmentType.id } });
    editDialog.content.onSave.subscribe(() => {
      this.list();
    });
  }

  deleteAppointmentType(appointmentType: AppointmentTypeDto): void {
      abp.message.confirm('Are you sure you want to delete this appointment type?', undefined, (result: boolean) => {
          if (result) {
              this._appointmentTypesService.delete(appointmentType.id).subscribe(() => {
                  abp.notify.success('Deleted successfully');
                  this.list();
              });
          }
      });
  }

  delete(entity: AppointmentTypeDto): void {
      this.deleteAppointmentType(entity);
  }

  clearFilters(): void {
      this.list();
  }
}