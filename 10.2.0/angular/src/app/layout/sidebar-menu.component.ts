import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { Router, RouterEvent, NavigationEnd, PRIMARY_OUTLET, RouterLink } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MenuItem } from '@shared/layout/menu-item';
import { NgTemplateOutlet } from '@angular/common';
import { CollapseDirective } from 'ngx-bootstrap/collapse';

@Component({
    selector: 'sidebar-menu',
    templateUrl: './sidebar-menu.component.html',
    standalone: true,
    imports: [NgTemplateOutlet, RouterLink, CollapseDirective],
})
export class SidebarMenuComponent extends AppComponentBase implements OnInit {
    menuItems: MenuItem[];
    menuItemsMap: { [key: number]: MenuItem } = {};
    activatedMenuItems: MenuItem[] = [];
    routerEvents: BehaviorSubject<RouterEvent> = new BehaviorSubject(undefined);
    homeRoute = '/app/about';

    constructor(
        injector: Injector,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.menuItems = this.getMenuItems();
        this.patchMenuItems(this.menuItems);

        this.router.events.subscribe((event: NavigationEnd) => {
            const currentUrl = event.url !== '/' ? event.url : this.homeRoute;
            const primaryUrlSegmentGroup = this.router.parseUrl(currentUrl).root.children[PRIMARY_OUTLET];
            if (primaryUrlSegmentGroup) {
                this.activateMenuItems('/' + primaryUrlSegmentGroup.toString());
            }
        });
    }

//     getMenuItems(): MenuItem[] {
//         return [
//             // new MenuItem(this.l('About'), '/app/about', 'fas fa-info-circle'),
//             new MenuItem(this.l('HomePage'), '/app/home', 'fas fa-home'),
//             new MenuItem(this.l('Roles'), '/app/roles', 'fas fa-theater-masks', 'Pages.Roles'),
//             new MenuItem(this.l('Tenants'), '/app/tenants', 'fas fa-building', 'Pages.Tenants'),
//             new MenuItem(this.l('Users'), '/app/users', 'fas fa-users', 'Pages.Users'),
//             // new MenuItem(this.l('Doctors'), '/app/doctors', 'fa fa-user-md', 'Pages.Doctors'),
//         //    new MenuItem('Doctor', '', 'fa fa-user-md', 'Pages.Doctors', [
//             new MenuItem('Appointments', '/app/doctors/appointments', 'fa fa-calendar','Pages.Doctors'),
//             new MenuItem('Patients', '/app/doctors/patients', 'fa fa-user-circle','Pages.Doctors'),
//             new MenuItem('Prescriptions', '/app/doctors/prescriptions', 'fa-solid fa-prescription','Pages.Doctors'),
//             new MenuItem('Visits', '/app/doctors/visits', 'fa fa-bed','Pages.Doctors'),
//             new MenuItem('Lab Orders', '/app/doctors/lab-order', 'fa fa-flask','Pages.Doctors'),

// // ])

//              new MenuItem('Insurance', '/app/billing-staff/insurance', 'fa fa-bed','Pages.BillingStaff'),
//              new MenuItem('Invoices', '/app/billing-staff/invoices', 'fa fa-bed','Pages.BillingStaff'),
//              new MenuItem('Payments', '/app/billing-staff/payments', 'fa fa-bed','Pages.BillingStaff'),
//              new MenuItem('Reports', '/app/billing-staff/reports', 'fa fa-bed','Pages.BillingStaff'),

//              new MenuItem('Inventory', '/app/lab-technician/inventory', 'fa fa-bed','Pages.LabTechnician'),
//              new MenuItem('Reports', '/app/lab-technician/reports', 'fa fa-bed','Pages.LabTechnician'),
//              new MenuItem('Test-Request', '/app/lab-technician/test-requests', 'fa fa-bed','Pages.LabTechnician'),

//              new MenuItem('Patients', '/app/nurse/assigned-patients', 'fa fa-bed','Pages.Nurse'),
//              new MenuItem('Medication', '/app/nurse/medication-log', 'fa fa-bed','Pages.Nurse'),
//              new MenuItem('Vitals Notes', '/app/nurse/vitals-notes', 'fa fa-bed','Pages.Nurse'),

//              new MenuItem('My Appointments', '/app/patient/patients-appointments', 'fa fa-bed','Pages.Patient'),
//              new MenuItem('Lab Reports', '/app/patient/patients-reports', 'fa fa-bed','Pages.Patient'),
//              new MenuItem('Payments', '/app/patient/patients-payments', 'fa fa-bed','Pages.Patient'),
//              new MenuItem('Prescriptions', '/app/patient/patients-prescriptions', 'fa fa-bed','Pages.Patient'),

//              new MenuItem('Inventory', '/app/pharmacist/pharmacist-prescriptions', 'fa fa-bed','Pages.Pharmacist'),
//              new MenuItem('Prescriptions', '/app/pharmacist/pharmacist-inventory', 'fa fa-bed','Pages.Pharmacist'),

//             // new MenuItem(this.l('MultiLevelMenu'), '', 'fas fa-circle', '', [
//             //     new MenuItem('ASP.NET Boilerplate', '', 'fas fa-dot-circle', '', [
//             //         new MenuItem('Home', 'https://aspnetboilerplate.com?ref=abptmpl', 'far fa-circle'),
//             //         new MenuItem('Templates', 'https://aspnetboilerplate.com/Templates?ref=abptmpl', 'far fa-circle'),
//             //         new MenuItem('Samples', 'https://aspnetboilerplate.com/Samples?ref=abptmpl', 'far fa-circle'),
//             //         new MenuItem(
//             //             'Documents',
//             //             'https://aspnetboilerplate.com/Pages/Documents?ref=abptmpl',
//             //             'far fa-circle'
//             //         ),
//             //     ]),
//             //     new MenuItem('ASP.NET Zero', '', 'fas fa-dot-circle', '', [
//             //         new MenuItem('Home', 'https://aspnetzero.com?ref=abptmpl', 'far fa-circle'),
//             //         new MenuItem('Features', 'https://aspnetzero.com/Features?ref=abptmpl', 'far fa-circle'),
//             //         new MenuItem('Pricing', 'https://aspnetzero.com/Pricing?ref=abptmpl#pricing', 'far fa-circle'),
//             //         new MenuItem('Faq', 'https://aspnetzero.com/Faq?ref=abptmpl', 'far fa-circle'),
//             //         new MenuItem('Documents', 'https://aspnetzero.com/Documents?ref=abptmpl', 'far fa-circle'),
//             //     ]),
//             // ]),
//         ];
//     }

