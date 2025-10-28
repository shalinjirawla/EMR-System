import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { BirthDeathRecordRoutingModule } from './birth-death-record-routing.module';
import { BirthDeathRecordComponent } from './birth-death-record.component';
import { BirthRecordComponent } from './birth-record/birth-record.component';
import { DeathRecordComponent } from './death-record/death-record.component';

@NgModule({
  imports: [SharedModule,
    BirthDeathRecordRoutingModule,
    BirthDeathRecordComponent,
    BirthRecordComponent,
    DeathRecordComponent
  ],
})
export class BirthDeathRecordModule {}
