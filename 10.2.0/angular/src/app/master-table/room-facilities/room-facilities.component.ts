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
import { CreateupdateRoomFacilitiesComponent } from '../createupdate-room-facilities/createupdate-room-facilities.component';
import { RoomFacilityMasterDto, RoomFacilityMasterDtoPagedResultDto, RoomFacilityMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import { MenuItem } from 'primeng/api';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
@Component({
    selector: 'app-room-facilities',
    templateUrl: './room-facilities.component.html',
    styleUrls: ['./room-facilities.component.css'],
    providers: [RoomFacilityMasterServiceProxy],
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, CardModule, BreadcrumbModule, TooltipModule, TableModule, ChipModule, SelectModule, MenuModule, ButtonModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe],
})
export class RoomFacilitiesComponent extends PagedListingComponentBase<RoomFacilityMasterDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    facilities: RoomFacilityMasterDto[] = [];
    selectedRecord: RoomFacilityMasterDto;
    items: MenuItem[];
    editDeleteMenus: MenuItem[];
    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _roomFacilitiesService: RoomFacilityMasterServiceProxy,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
    }

    ngOnInit(): void {
        this.items = [{ label: 'Home', routerLink: '/' }, { label: 'Room Facilities' }];

        this.editDeleteMenus = [
            {
                label: 'Edit',
                icon: 'pi pi-pencil',
                command: () => {
                    if (this.selectedRecord) {
                        this.editRoomFacility(this.selectedRecord);
                    }
                }
            },
            {
                label: 'Delete',
                icon: 'pi pi-trash',
                command: () => {
                    if (this.selectedRecord) {
                        this.deleteRoomFacility(this.selectedRecord);
                    }
                }
            }
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
        this._roomFacilitiesService
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
            .subscribe((result: RoomFacilityMasterDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    createRoomFacility(): void {
        let createDialog: BsModalRef = this._modalService.show(CreateupdateRoomFacilitiesComponent, { class: 'modal-lg' });
        createDialog.content.onSave.subscribe(() => {
            this.list();
        });
    }

    editRoomFacility(facility: RoomFacilityMasterDto): void {
        let editDialog: BsModalRef = this._modalService.show(CreateupdateRoomFacilitiesComponent, { class: 'modal-lg', initialState: { id: facility.id } });
        editDialog.content.onSave.subscribe(() => {
            this.list();
        });
    }

    deleteRoomFacility(facility: RoomFacilityMasterDto): void {
        abp.message.confirm('Are you sure you want to delete this facility?', undefined, (result: boolean) => {
            if (result) {
                this._roomFacilitiesService.delete(facility.id).subscribe(() => {
                    abp.notify.success('Deleted successfully');
                    this.list();
                });
            }
        });
    }

    delete(entity: RoomFacilityMasterDto): void {
        this.deleteRoomFacility(entity);
    }

    clearFilters(): void {
        this.list();
    }
}
