import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { RoomDto, RoomDtoPagedResultDto, RoomServiceProxy, RoomStatus } from '@shared/service-proxies/service-proxies';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { NgIf } from '@angular/common';
import { EditRoomComponent } from '../room/edit-room/edit-room.component'
import { AddRoomComponent } from '../room/add-room/add-room.component'
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ChipModule } from 'primeng/chip';
import { TagModule } from 'primeng/tag';
import { RadioButtonModule } from 'primeng/radiobutton';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { InputTextModule } from 'primeng/inputtext';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
@Component({
    selector: 'app-room',
    imports: [FormsModule,BreadcrumbModule,InputTextModule,CardModule, TooltipModule,TableModule,RadioButtonModule, TagModule, SelectModule, MenuModule,
        ButtonModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, ChipModule, LocalizePipe],
    animations: [appModuleAnimation()],
    providers: [RoomServiceProxy],
    templateUrl: './room.component.html',
    styleUrl: './room.component.css'
})
export class RoomComponent extends PagedListingComponentBase<RoomDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    rooms: RoomDto[] = [];
    keyword = '';
    roomTypeMasterId: number | undefined;
    selectedRecord: RoomDto;
    statusFilter: string = 'all';
    status: RoomStatus | undefined;   // default = “All”
    editDeleteMenus: MenuItem[];
    items: MenuItem[] | undefined;

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _roomService: RoomServiceProxy,
        cd: ChangeDetectorRef,
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }
    ngOnInit(): void {
        this.items = [
            { label: 'Home', routerLink: '/' },
            { label: 'Rooms' },
        ];

        this.editDeleteMenus = [
            { label: 'Edit', icon: 'pi pi-pencil', command: () => this.EditNewRoom(this.selectedRecord) },
            { label: 'Delete', icon: 'pi pi-trash', command: () => this.delete(this.selectedRecord) }
        ];
    }
    clearFilters(): void {
        this.keyword = '';
        this.status = undefined;
        this.list();
    }

    onStatusChange() {
        this.list(); // or this.list() depending on your implementation
    }
    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }
        this.primengTableHelper.showLoadingIndicator();
        this._roomService
            .getAll(
                this.keyword,
                this.roomTypeMasterId,
                this.status,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: RoomDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }
    delete(appMt: RoomDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
            if (result) {
                this._roomService.delete(appMt.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }
    AddNewRoom(): void {
        this.showCreateOrEditRoomDialog();
    }
    EditNewRoom(dto: RoomDto): void {
        this.showCreateOrEditRoomDialog(dto.id);
    }
    showCreateOrEditRoomDialog(id?: number): void {
        let CreateOrEditRoomDialog: BsModalRef;
        if (!id) {
            CreateOrEditRoomDialog = this._modalService.show(AddRoomComponent, {
                class: 'modal-lg',
            });
        }
        else {
            CreateOrEditRoomDialog = this._modalService.show(EditRoomComponent, {
                class: 'modal-lg',
                initialState: {
                    id: id,
                },
            });
        }

        CreateOrEditRoomDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }

    getStatusClass(status: RoomStatus): string {

        switch (status) {
            case RoomStatus._0: return 'status-available';
            case RoomStatus._1: return 'status-occupied';
            case RoomStatus._2: return 'status-reserved';
            case RoomStatus._3: return 'status-maint';
            default: return '';
        }
    }

    getStatusLabel(status: RoomStatus): string {

        switch (status) {
            case RoomStatus._0: return 'Available';
            case RoomStatus._1: return 'Occupied';
            case RoomStatus._2: return 'Reserved';
            case RoomStatus._3: return 'Under Maintenance';
            default: return '';
        }
    }

    getStatusSeverity(status: RoomStatus): 'info' | 'warn' | 'success' | 'danger' | 'secondary' | 'contrast' {
        switch (status) {
            case RoomStatus._0: return 'success';      // Available
            case RoomStatus._1: return 'danger';       // Occupied
            case RoomStatus._2: return 'info';         // Reserved
            case RoomStatus._3: return 'secondary';    // Maintenance
            default: return 'contrast';
        }
    }
    onFilterChange(selected: string) {
  switch (selected) {
    case 'all':
      this.status = undefined;
      break;
    case 'available':
      this.status = 0;
      break;
    case 'occupied':
      this.status = 1;
      break;
    case 'reserved':
      this.status = 2;
      break;
    case 'maintenance':
      this.status = 3;
      break;
  }
  this.list();
}

}
