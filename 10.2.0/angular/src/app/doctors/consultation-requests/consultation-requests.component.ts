import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ChipModule } from 'primeng/chip';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { ConsultationRequestsDto, ConsultationRequestsServiceProxy, Status } from '@shared/service-proxies/service-proxies';
import { PrimeTemplate } from 'primeng/api';
import { PaginatorModule } from 'primeng/paginator';
import { BreadcrumbModule } from 'primeng/breadcrumb';


@Component({
  selector: 'app-consultation-requests',
  templateUrl: './consultation-requests.component.html',
  styleUrls: ['./consultation-requests.component.css'],
  animations: [appModuleAnimation()],
  imports: [
    FormsModule, TableModule,BreadcrumbModule, DialogModule, TextareaModule, TagModule, CommonModule, PrimeTemplate,
    OverlayPanelModule, MenuModule, ButtonModule, NgIf, PaginatorModule, ChipModule, LocalizePipe
  ],
  providers: [ConsultationRequestsServiceProxy]
})
export class ConsultationRequestsComponent extends PagedListingComponentBase<ConsultationRequestsDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  visible: boolean = false;
  selectedRecords!: ConsultationRequestsDto;
  _adviceResponse!: string;

  showDoctorColumn: boolean = false;
  showNurseColumn: boolean = false;

  statusOptions = [
    { label: 'Pending', value: Status._0 },
    { label: 'In_review', value: Status._1 },
    { label: 'Completed', value: Status._2 },
  ];

  items = [
    { label: 'Home', routerLink: '/' },
    { label: 'Consultation Requests' },
  ];

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _consultationRequestsService: ConsultationRequestsServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.GetLoggedInUserRole();
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();

    this._consultationRequestsService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  refresh(): void {
    this.list();
  }

  delete(entity: ConsultationRequestsDto): void {
    abp.message.confirm("Are you sure you want to delete this?", undefined, (result: boolean) => {
      if (result) {
        this._consultationRequestsService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }

  GetLoggedInUserRole(): void {
    this._consultationRequestsService.getCurrentUserRoles().subscribe(res => {
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

  getStatusLabel(value: number): string {
    const status = this.statusOptions.find(s => s.value === value);
    return status ? status.label : '';
  }

  getStatusSeverity(value: number) {
    switch (value) {
      case Status._0: return 'badge-soft-warning p-1 rounded';
      case Status._1: return 'badge-soft-primary p-1 rounded';
      case Status._2: return 'badge-soft-success p-1 rounded';
      default: return 'badge-soft-teal p-1 rounded';
    }
  }

  showDialog(record: ConsultationRequestsDto): void {
    this.selectedRecords = record;
    this.visible = true;
    this._adviceResponse = record.adviceResponse || '';
  }

  UpdateStatus(record: ConsultationRequestsDto, status: Status): void {
    const dto: any = {
      id: record.id,
      tenantId: record.tenantId,
      prescriptionId: record.prescriptionId,
      requestingDoctorId: record.requestingDoctorId,
      requestedSpecialistId: record.requestedSpecialistId,
      status: status,
      notes: record.notes,
      adviceResponse: this._adviceResponse || null
    };
    this._consultationRequestsService.update(dto).subscribe(() => {
      this.visible = false;
      this.refresh();
    });
  }

  OnDialogBoxHide(): void {
    this.visible = false;
    this._adviceResponse = '';
  }

  isInPending(status: Status): boolean {
    return status === Status._0;
  }

  isInReview(status: Status): boolean {
    return status === Status._1;
  }

  isCompleted(status: Status): boolean {
    return status === Status._2;
  }
}
