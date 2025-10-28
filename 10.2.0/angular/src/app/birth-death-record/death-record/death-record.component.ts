import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import {
  DeathRecordDto,
  DeathRecordDtoPagedResultDto,
  DeathRecordServiceProxy,
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
import { CreateDeathRecordComponent } from '../create-death-record/create-death-record.component';
import { EditDeathRecordComponent } from '../edit-death-record/edit-death-record.component';
import { ViewDeathRecordComponent } from '../view-death-record/view-death-record.component';

@Component({
  selector: 'app-death-record',
  animations: [appModuleAnimation()],
  imports: [LocalizePipe,FormsModule,NgIf,DatePipe,CommonModule,BreadcrumbModule,TableModule,
    InputTextModule,PaginatorModule,TooltipModule,MenuModule,CardModule,ButtonModule,],
  providers: [DeathRecordServiceProxy],
  templateUrl: './death-record.component.html',
  styleUrl: './death-record.component.css',
})
export class DeathRecordComponent
  extends PagedListingComponentBase<DeathRecordDto>
  implements OnInit
{
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  keyword = '';
  items: MenuItem[] = [];
  editDeleteMenus: MenuItem[] = [];
  selectedRecord: DeathRecordDto;

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _deathRecordService: DeathRecordServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Death Records' },
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

    this._deathRecordService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: DeathRecordDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  addNewDeathRecord(): void {
    const dialog: BsModalRef = this._modalService.show(CreateDeathRecordComponent, { class: 'modal-lg' });
    dialog.content.onSave.subscribe(() => this.refresh());
  }

  editRecord(dto: DeathRecordDto): void {
    const dialog: BsModalRef = this._modalService.show(EditDeathRecordComponent, {
      class: 'modal-lg',
      initialState: { deathRecordId: dto.id },
    });
    dialog.content.onSave.subscribe(() => this.refresh());
  }

  viewRecord(dto: DeathRecordDto): void {
    this._modalService.show(ViewDeathRecordComponent, {
      class: 'modal-lg',
      initialState: {
        deathRecordId: dto.id
      },
    });
  }

  protected delete(entity: DeathRecordDto): void {
    abp.message.confirm('Are you sure you want to delete this record?', undefined, (result: boolean) => {
      if (result) {
        this._deathRecordService.delete(entity.id).subscribe(() => {
          abp.notify.success(this.l('SuccessfullyDeleted'));
          this.refresh();
        });
      }
    });
  }
}
