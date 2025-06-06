import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BillingStaffComponent } from './billing-staff.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { InsuranceComponent } from './insurance/insurance.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { PaymentsComponent } from './payments/payments.component';
import { ReportsComponent } from './reports/reports.component';







const routes: Routes = [
    {
        path: '',
        component: BillingStaffComponent,
          canActivate: [AppRouteGuard],
        children: [
            {
                path: 'insurance',
                component: InsuranceComponent,
            },
            {
                path: 'invoices',
                component: InvoicesComponent,
            },
            {
                path: 'payments',
                component: PaymentsComponent,
            },
            {
                path: 'reports',
                component: ReportsComponent,
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
export class BillingStaffRoutingModule {}