        getMenuItems(): MenuItem[] {
    return [
        new MenuItem(this.l('HomePage'), '/app/home', 'fas fa-home'),
        new MenuItem(this.l('Roles'), '/app/roles', 'fas fa-user-shield', 'Pages.Roles'),
        new MenuItem(this.l('Tenants'), '/app/tenants', 'fas fa-building', 'Pages.Tenants'),
        new MenuItem(this.l('Users'), '/app/users', 'fas fa-users', 'Pages.Users'),

        // Doctor Section
        new MenuItem('Appointments', '/app/doctors/appointments', 'fas fa-calendar-check', 'Pages.Doctors'),
        new MenuItem('Patients', '/app/doctors/patients', 'fas fa-user-injured', 'Pages.Doctors'),
        new MenuItem('Prescriptions', '/app/doctors/prescriptions', 'fas fa-file-prescription', 'Pages.Doctors'),
        new MenuItem('Visits', '/app/doctors/visits', 'fas fa-procedures', 'Pages.Doctors'),
        new MenuItem('Lab Orders', '/app/doctors/lab-order', 'fas fa-vials', 'Pages.Doctors'),

        // Billing Staff
        new MenuItem('Insurance', '/app/billing-staff/insurance', 'fas fa-file-invoice-dollar', 'Pages.BillingStaff'),
        new MenuItem('Invoices', '/app/billing-staff/invoices', 'fas fa-file-invoice', 'Pages.BillingStaff'),
        new MenuItem('Payments', '/app/billing-staff/payments', 'fas fa-credit-card', 'Pages.BillingStaff'),
        new MenuItem('Reports', '/app/billing-staff/reports', 'fas fa-chart-line', 'Pages.BillingStaff'),

        // Lab Technician
        new MenuItem('Inventory', '/app/lab-technician/inventory', 'fas fa-boxes', 'Pages.LabTechnician'),
        new MenuItem('Reports', '/app/lab-technician/reports', 'fas fa-file-medical-alt', 'Pages.LabTechnician'),
        new MenuItem('Test-Request', '/app/lab-technician/test-requests', 'fas fa-microscope', 'Pages.LabTechnician'),

        // Nurse
        new MenuItem('Patients', '/app/nurse/assigned-patients', 'fas fa-procedures', 'Pages.Nurse'),
        new MenuItem('Medication', '/app/nurse/medication-log', 'fas fa-pills', 'Pages.Nurse'),
        new MenuItem('Vitals Notes', '/app/nurse/vitals-notes', 'fas fa-notes-medical', 'Pages.Nurse'),

        // Patient
        new MenuItem('My Appointments', '/app/patient/patients-appointments', 'fas fa-calendar-alt', 'Pages.Patient'),
        new MenuItem('Lab Reports', '/app/patient/patients-reports', 'fas fa-vial', 'Pages.Patient'),
        new MenuItem('Payments', '/app/patient/patients-payments', 'fas fa-money-check-alt', 'Pages.Patient'),
        new MenuItem('Prescriptions', '/app/patient/patients-prescriptions', 'fas fa-prescription-bottle-alt', 'Pages.Patient'),

        // Pharmacist
        new MenuItem('Inventory', '/app/pharmacist/pharmacist-prescriptions', 'fas fa-capsules', 'Pages.Pharmacist'),
        new MenuItem('Prescriptions', '/app/pharmacist/pharmacist-inventory', 'fas fa-prescription-bottle', 'Pages.Pharmacist'),
    ];
}


