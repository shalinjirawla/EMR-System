import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@node_modules/@angular/common';
import { ButtonModule } from 'primeng/button';
import { AppComponentBase } from '@shared/app-component-base';
import { DeathRecordServiceProxy, DeathRecordDto } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
@Component({
  selector: 'app-view-death-record',
  imports: [CommonModule,ButtonModule],
  providers:[DeathRecordServiceProxy],
  templateUrl: './view-death-record.component.html',
  styleUrl: './view-death-record.component.css'
})
export class ViewDeathRecordComponent extends AppComponentBase implements OnInit {
  deathRecordId: number;
  record: DeathRecordDto = new DeathRecordDto();

  constructor(
    injector: Injector,
    private _deathRecordService: DeathRecordServiceProxy,
    public bsModalRef: BsModalRef,
    public cd:ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.deathRecordId) {
      this.loadDeathRecord();
    }
  }

  loadDeathRecord(): void {
    this._deathRecordService.get(this.deathRecordId).subscribe((res) => {
      this.record = res;
      this.cd.detectChanges();
    });
  }

  close(): void {
    this.bsModalRef.hide();
  }
}
