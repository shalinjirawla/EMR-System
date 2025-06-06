import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LabTechnicianComponent } from './lab-technician.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { InventoryComponent } from './inventory/inventory.component';
import { ReportsComponent } from './reports/reports.component';
import { TestRequestsComponent } from './test-requests/test-requests.component';


const routes: Routes = [
    {
        path: '',
        component: LabTechnicianComponent,
          canActivate: [AppRouteGuard],
        children: [
            {
                path: 'inventory',
                component: InventoryComponent,
            },
            {
                path: 'reports',
                component: ReportsComponent,
            },
            {
                path: 'test-requests',
                component: TestRequestsComponent,
            },
            {
                path: '',
                redirectTo: 'test-requests',
                pathMatch: 'full'
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class LabTechnicianRoutingModule {}