    patchMenuItems(items: MenuItem[], parentId?: number): void {
        items.forEach((item: MenuItem, index: number) => {
            
            item.id = parentId ? Number(parentId + '' + (index + 1)) : index + 1;
            if (parentId) {
                item.parentId = parentId;
            }
            if (parentId || item.children) {
                this.menuItemsMap[item.id] = item;
            }
            if (item.children) {
                this.patchMenuItems(item.children, item.id);
            }
        });
    }

    activateMenuItems(url: string): void {
        this.deactivateMenuItems(this.menuItems);
        this.activatedMenuItems = [];
        const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
        foundedItems.forEach((item) => {
            this.activateMenuItem(item);
        });
    }

    deactivateMenuItems(items: MenuItem[]): void {
        items.forEach((item: MenuItem) => {
            item.isActive = false;
            item.isCollapsed = true;
            if (item.children) {
                this.deactivateMenuItems(item.children);
            }
        });
    }

    findMenuItemsByUrl(url: string, items: MenuItem[], foundedItems: MenuItem[] = []): MenuItem[] {
        items.forEach((item: MenuItem) => {
            if (item.route === url) {
                foundedItems.push(item);
            } else if (item.children) {
                this.findMenuItemsByUrl(url, item.children, foundedItems);
            }
        });
        return foundedItems;
    }

    activateMenuItem(item: MenuItem): void {
        item.isActive = true;
        if (item.children) {
            item.isCollapsed = false;
        }
        this.activatedMenuItems.push(item);
        if (item.parentId) {
            this.activateMenuItem(this.menuItemsMap[item.parentId]);
        }
    }

    isMenuItemVisible(item: MenuItem): boolean {
        if (!item.permissionName) {
            return true;
        }
        return this.permission.isGranted(item.permissionName);
    }
}
