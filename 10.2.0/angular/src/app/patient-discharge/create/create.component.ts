import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { BadgeModule } from 'primeng/badge';
import { AvatarModule } from 'primeng/avatar';
import { Component, Injector, ChangeDetectorRef, ViewChild, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table, TableModule } from 'primeng/table';
import { ActivatedRoute, Router } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { StepperModule } from 'primeng/stepper';
import { StepsModule } from 'primeng/steps';
@Component({
  selector: 'app-create',
  animations: [appModuleAnimation()],
  templateUrl: './create.component.html',
  styleUrl: './create.component.css',
  imports: [StepperModule, FormsModule, StepsModule, CommonModule, TableModule, AvatarModule, BadgeModule, TabsModule, PaginatorModule, CheckboxModule,
    BreadcrumbModule, TooltipModule, CardModule, TagModule, SelectModule, InputTextModule, MenuModule, ButtonModule],
})
export class CreateComponent implements OnInit {
  patientId: number;
  items: MenuItem[] | undefined;
  activeStep: number = 1;
  value: any;
  constructor(
    private _activatedRoute: ActivatedRoute,
    cd: ChangeDetectorRef, private router: Router,
  ) {
    this.patientId = Number(this._activatedRoute.snapshot.paramMap.get('id'));
  }
  ngOnInit(): void {
    this.items = [
      { label: 'Home', routerLink: '/' },
      { label: 'Create Discharge summary' },
    ];
  }
  gotList() {
    this.router.navigate(['app/doctors/patients'],);
  }
}