import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import {MasterTableRoutingModule} from '../master-table/master-table-routing.module';
import { RoomFacilitiesComponent } from './room-facilities/room-facilities.component';
import { RoomTypesComponent } from './room-types/room-types.component';
import { LabReportTypeComponent } from './lab-report-type/lab-report-type.component';
import { DoctorMasterComponent } from './doctor-master/doctor-master.component';
import { AppointmentTypesComponent } from './appointment-types/appointment-types.component';



@NgModule({
    imports: [
        SharedModule,
        CommonModule,
        MasterTableRoutingModule,
        RoomFacilitiesComponent,
        RoomTypesComponent,
        LabReportTypeComponent,
        DoctorMasterComponent,
        AppointmentTypesComponent
    ],
})
export class MasterTableModule {}
