import { NgModule } from '@angular/core';
import { DepositRoutingModule } from './deposit-routing.module';
import { DepositComponent } from './deposit.component';
import { SharedModule } from 'primeng/api';

@NgModule({
  imports: [SharedModule,
    DepositRoutingModule,
    DepositComponent,
  ],
})
export class DepositModule {}
