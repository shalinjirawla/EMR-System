import { ChangeDetectorRef, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule } from 'primeng/paginator';
import { RadioButtonModule } from 'primeng/radiobutton';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { InputTextModule } from 'primeng/inputtext';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';

import { 
  EmergencyProcedureDto, 
  EmergencyProcedureDtoPagedResultDto, 
  EmergencyProcedureServiceProxy, 
  ProcedureCategory
} from '@shared/service-proxies/service-proxies';

import { CreateupdateEmergencyProcedureComponent } from '../createupdate-emergency-procedure/createupdate-emergency-procedure.component';

@Component({
  selector: 'app-emergency-procedure',
  standalone: true,
  imports: [
    FormsModule,BreadcrumbModule,InputTextModule,CardModule, TooltipModule,RadioButtonModule,
    TableModule,OverlayPanelModule,ButtonModule,NgIf,PaginatorModule,LocalizePipe,CommonModule],
  providers: [EmergencyProcedureServiceProxy],
  animations: [appModuleAnimation()],
  templateUrl: './emergency-procedure.component.html',
  styleUrl: './emergency-procedure.component.css'
})
export class EmergencyProcedureComponent extends PagedListingComponentBase<EmergencyProcedureDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  procedures: EmergencyProcedureDto[] = [];
   keyword = '';
  category: ProcedureCategory | undefined;
  selectedRecord: EmergencyProcedureDto;
  items: MenuItem[];
  editDeleteMenus: MenuItem[];
   ProcedureCategory = ProcedureCategory;

   procedureCategories = [
    { label: 'All', value: undefined },
    { label: 'Minor', value: ProcedureCategory._0 },
    { label: 'Major', value: ProcedureCategory._1 },
    { label: 'Life Saving', value: ProcedureCategory._2 }
  ];

  constructor(
    injector: Injector,
    private _modalService: BsModalService,
    private _emergencyProcedureService: EmergencyProcedureServiceProxy,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Emergency Procedures' }
    ];

    this.editDeleteMenus = [
      { label: 'Edit', icon: 'pi pi-pencil', command: () => this.editProcedure(this.selectedRecord) },
      { label: 'Delete', icon: 'pi pi-trash', command: () => this.deleteProcedure(this.selectedRecord) }
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

    this._emergencyProcedureService
      .getAll(
         this.keyword,
        this.category,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result: EmergencyProcedureDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.cd.detectChanges();
      });
  }

  createProcedure(): void {
    let createDialog: BsModalRef = this._modalService.show(CreateupdateEmergencyProcedureComponent, { class: 'modal-lg' });
    createDialog.content.onSave.subscribe(() => {
      this.list();
    });
  }

  editProcedure(procedure: EmergencyProcedureDto): void {
    let editDialog: BsModalRef = this._modalService.show(CreateupdateEmergencyProcedureComponent, { 
      class: 'modal-lg', 
      initialState: { id: procedure.id } 
    });
    editDialog.content.onSave.subscribe(() => {
      this.list();
    });
  }

  deleteProcedure(procedure: EmergencyProcedureDto): void {
    abp.message.confirm('Are you sure you want to delete this procedure?', undefined, (result: boolean) => {
      if (result) {
        this._emergencyProcedureService.delete(procedure.id).subscribe(() => {
          abp.notify.success('Deleted successfully');
          this.list();
        });
      }
    });
  }

  delete(entity: EmergencyProcedureDto): void {
    this.deleteProcedure(entity);
  }

   onFilterChange(selected: string) {
    switch (selected) {
      case 'all': this.category = undefined; break;
      case 'minor': this.category = ProcedureCategory._0; break;
      case 'major': this.category = ProcedureCategory._1; break;
      case 'life-saving': this.category = ProcedureCategory._2; break;
    }
    this.list();
  }

}
