import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { Table, TableModule } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { finalize } from 'rxjs/operators';
import { CreateUpdateTriageComponent } from '../create-update-triage/create-update-triage.component';
import { TriageDto, TriageDtoPagedResultDto, TriageServiceProxy } from '@shared/service-proxies/service-proxies';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { FormsModule } from '@node_modules/@angular/forms';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CommonModule } from '@node_modules/@angular/common';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonModule } from 'primeng/button';


@Component({
  selector: 'app-emergency-triage',
  imports: [Paginator, LocalizePipe,FormsModule, TableModule,CommonModule,OverlayPanelModule,ButtonModule],
  providers:[TriageServiceProxy],
  animations: [appModuleAnimation()],
  templateUrl: './emergency-triage.component.html',
  styleUrl: './emergency-triage.component.css'
})
export class EmergencyTriageComponent extends PagedListingComponentBase<TriageDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  keyword = '';
  selectedRecord: TriageDto;

  constructor(
    injector: Injector,
    private _triageService: TriageServiceProxy,
    private _modalService: BsModalService,
     cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  ngOnInit(): void {}

  list(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();
    this._triageService.getAll(
      this.keyword,
      this.primengTableHelper.getSorting(this.dataTable),
      this.primengTableHelper.getSkipCount(this.paginator, event),
      this.primengTableHelper.getMaxResultCount(this.paginator, event)
    ).pipe(
      finalize(() => this.primengTableHelper.hideLoadingIndicator())
    ).subscribe((result: TriageDtoPagedResultDto) => {
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.cd.detectChanges();
    });
  }

  createTriage(): void {
    this.showCreateOrEditModal();
  }

  editTriage(record: TriageDto): void {
    this.showCreateOrEditModal(record.id);
  }

  showCreateOrEditModal(id?: number): void {
    let modal: BsModalRef;
    if (!id) {
      modal = this._modalService.show(CreateUpdateTriageComponent, { class: 'modal-lg' });
    } else {
      modal = this._modalService.show(CreateUpdateTriageComponent, { class: 'modal-lg', initialState: { id } });
    }
    modal.content.onSave.subscribe(() => this.refresh());
  }

  delete(record: TriageDto): void {
    abp.message.confirm('Are you sure?', undefined, (result: boolean) => {
      if (result) {
        this._triageService.delete(record.id).subscribe(() => {
          abp.notify.success('Deleted successfully');
          this.refresh();
        });
      }
    });
  }
}