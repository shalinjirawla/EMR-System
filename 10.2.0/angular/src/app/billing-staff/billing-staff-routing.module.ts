import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BillingStaffComponent } from './billing-staff.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { InvoicesComponent } from './invoices/invoices.component';
import { ReportsComponent } from './reports/reports.component';
import { InsuranceClaimComponent } from "./insurance-claim/insurance-claim.component";

const routes: Routes = [
    {
        path: '',
        component: BillingStaffComponent,
        canActivate: [AppRouteGuard],
        children: [
            {
                path: 'invoices',
                component: InvoicesComponent,
            },
            {
                path: 'reports',
                component: ReportsComponent,
            },
            {
                path: 'insurance-claim',
                component: InsuranceClaimComponent
            },
            {
                path: '',
                redirectTo: 'invoices',
                pathMatch: 'full'
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class BillingStaffRoutingModule { }
