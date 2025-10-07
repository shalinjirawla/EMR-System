import { Component, Injector, ChangeDetectorRef, ViewChild, OnInit } from '@angular/core';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PatientDischargeServiceProxy, PatientDischargeDtoPagedResultDto, PatientDischargeDto } from '@shared/service-proxies/service-proxies';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ButtonModule } from 'primeng/button';
import { MenuItem } from 'primeng/api';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { AvatarModule } from 'primeng/avatar';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrl: './list.component.css',
  animations: [appModuleAnimation()],
  standalone: true,
  imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, AvatarModule,
    BreadcrumbModule, CardModule, InputTextModule, LocalizePipe, ButtonModule, DatePipe],
  providers: [PatientDischargeServiceProxy]
})
export class ListComponent extends PagedListingComponentBase<PatientDischargeDto> implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  items: MenuItem[] | undefined;
  keyword = '';

  constructor(
    injector: Injector,
    private _patientDischargeService: PatientDischargeServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private http: HttpClient,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
    this.keyword = this._activatedRoute.snapshot.queryParams['keyword'] || '';
  }
  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Discharged Patient' },
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

    this._patientDischargeService
      .getAll(
        this.keyword,
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .pipe(
        finalize(() => {
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe((result: PatientDischargeDtoPagedResultDto) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
        this.cd.detectChanges();
      });
  }

  delete() { }
  getShortName(fullName: string): string {
    if (!fullName) return '';
    const words = fullName.trim().split(' ');
    const firstInitial = words[0].charAt(0).toUpperCase();
    const lastInitial = words.length > 1 ? words[words.length - 1].charAt(0).toUpperCase() : '';
    return firstInitial + lastInitial;
  }
  downloadPDF(pID: number) {
  const url = `https://localhost:44311/Download?patientId=${pID}`;

  this.http.get(url, { responseType: 'blob' }).subscribe({
    next: (res: Blob) => {
      // Create a link element to download
      const blob = new Blob([res], { type: 'application/pdf' });
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = `DischargeSummary_${pID}.pdf`;
      link.click();
      window.URL.revokeObjectURL(link.href); // Clean up
    },
    error: (err) => {
      console.error('PDF download failed', err);
      alert('Failed to download PDF');
    }
  });
}

}
