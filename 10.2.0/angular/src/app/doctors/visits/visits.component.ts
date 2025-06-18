import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { NgIf } from '@node_modules/@angular/common';
import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';


@Component({
  selector: 'app-visits',
//   imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe],
  templateUrl: './visits.component.html',
  styleUrl: './visits.component.css'
})
export class VisitsComponent  {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    patients: any[] = [];
    keyword = '';
    isActive: boolean | null;
    advancedFiltersVisible = false;

    constructor(
        injector: Injector,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _userService: UserServiceProxy,
        // private _patientService: PatientServiceProxy,
        cd: ChangeDetectorRef
    ) {
        // super(injector, cd);
        // this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    clearFilters(): void {
        this.keyword = '';
        this.isActive = undefined;
    }

    // list(event?: LazyLoadEvent): void {
    //     if (this.primengTableHelper.shouldResetPaging(event)) {
    //         this.paginator.changePage(0);

    //         if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
    //             return;
    //         }
    //     }
    //     this.primengTableHelper.showLoadingIndicator();

    //     this._patientService
    //         .getAll(
    //             this.primengTableHelper.getSorting(this.dataTable),
    //             this.primengTableHelper.getSkipCount(this.paginator, event),
    //             this.primengTableHelper.getMaxResultCount(this.paginator, event)
    //         )
    //         .pipe(
    //             finalize(() => {
    //                 this.primengTableHelper.hideLoadingIndicator();
    //             })
    //         )
    //         .subscribe((result: PatientDtoPagedResultDto) => {
    //             this.primengTableHelper.records = result.items;
    //             this.primengTableHelper.totalRecordsCount = result.totalCount;
    //             this.primengTableHelper.hideLoadingIndicator();
    //             this.cd.detectChanges();
    //         });
    // }
    // delete(visits: any): void {
    //     abp.message.confirm(this.l('UserDeleteWarningMessage'), undefined, (result: boolean) => {
    //         if (result) {
    //             this._patientService.delete(visits.id).subscribe(() => {
    //                 abp.notify.success(this.l('SuccessfullyDeleted'));
    //                 this.refresh();
    //             });
    //         }
    //     });
    // }
}
