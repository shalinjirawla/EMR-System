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
import { RoomTypeMasterDto, RoomTypeMasterDtoPagedResultDto, RoomTypeMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import { SelectModule } from 'primeng/select';
import { ChipModule } from 'primeng/chip';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { CreateupdateRoomTypesComponent } from '../createupdate-room-types/createupdate-room-types.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';


@Component({
    selector: 'app-room-types',
    templateUrl: './room-types.component.html',
    styleUrls: ['./room-types.component.css'],
    providers: [RoomTypeMasterServiceProxy],
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, ChipModule, SelectModule, MenuModule, ButtonModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe],
})
export class RoomTypesComponent extends PagedListingComponentBase<RoomTypeMasterDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    roomTypes: RoomTypeMasterDto[] = [];

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _roomTypesService: RoomTypeMasterServiceProxy,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
    }

    ngOnInit(): void {
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }
        this.primengTableHelper.showLoadingIndicator();
        this._roomTypesService
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
            .subscribe((result: RoomTypeMasterDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.roomTypes = result.items;
                this.cd.detectChanges();
            });
    }

    createRoomType(): void {
      let createDialog: BsModalRef = this._modalService.show(CreateupdateRoomTypesComponent, { class: 'modal-lg' });
      createDialog.content.onSave.subscribe(() => {
        this.list();
      });
    }

    editRoomType(roomType: RoomTypeMasterDto): void {
      let editDialog: BsModalRef = this._modalService.show(CreateupdateRoomTypesComponent, { class: 'modal-lg', initialState: { id: roomType.id } });
      editDialog.content.onSave.subscribe(() => {
        this.list();
      });
    }

    deleteRoomType(roomType: RoomTypeMasterDto): void {
        abp.message.confirm('Are you sure you want to delete this room type?', undefined, (result: boolean) => {
            if (result) {
                this._roomTypesService.delete(roomType.id).subscribe(() => {
                    abp.notify.success('Deleted successfully');
                    this.list();
                });
            }
        });
    }

    delete(entity: RoomTypeMasterDto): void {
        this.deleteRoomType(entity);
    }

    clearFilters(): void {
        this.list();
    }
}
