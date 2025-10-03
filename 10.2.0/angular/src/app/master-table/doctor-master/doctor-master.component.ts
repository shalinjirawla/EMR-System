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
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { DoctorMasterDto, DoctorMasterDtoPagedResultDto, DoctorMasterServiceProxy, RoomFacilityMasterDto, RoomFacilityMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateupdateDoctorMasterComponent } from '../createupdate-doctor-master/createupdate-doctor-master.component';
import { MenuItem } from 'primeng/api';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
@Component({
    selector: 'app-doctor-master',
    imports: [FormsModule, TableModule, CardModule, BreadcrumbModule, TooltipModule, ChipModule, SelectModule, MenuModule, ButtonModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe],
    providers: [DoctorMasterServiceProxy],
    standalone: true,
    animations: [appModuleAnimation()],
    templateUrl: './doctor-master.component.html',
    styleUrl: './doctor-master.component.css'
})
export class DoctorMasterComponent extends PagedListingComponentBase<DoctorMasterDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    doctors: DoctorMasterDto[] = [];
    selectedRecord: DoctorMasterDto;
    items: MenuItem[];
    editDeleteMenus: MenuItem[];
    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _doctorService: DoctorMasterServiceProxy,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
    }

    ngOnInit(): void {
        this.items = [{ label: 'Home', routerLink: '/' }, { label: 'Doctor Master' }];

        this.editDeleteMenus = [
            { label: 'Edit', icon: 'pi pi-pencil', command: () => this.selectedRecord && this.editDoctor(this.selectedRecord) },
            { label: 'Delete', icon: 'pi pi-trash', command: () => this.selectedRecord && this.deleteDoctor(this.selectedRecord) }
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
        this._doctorService
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
            .subscribe((result: DoctorMasterDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    createDoctor(): void {
        let createDialog: BsModalRef = this._modalService.show(CreateupdateDoctorMasterComponent, { class: 'modal-lg' });
        createDialog.content.onSave.subscribe(() => {
            this.list();
        });
    }

    editDoctor(doctor: DoctorMasterDto): void {
        let editDialog: BsModalRef = this._modalService.show(CreateupdateDoctorMasterComponent, { class: 'modal-lg', initialState: { id: doctor.id } });
        editDialog.content.onSave.subscribe(() => {
            this.list();
        });
    }

    deleteDoctor(doctor: DoctorMasterDto): void {
        abp.message.confirm('Are you sure you want to delete this doctor?', undefined, (result: boolean) => {
            if (result) {
                this._doctorService.delete(doctor.id).subscribe(() => {
                    abp.notify.success('Deleted successfully');
                    this.list();
                });
            }
        });
    }

    delete(entity: DoctorMasterDto): void {
        this.deleteDoctor(entity);
    }

    clearFilters(): void {
        this.list();
    }
}
