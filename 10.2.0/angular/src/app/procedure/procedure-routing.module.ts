import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { ProcedureComponent } from './procedure.component';
import { ProcedureRequestsComponent } from './procedure-requests/procedure-requests.component';
import { ProcedureReceiptComponent } from './procedure-receipt/procedure-receipt.component';

const routes: Routes = [
    {
        path: '',
        component: ProcedureComponent,
          canActivate: [AppRouteGuard],
        children: [
            {
                path: 'procedure-requests',
                component: ProcedureRequestsComponent,
            },
            {
                path: 'procedure-receipts',
                component: ProcedureReceiptComponent,
            },
            {
                path: '',
                redirectTo: 'procedure-requests',
                pathMatch: 'full'
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ProcedureRoutingModule {}
