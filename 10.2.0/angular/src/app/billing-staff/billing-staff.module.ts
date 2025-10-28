import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { BillingStaffRoutingModule } from './billing-staff-routing.module';
import { BillingStaffComponent } from './billing-staff.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { ReportsComponent } from './reports/reports.component';
import { InsuranceClaimComponent } from './insurance-claim/insurance-claim.component';


@NgModule({
    imports: [
        SharedModule,
        BillingStaffRoutingModule,
        CommonModule,
        BillingStaffComponent,
        InvoicesComponent,
        ReportsComponent,
        InsuranceClaimComponent
       
      
    ],
})
export class BillingStaffModule {}
