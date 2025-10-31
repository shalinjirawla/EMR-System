import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import {
  BirthRecordDto, BirthRecordDtoPagedResultDto,
  BirthRecordServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LazyLoadEvent, MenuItem } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FormsModule } from '@angular/forms';
import { NgIf, DatePipe, CommonModule } from '@angular/common';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { TooltipModule } from 'primeng/tooltip';
import { MenuModule } from 'primeng/menu';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateBirthRecordComponent } from '../create-birth-record/create-birth-record.component';
import { ViewBirthRecordComponent } from '../view-birth-record/view-birth-record.component';
import { EditBirthRecordComponent } from '../edit-birth-record/edit-birth-record.component';
@Component({
  selector: 'app-birth-record',
  animations: [appModuleAnimation()],
  imports: [LocalizePipe, FormsModule, NgIf, DatePipe, CommonModule, BreadcrumbModule, TableModule,
    InputTextModule, PaginatorModule, TooltipModule, MenuModule, CardModule, ButtonModule],
  providers: [BirthRecordServiceProxy],
  templateUrl: './birth-record.component.html',
  styleUrl: './birth-record.component.css'
})
export class BirthRecordComponent
  extends PagedListingComponentBase<BirthRecordDto>
  implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  items: MenuItem[] = [];
  editDeleteMenus: MenuItem[] = [];
  selectedRecord: BirthRecordDto;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _birthRecordService: BirthRecordServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Birth Records' },
    ];

    this.editDeleteMenus = [
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => this.editRecord(this.selectedRecord),
      },
      {
        label: 'View',
        icon: 'pi pi-eye',
        command: () => this.viewRecord(this.selectedRecord),
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.delete(this.selectedRecord),
      },
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

    this._birthRecordService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: BirthRecordDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  addNewBirthRecord(): void {
    const dialog: BsModalRef = this._modalService.show(CreateBirthRecordComponent, { class: 'modal-lg' });
    dialog.content.onSave.subscribe(() => this.refresh());
  }

  editRecord(dto: BirthRecordDto): void {
    const dialog: BsModalRef = this._modalService.show(EditBirthRecordComponent, {
      class: 'modal-lg',
      initialState: { birthRecordId: dto.id },
    });
    dialog.content.onSave.subscribe(() => this.refresh());
  }

  viewRecord(dto: BirthRecordDto): void {
    this._modalService.show(ViewBirthRecordComponent, {
      class: 'modal-lg',
      initialState: { birthRecordId: dto.id },
    });
  }
  getDeliveryTypeName(type: number): string {
    switch (type) {
      case 1: return 'Normal';
      case 2: return 'Caesarean';
      default: return '-';
    }
  }

  getGenderName(type: number): string {
    switch (type) {
      case 1: return 'Male';
      case 2: return 'Female';
      case 3: return 'Other';
      default: return '-';
    }
  }
  getBirthTypeName(type: number): string {
    switch (type) {
      case 1: return 'Single';
      case 2: return 'Twins';
      case 3: return 'Triplets';
      case 4: return 'Multiple';
      default: return '-';
    }
  }
  protected delete(entity: BirthRecordDto): void {
    abp.message.confirm('Are you sure you want to delete this record?', entity.motherName, (result: boolean) => {
      if (result) {
        this._birthRecordService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
}
