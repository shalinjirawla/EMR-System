import { Component, Injector, ChangeDetectorRef, ViewChild, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { RoleServiceProxy, RoleDto, RoleDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { CreateRoleDialogComponent } from './create-role/create-role-dialog.component';
import { EditRoleDialogComponent } from './edit-role/edit-role-dialog.component';
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
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
interface FilterOption {
    label: string;
    value: any;
    checked: boolean;
}
@Component({
    templateUrl: './roles.component.html',
    styleUrl: './roles.component.css',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, CheckboxModule,
        BreadcrumbModule, TooltipModule, CardModule, TagModule, SelectModule, InputTextModule,
        LocalizePipe, OverlayPanelModule, MenuModule, ButtonModule],
})
export class RolesComponent extends PagedListingComponentBase<RoleDto> implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    items: MenuItem[] | undefined;
    editDeleteMenus: MenuItem[] | undefined;
    roles: RoleDto[] = [];
    keyword = '';
    selectedRecord: RoleDto;

    constructor(
        injector: Injector,
        private _rolesService: RoleServiceProxy,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['keyword'] || '';
    }
    ngOnInit(): void {
        this.items = [
            { label: 'Home', routerLink: '/' },
            { label: 'Roles' },
        ];
        this.editDeleteMenus = [
            {
                label: 'Edit',
                icon: 'pi pi-pencil',
                command: () => this.editRole(this.selectedRecord)  // call edit
            },
            {
                label: 'Delete',
                icon: 'pi pi-trash',
                command: () => this.delete(this.selectedRecord)  // call edit
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

        this._rolesService
            .getAll(
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
            .subscribe((result: RoleDtoPagedResultDto) => {

                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(role: RoleDto): void {
        abp.message.confirm(this.l('RoleDeleteWarningMessage', role.displayName), undefined, (result: boolean) => {
            if (result) {
                this._rolesService
                    .delete(role.id)
                    .pipe(
                        finalize(() => {
                            abp.notify.success(this.l('SuccessfullyDeleted'));
                            this.refresh();
                        })
                    )
                    .subscribe(() => { });
            }
        });
    }

    createRole(): void {
        this.showCreateOrEditRoleDialog();
    }

    editRole(role: RoleDto): void {
        this.showCreateOrEditRoleDialog(role.id);
    }

    showCreateOrEditRoleDialog(id?: number): void {
        let createOrEditRoleDialog: BsModalRef;
        if (!id) {
            createOrEditRoleDialog = this._modalService.show(CreateRoleDialogComponent, {
                class: 'modal-lg',
            });
        } else {
            createOrEditRoleDialog = this._modalService.show(EditRoleDialogComponent, {
                class: 'modal-lg',
                initialState: {
                    id: id,
                },
            });
        }

        createOrEditRoleDialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
}
