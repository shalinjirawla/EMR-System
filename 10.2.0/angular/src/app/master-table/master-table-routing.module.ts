import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MasterTableComponent } from './master-table.component';

const routes: Routes = [
    {
        path: '',
        component: MasterTableComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class MasterTableRoutingModule {}
