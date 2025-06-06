import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { BillingStaffRoutingModule } from './billing-staff-routing.module';
import { BillingStaffComponent } from './billing-staff.component';
import { InsuranceComponent } from './insurance/insurance.component';
import { InvoicesComponent } from './invoices/invoices.component';
import { PaymentsComponent } from './payments/payments.component';
import { ReportsComponent } from './reports/reports.component';


@NgModule({
    imports: [
        SharedModule,
        BillingStaffRoutingModule,
        CommonModule,
        BillingStaffComponent,
        InsuranceComponent,
        InvoicesComponent,
        PaymentsComponent,
        ReportsComponent
       
      
    ],
})
export class BillingStaffModule {}
