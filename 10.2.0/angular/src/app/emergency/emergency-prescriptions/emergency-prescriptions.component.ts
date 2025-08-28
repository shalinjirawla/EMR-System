import { Component, ViewChild, Injector, ChangeDetectorRef, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { PaginatorModule, Paginator } from "primeng/paginator";
import { TableModule, Table } from "primeng/table";
import { appModuleAnimation } from "../../../shared/animations/routerTransition";
import { LocalizePipe } from "../../../shared/pipes/localize.pipe";
import { PrescriptionServiceProxy, PrescriptionDto, PrescriptionDtoPagedResultDto } from "../../../shared/service-proxies/service-proxies";
import { CreateUpdateEmergencyPrescriptionsComponent } from "../create-update-emergency-prescriptions/create-update-emergency-prescriptions.component";
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
import moment from 'moment';
@Component({
  selector: 'app-emergency-prescriptions',
  animations: [appModuleAnimation()],
  imports: [FormsModule, TableModule, PrimeTemplate, CalendarModule, NgIf, PaginatorModule, ButtonModule, LocalizePipe, DatePipe, CommonModule, OverlayPanelModule, MenuModule],
  templateUrl: './emergency-prescriptions.component.html',
  styleUrl: './emergency-prescriptions.component.css',
  providers: [PrescriptionServiceProxy]
})
export class EmergencyPrescriptionsComponent extends PagedListingComponentBase<PrescriptionDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  prescriptions: PrescriptionDto[] = [];
  keyword = '';
  dateRange: Date[];
  isActive: boolean | null;
  advancedFiltersVisible = false;
  showDoctorColumn: boolean = false;
  showNurseColumn: boolean = false;
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
        const filteredList = result.items;
        this.primengTableHelper.records = filteredList.filter(x => x.isEmergencyPrescription == true);
        this.primengTableHelper.totalRecordsCount = filteredList.length;
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
      createOrEditUserDialog = this._modalService.show(CreateUpdateEmergencyPrescriptionsComponent, {
        class: 'modal-lg',
      });
    }
    else {
      createOrEditUserDialog = this._modalService.show(CreateUpdateEmergencyPrescriptionsComponent, {
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
