import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { ProcedureRoutingModule } from './procedure-routing.module';
import { ProcedureComponent } from './procedure.component';
import {ProcedureReceiptComponent} from './procedure-receipt/procedure-receipt.component';
import {ProcedureRequestsComponent} from './procedure-requests/procedure-requests.component';

@NgModule({
    imports: [
        SharedModule,
        ProcedureRoutingModule,
        CommonModule,
        ProcedureComponent,
        ProcedureRequestsComponent,
        ProcedureReceiptComponent
       
      
    ],
})
export class ProcedureModule {}
