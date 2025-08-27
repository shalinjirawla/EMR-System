import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import {MasterTableRoutingModule} from '../master-table/master-table-routing.module';
import { RoomFacilitiesComponent } from './room-facilities/room-facilities.component';
import { RoomTypesComponent } from './room-types/room-types.component';
import { LabReportTypeComponent } from './lab-report-type/lab-report-type.component';
import { DoctorMasterComponent } from './doctor-master/doctor-master.component';
import { AppointmentTypesComponent } from './appointment-types/appointment-types.component';
import { MeasureUnitsComponent } from './measure-unit/measure-unit.component';
import { LabTestComponent } from './lab-test/lab-test.component';
import { LabTestItemsComponent } from './lab-test-items/lab-test-items.component';
import { TestResultLimitComponent } from './test-result-limit/test-result-limit.component';
import { HealthPackageComponent } from './health-package/health-package.component';
import {BedsComponent} from './beds/beds.component'
import { DepartmentComponent } from './department/department.component';


@NgModule({
    imports: [
        SharedModule,
        CommonModule,
        MasterTableRoutingModule,
        RoomFacilitiesComponent,
        RoomTypesComponent,
        LabReportTypeComponent,
        DoctorMasterComponent,
        AppointmentTypesComponent,
        MeasureUnitsComponent,
        LabTestComponent,
        LabTestItemsComponent,
        TestResultLimitComponent,
        HealthPackageComponent,
        BedsComponent,
        DepartmentComponent
    ],
})
export class MasterTableModule {}
