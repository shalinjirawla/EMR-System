import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule, DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { AvatarModule } from 'primeng/avatar';
import { DividerModule } from 'primeng/divider';
import { BirthRecordServiceProxy, BirthRecordDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import moment from 'moment';


@Component({
  selector: 'app-view-birth-record',
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule, TooltipModule, AvatarModule, DividerModule],
  templateUrl: './view-birth-record.component.html',
  styleUrls: ['./view-birth-record.component.css'],
  providers: [BirthRecordServiceProxy, DatePipe]
})
export class ViewBirthRecordComponent extends AppComponentBase implements OnInit {
  // set by initialState when opening modal
  birthRecordId: number;

  record: BirthRecordDto = new BirthRecordDto();

  loading = false;

  // maps for enums (if backend returns numeric enum)
  deliveryTypeMap = { 1: 'Normal', 2: 'Caesarean', 3: 'Forceps', 4: 'Vacuum', 5: 'Others' };
  genderMap = { 1: 'Male', 2: 'Female', 3: 'Other' };
  birthTypeMap = { 1: 'Single', 2: 'Twins', 3: 'Triplets', 4: 'Multiple' };

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _birthRecordService: BirthRecordServiceProxy,
    private datePipe: DatePipe,
    public cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (!this.birthRecordId) return;
    this.load();
  }

  load() {
    this.loading = true;
    this._birthRecordService.get(this.birthRecordId).subscribe({
      next: (res) => {
        this.record = res;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  formatDate(d: any): string {
    if (!d) return '-';
    return this.datePipe.transform(d, 'dd MMM yyyy') || '-';
  }

  formatTime(t: any): string {
    if (!t) return '-';

    // If it's a Moment object, convert to string first
    if (moment.isMoment(t)) {
      return (t as moment.Moment).format('hh:mm A');
    }

    if (typeof t === 'string') {
      const parts = t.split(':');
      if (parts.length >= 2) {
        const hh = parseInt(parts[0], 10);
        const mm = parseInt(parts[1].slice(0, 2), 10);
        const tmp = new Date();
        tmp.setHours(isNaN(hh) ? 0 : hh, isNaN(mm) ? 0 : mm);
        return this.datePipe.transform(tmp, 'hh:mm a') || t;
      }
    }

    return '-';
  }

  getDeliveryLabel(val: number | any) {
    return this.deliveryTypeMap[val] ?? val ?? '-';
  }

  getGenderLabel(val: number | any) {
    return this.genderMap[val] ?? val ?? '-';
  }

  getBirthTypeLabel(val: number | any) {
    return this.birthTypeMap[val] ?? val ?? '-';
  }

  close() {
    this.bsModalRef.hide();
  }

  print() {
    // simple print of modal content
    window.print();
  }
}
