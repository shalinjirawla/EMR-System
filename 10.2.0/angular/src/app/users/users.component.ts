import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { UserServiceProxy, UserDto, UserDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './reset-password/reset-password.component';
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
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
interface FilterOption {
    label: string;
    value: any;
    checked: boolean;
}
@Component({
    templateUrl: './users.component.html',
    styleUrl: './users.component.css',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [TagModule, SelectModule, CheckboxModule, TooltipModule, AvatarModule, AvatarGroupModule, CardModule, FormsModule, TableModule,
        BreadcrumbModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe,
        OverlayPanelModule, MenuModule, ButtonModule, InputTextModule],
})
export class UsersComponent extends PagedListingComponentBase<UserDto> implements OnInit {
    items: MenuItem[] | undefined;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    users: UserDto[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;
    loading = true;
    activeFilter = {
        isActive: false,
        isNotActive: false,
        all: true
    };
    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }
    ngOnInit(): void {
        this.items = [
            { label: 'Home', routerLink: '/' },
            { label: 'Users' },
        ];
    }
    createUser(): void {
        this.showCreateOrEditUserDialog();
    }

    editUser(user: UserDto): void {
        this.showCreateOrEditUserDialog(user.id);
    }

    public resetPassword(user: UserDto): void {
        this.showResetPasswordUserDialog(user.id);
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
        this._userService
            .getAll(
                this.keyword,
                this.isActive,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: UserDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items;
                this.loading = false;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }
    delete(user: UserDto): void {
        abp.message.confirm(this.l('UserDeleteWarningMessage', user.fullName), undefined, (result: boolean) => {
            if (result) {
                this._userService.delete(user.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.refresh();
                });
            }
        });
    }
    onFilterChange(selected: 'active' | 'notActive' | 'all', event: any) {
        if (selected === 'all') {
            if (event.checked) {
                this.activeFilter.isActive = false;
                this.activeFilter.isNotActive = false;
                this.isActive = undefined; // "All"
            } else {
                // if uncheck All, force it back (must always have one selected)
                this.activeFilter.all = true;
                this.isActive = undefined;
            }
        }
        else if (selected === 'active') {
            if (event.checked) {
                this.activeFilter.isNotActive = false;
                this.activeFilter.all = false;
                this.isActive = true;
            } else {
                // if uncheck Active, fallback to All
                this.activeFilter.all = true;
                this.isActive = undefined;
            }
        }
        else if (selected === 'notActive') {
            if (event.checked) {
                this.activeFilter.isActive = false;
                this.activeFilter.all = false;
                this.isActive = false;
            } else {
                // if uncheck Not Active, fallback to All
                this.activeFilter.all = true;
                this.isActive = undefined;
            }
        }

        this.cd.detectChanges();
        this.list();
    }

    private showResetPasswordUserDialog(id?: number): void {
        this._modalService.show(ResetPasswordDialogComponent, {
            class: 'modal-lg',
            initialState: {
                id: id,
            },
        });
    }

    private showCreateOrEditUserDialog(id?: number): void {
        let createOrEditUserDialog: BsModalRef;
        if (!id) {
            createOrEditUserDialog = this._modalService.show(CreateUserDialogComponent, {
                class: 'modal-lg',
            });
        } else {
            createOrEditUserDialog = this._modalService.show(EditUserDialogComponent, {
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
}
