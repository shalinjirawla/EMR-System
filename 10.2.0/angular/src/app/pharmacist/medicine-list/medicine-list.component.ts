import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import {
  MedicineMasterDto,
  MedicineMasterDtoPagedResultDto,
  MedicineMasterServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { LazyLoadEvent } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { CreateMedicineListComponent } from '../create-medicine-list/create-medicine-list.component';
import { EditMedicineComponent } from '../edit-medicine/edit-medicine.component';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { FormsModule } from '@angular/forms';
import { NgIf, DatePipe } from '@angular/common';
import { appModuleAnimation } from '../../../shared/animations/routerTransition';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import moment from 'moment';
import {EditMedicineListComponent} from '../edit-medicine-list/edit-medicine-list.component'

@Component({
  selector: 'app-medicine-inventory',
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [
    LocalizePipe,
    FormsModule,
    TableModule,
    CalendarModule,
    NgIf,
    PaginatorModule,
    ButtonModule,
    OverlayPanelModule,
    MenuModule,
  ],
  providers: [MedicineMasterServiceProxy],
  templateUrl: './medicine-list.component.html',
  styleUrl: './medicine-list.component.css',
})
export class MedicineListComponent
  extends PagedListingComponentBase<MedicineMasterDto>
  implements OnInit {

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  minStock: number | undefined;
  isAvailable: boolean | null;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _activatedRoute: ActivatedRoute,
    private _medicineService: MedicineMasterServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['filterText'] || '';
  }

  ngOnInit(): void {}

  clearFilters(): void {
    this.keyword = '';
    this.minStock = undefined;
    this.isAvailable = undefined;
    this.list();
  }

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();

    this._medicineService
      .getAll(
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: MedicineMasterDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  protected delete(entity: MedicineMasterDto): void {
    abp.message.confirm(
      "Are you sure you want to delete this?",
      undefined,
      (result: boolean) => {
        if (result) {
          this._medicineService.delete(entity.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  addNewMedicine(): void {
    this.showCreateOrEditMedicine();
  }

  editNewMedicine(dto: MedicineMasterDto): void {
    this.showCreateOrEditMedicine(dto.id);
  }

  showCreateOrEditMedicine(id?: number): void {
    let dialog: BsModalRef;
    if (!id) {
      dialog = this._modalService.show(CreateMedicineListComponent, { class: 'modal-lg' });
    } 
    else {
      dialog = this._modalService.show(EditMedicineListComponent, {
        class: 'modal-lg',
        initialState: { medicineId: id }
      });
    }

    dialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
}