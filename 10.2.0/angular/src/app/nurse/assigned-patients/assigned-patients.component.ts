import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { UserServiceProxy, UserDto, UserDtoPagedResultDto, PatientDto, PatientServiceProxy, PatientDtoPagedResultDto, NurseServiceProxy, PatientsForDoctorAndNurseDtoPagedResultDto, PatientDischargeServiceProxy, DischargeStatus } from '@shared/service-proxies/service-proxies';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { CheckboxModule } from 'primeng/checkbox';
@Component({
  selector: 'app-assigned-patients',
  templateUrl: './assigned-patients.component.html',
  styleUrl: './assigned-patients.component.css',
  animations: [appModuleAnimation()],
  imports: [ConfirmDialog, FormsModule, CheckboxModule, BreadcrumbModule, SelectModule, InputTextModule, TagModule, TableModule, PrimeTemplate, ConfirmDialogModule, NgIf, PaginatorModule, LocalizePipe, OverlayPanelModule, MenuModule, ButtonModule,],
  providers: [PatientServiceProxy, NurseServiceProxy, PatientDischargeServiceProxy, ConfirmationService],
})
export class AssignedPatientsComponent extends PagedListingComponentBase<PatientDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  users: PatientDto[] = [];
  keyword = '';
  items: MenuItem[] = [];
  isActive: boolean | null;
  advancedFiltersVisible = false;
  nurseID: number;
  statusOptions = [
    { label: 'Pending', value: DischargeStatus._0 },
    { label: 'Initiated', value: DischargeStatus._1 },
    { label: 'Sent To Doctor', value: DischargeStatus._2 },
    { label: 'Doctor Verified', value: DischargeStatus._3 },
    { label: 'Sent To LabTechnician', value: DischargeStatus._4 },
    { label: 'LabTechnician Completed', value: DischargeStatus._5 },
    { label: 'Sent To Billing', value: DischargeStatus._6 },
    { label: 'Billing Completed', value: DischargeStatus._7 },
    { label: 'Final Approval', value: DischargeStatus._8 },
    { label: 'Discharged', value: DischargeStatus._9 },
  ];
  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _modalService: BsModalService,
    private _patientService: PatientServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _patientDischargeService: PatientDischargeServiceProxy,
    private confirmationService: ConfirmationService,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }
  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: this.l('Patients') }
    ];
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
  }
  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);

      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();
    this._patientService
      .patientsForNurse(

        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(
        finalize(() => {
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe((result: PatientsForDoctorAndNurseDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
        this.cd.detectChanges();
      });
  }
  delete(user: PatientDto): void {
    abp.message.confirm(this.l('UserDeleteWarningMessage', user.fullName), undefined, (result: boolean) => {
      if (result) {
        this._userService.delete(user.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
  FetchNurseID() {
    this._nurseService.getNurseDetailsByAbpUserID(abp.session.userId).subscribe({
      next: (res) => {
        this.nurseID = res.id;
      }, error: (err) => {

      }
    })
  }
  calculateAge(dob: string | Date): number {
    const birthDate = new Date(dob);
    const today = new Date();

    let age = today.getFullYear() - birthDate.getFullYear();

    const hasBirthdayPassedThisYear =
      today.getMonth() > birthDate.getMonth() ||
      (today.getMonth() === birthDate.getMonth() && today.getDate() >= birthDate.getDate());

    if (!hasBirthdayPassedThisYear) {
      age--;
    }

    return age;
  }
  initiateDischarge(record: any) {
    this._patientDischargeService
      .dischargeStatusChange(record.id, DischargeStatus._1)
      .subscribe(() => {
        abp.notify.success("Discharge initiated successfully.");
        this.refresh(); // Refresh table
      });
  }
  confirmInitiate(record: any) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to initiate discharge for ' + record.fullName + ' ?',
      header: 'Confirm Discharge',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.initiateDischarge(record);
      }
    });
  }
  getStatusLabel(value: number): string {
    const status = this.statusOptions.find(s => s.value === value);
    const dataa = status ? status.label : '';
    return dataa;
  }
  getStatusSeverity(value: number) {
    switch (value) {
      case DischargeStatus._0:
      case DischargeStatus._1:
      case DischargeStatus._2:
      case DischargeStatus._3:
      case DischargeStatus._4:
      case DischargeStatus._5:
      case DischargeStatus._6:
      case DischargeStatus._7:
            return 'badge-soft-primary p-1 rounded';
      case DischargeStatus._8:
      case DischargeStatus._9:
            return 'badge-soft-success p-1 rounded';
      default: return 'badge-soft-teal p-1 rounded';
    }
  }
}
