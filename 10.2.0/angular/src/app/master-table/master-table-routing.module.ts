import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MasterTableComponent } from './master-table.component';
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
import { BedsComponent } from './beds/beds.component';
import { EmergencyChargesComponent } from './emergency-charges/emergency-charges.component';
import {DepartmentComponent} from './department/department.component'
import {EmergencyProcedureComponent} from './emergency-procedure/emergency-procedure.component'

const routes: Routes = [
    {
        path: '',
        component: MasterTableComponent,
        pathMatch: 'full',
    },
    {
        path: 'room-facilities',
        component: RoomFacilitiesComponent,
    },
    {
        path: 'room-types',
        component: RoomTypesComponent,
    },
    {
        path: 'lab-report-type',
        component: LabReportTypeComponent,
    },
    {
        path: 'doctor-master',
        component: DoctorMasterComponent,
    },
    {
        path: 'appointment-types',
        component: AppointmentTypesComponent,
    },
    {
        path: 'measure-units',
        component: MeasureUnitsComponent,
    },
    {
        path: 'lab-test',
        component: LabTestComponent,
    },
    {
        path: 'lab-test-items',
        component: LabTestItemsComponent,
    },
    {
        path: 'test-result-limit',
        component: TestResultLimitComponent,
    },
    {
        path:'helth-package',
        component:HealthPackageComponent
    },
    {
        path:'beds',
        component:BedsComponent
    },
    {
        path:'emergency-charges',
        component:EmergencyChargesComponent
    },
    {
        path:'department',
        component:DepartmentComponent
    },
    {
        path:'emergency-procedure',
        component:EmergencyProcedureComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class MasterTableRoutingModule {}
