import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { EmergencyMasterDto, EmergencyMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { NgIf } from '@angular/common';
import { CreateupdateEmergencyChargesComponent } from '../createupdate-emergency-charges/createupdate-emergency-charges.component'
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ChipModule } from 'primeng/chip';
import { TagModule } from 'primeng/tag';


@Component({
    selector: 'app-emergency-charges',
    imports: [FormsModule, TableModule, TagModule, SelectModule, MenuModule,
        ButtonModule, OverlayPanelModule, PrimeTemplate, NgIf, PaginatorModule, ChipModule, LocalizePipe],
    animations: [appModuleAnimation()],
    providers: [EmergencyMasterServiceProxy],
    templateUrl: './emergency-charges.component.html',
    styleUrl: './emergency-charges.component.css'
})
export class EmergencyChargesComponent extends PagedListingComponentBase<EmergencyMasterDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    isOneRecords = false;
    _list!: EmergencyMasterDto[];
    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _emergencyMasterService:EmergencyMasterServiceProxy,
        cd: ChangeDetectorRef,
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
        this._emergencyMasterService
            .getAll(
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: any) => {
                this.primengTableHelper.records = result.items;
                this._list = result.items;
                if (result.items.length >= 1) {
                    this.isOneRecords = true;
                }
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }
    delete(appMt: EmergencyMasterDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
            if (result) {
                this._emergencyMasterService.delete(appMt.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                    if (this.isOneRecords) {
                        this.isOneRecords = false;
                    }
                });
            }
        });
    }
    showCreateOrEditDialog(id?: number): void {
        if (!id) {
            this.create();
        }
        else {
            this.edit(id);
        }
    }

    create(): void {
        let createDialog: BsModalRef = this._modalService.show(CreateupdateEmergencyChargesComponent, { class: 'modal-lg' });
        createDialog.content.onSave.subscribe(() => {
            this.list();
        });
    }

    edit(_id: any): void {
        let editDialog: BsModalRef = this._modalService.show(CreateupdateEmergencyChargesComponent, { class: 'modal-lg', initialState: { id: _id } });
        editDialog.content.onSave.subscribe(() => {
            this.list();
        });
    }
}
