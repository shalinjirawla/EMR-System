import { Component, ViewChild, Injector, ChangeDetectorRef, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { PaginatorModule, Paginator } from "primeng/paginator";
import { TableModule, Table } from "primeng/table";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { LocalizePipe } from "../../../shared/pipes/localize.pipe";
import { PrescriptionServiceProxy, PrescriptionDto, PrescriptionDtoPagedResultDto } from "../../../shared/service-proxies/service-proxies";
import { CreatePrescriptionsComponent } from "../create-prescriptions/create-prescriptions.component";
import { EditPrescriptionsComponent } from "../edit-prescriptions/edit-prescriptions.component";
import { PagedListingComponentBase } from "@shared/paged-listing-component-base";
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { CommonModule, NgIf } from '@angular/common';
import { DatePipe } from "@angular/common";
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { ViewPrescriptionComponent } from '@app/doctors/view-prescription/view-prescription.component'

import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';

import moment from 'moment';
@Component({
  selector: 'app-prescriptions',
  animations: [appModuleAnimation()],
  imports: [FormsModule, TableModule, PrimeTemplate, CalendarModule, NgIf, PaginatorModule, InputTextModule,
    BreadcrumbModule, TooltipModule, CardModule, TagModule, AvatarModule, AvatarGroupModule, SelectModule, CheckboxModule,
    ButtonModule, LocalizePipe, DatePipe, CommonModule, OverlayPanelModule, MenuModule],
  templateUrl: './prescriptions.component.html',
  styleUrl: './prescriptions.component.css',
  providers: [PrescriptionServiceProxy]
})
export class PrescriptionsComponent extends PagedListingComponentBase<PrescriptionDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  items: MenuItem[] | undefined;
  editDeleteMenus: MenuItem[] | undefined;
  prescriptions: PrescriptionDto[] = [];
  keyword = '';
  dateRange: Date[];
  isActive: boolean | null;
  advancedFiltersVisible = false;
  showDoctorColumn: boolean = false;
  showNurseColumn: boolean = false;
  selectedRecord: PrescriptionDto;
  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _prescriptionService: PrescriptionServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }
  ngOnInit(): void {
    this.GetLoggedInUserRole();
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Prescription' },
    ];
    this.editDeleteMenus = [
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => this.editPrescription(this.selectedRecord)  // call edit
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.delete(this.selectedRecord)  // call delete
      },
      {
        label: 'View',
        icon: 'pi pi-eye',
        command: () => this.viewPrescription(this.selectedRecord)  // call delete
      }
    ];
  }
  clearFilters(): void {
    this.keyword = '';
    this.dateRange = [];
    this.list();
  }
  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    const fromDate = this.dateRange?.[0] ? moment(this.dateRange[0]) : undefined;
    const toDate = this.dateRange?.[1] ? moment(this.dateRange[1]) : undefined;

    this.primengTableHelper.showLoadingIndicator();

    this._prescriptionService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        fromDate,
        toDate,
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => {
        this.primengTableHelper.hideLoadingIndicator();
      }))
      .subscribe((result: PrescriptionDtoPagedResultDto) => {
        const filteredItems = result.items.filter(x => !x.isEmergencyPrescription);
        this.primengTableHelper.records = filteredItems;
        this.primengTableHelper.totalRecordsCount = filteredItems.length;
        this.cd.detectChanges();
      });
  }
  protected delete(entity: PrescriptionDto): void {
    abp.message.confirm("Are you sure u want to delete this", undefined, (result: boolean) => {
      if (result) {
        this._prescriptionService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
  createPrescription(): void {
    this.showCreateOrEditPrescriptionDialog();
  }
  editPrescription(dto: PrescriptionDto): void {
    this.showCreateOrEditPrescriptionDialog(dto.id);
  }
  viewPrescription(dto: PrescriptionDto): void {
    this.showViewPrescriptionDialog(dto.id);
  }

  private showViewPrescriptionDialog(id: number): void {
    const viewPrescriptionDialog: BsModalRef = this._modalService.show(
      ViewPrescriptionComponent,
      {
        class: 'modal-lg',
        initialState: {
          id: id
        }
      }
    );
  }
  showCreateOrEditPrescriptionDialog(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (!id) {
      createOrEditUserDialog = this._modalService.show(CreatePrescriptionsComponent, {
        class: 'modal-lg',
      });
    }
    else {
      createOrEditUserDialog = this._modalService.show(EditPrescriptionsComponent, {
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
  GetLoggedInUserRole() {
    this._prescriptionService.getCurrentUserRoles().subscribe(res => {
      this.showDoctorColumn = false;
      this.showNurseColumn = false;
      if (res && Array.isArray(res)) {
        if (res.includes('Admin')) {
          this.showDoctorColumn = true;
          this.showNurseColumn = true;
        } else if (res.includes('Doctors')) {
          this.showNurseColumn = true;
        } else if (res.includes('Nurse')) {
          this.showDoctorColumn = true;
        }
      }
      this.cd.detectChanges();
    });
  }
}
