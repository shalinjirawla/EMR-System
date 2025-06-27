import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import {MasterTableRoutingModule} from '../master-table/master-table-routing.module';


@NgModule({
    imports: [
        SharedModule,
        CommonModule,
        MasterTableRoutingModule
    ],
})
export class MasterTableModule {}
