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

    // getMenuItems(): MenuItem[] {
    //     return [
    //         new MenuItem(this.l('HomePage'), '/app/home', 'fas fa-home'),

    //         // Admin Section
    //         new MenuItem(this.l('Users'), '/app/users', 'fas fa-users-cog', 'Pages.Users'),
    //     new MenuItem(this.l('Roles'), '/app/roles', 'fas fa-user-tag', 'Pages.Roles'),
    //     new MenuItem(this.l('Tenants'), '/app/tenants', 'fas fa-network-wired', 'Pages.Tenants'),
    //         //new MenuItem(this.l('Rooms'), '/app/room', 'fas fa-hotel', 'Pages.Users'),
    //         new MenuItem(this.l('Admit Patient'), '/app/admission', 'fas fa-procedures', 'Pages.Users'),


    //         // Doctor Section
    //         new MenuItem(this.l('Doctor'), '', 'fas fa-user-md', 'Pages.Doctors', [
    //             new MenuItem(this.l('View Appointments'), '/app/doctors/view-appointments', 'fas fa-calendar-check', 'Pages.Doctors.Appointments.View'),
    //             new MenuItem(this.l('Assigned Patients'), '/app/doctors/patients', 'fas fa-user-injured', 'Pages.Doctors.Patients.View'),
    //             new MenuItem(this.l('Prescriptions'), '/app/doctors/prescriptions', 'fas fa-file-prescription', 'Pages.Doctors.Prescriptions.Create'),
    //             new MenuItem(this.l('Visits'), '/app/doctors/visits', 'fas fa-notes-medical', 'Pages.Doctors.Visits.View'),
    //             new MenuItem(this.l('Lab Orders'), '/app/doctors/lab-order', 'fas fa-vials', 'Pages.Doctors.LabOrders.Create'),
    //         ]),

    //         // Nurse Section
    //         new MenuItem(this.l('Nurse'), '', 'fas fa-user-nurse', 'Pages.Nurse', [
    //             new MenuItem(this.l('Appointments'), '/app/nurse/appointments', 'fas fa-calendar-plus', 'Pages.Doctors.Appointments.Schedule'),
    //             new MenuItem(this.l('Assigned Patients'), '/app/nurse/assigned-patients', 'fas fa-procedures', 'Pages.Nurse.Patients.View'),
    //         new MenuItem(this.l('Vitals'), '/app/nurse/vitals-notes', 'fas fa-heartbeat', 'Pages.Nurse.Vitals.View'),
    //         // new MenuItem(this.l('Add Vitals'), '/app/nurse/vitals-notes/add', 'fas fa-plus-circle', 'Pages.Nurse.Vitals.Create'),
    //         new MenuItem(this.l('Order Medication'), '/app/nurse/order-medicine', 'fas fa-pills', 'Pages.Nurse.Medication.View'),
    //     ]),


    //     // Billing Staff Section
    //     new MenuItem(this.l('Billing Staff'), '', 'fas fa-file-invoice-dollar', 'Pages.BillingStaff', [
    //         new MenuItem(this.l('Invoices'), '/app/billing-staff/invoices', 'fas fa-receipt', 'Pages.BillingStaff.Invoices.View'),
    //         // new MenuItem(this.l('Payments'), '/app/billing-staff/payments', 'fas fa-credit-card', 'Pages.BillingStaff.Payments.View'),
    //         // new MenuItem(this.l('Insurance'), '/app/billing-staff/insurance', 'fas fa-file-invoice-dollar', 'Pages.BillingStaff.Insurance.View'),
    //         // new MenuItem(this.l('Reports'), '/app/billing-staff/reports', 'fas fa-chart-line', 'Pages.BillingStaff.Reports.View'),
    //         new MenuItem(this.l('Deposit'), '/app/deposit', 'fas fa-hand-holding-usd', 'Pages.BillingStaff'),

    //     ]),

    //     // Lab Technician Section
    //     new MenuItem(this.l('Lab Technician'), '', 'fas fa-flask', 'Pages.LabTechnician', [
    //         new MenuItem(this.l('Test Requests'), '/app/lab-technician/test-requests', 'fas fa-microscope', 'Pages.LabTechnician.TestRequests.Manage'),
    //         new MenuItem(this.l('Lab Receipt'), '/app/lab-technician/lab-receipts', 'fas fa-receipt', 'Pages.LabTechnician'),
    //         // new MenuItem(this.l('Inventory'), '/app/lab-technician/inventory', 'fas fa-boxes', 'Pages.LabTechnician.Inventory.View'),
    //         // new MenuItem(this.l('Reports'), '/app/lab-technician/reports', 'fas fa-file-medical-alt', 'Pages.LabTechnician.Reports.View'),
    //     ]),

    //     // Pharmacist Section
    //     new MenuItem(this.l('Pharmacist'), '', 'fas fa-user-md', 'Pages.Pharmacist', [
    //         new MenuItem(this.l('Inventory'), '/app/pharmacist/pharmacist-inventory', 'fas fa-capsules', 'Pages.Pharmacist.Inventory.View'),
    //         new MenuItem(this.l('Prescriptions'), '/app/pharmacist/pharmacist-prescriptions', 'fas fa-prescription-bottle', 'Pages.Pharmacist.Prescriptions.View'),
    //     ]),
    //     new MenuItem(this.l('Master'), '', 'fas fa-cogs', 'Pages.Users', [
    //         new MenuItem(this.l('Laboratory Master'), '', 'fas fa-flask', 'Pages.Users', [
    //             new MenuItem(this.l('HealthPakage'), '/app/master/helth-package','fas fa-vial','Pages.Users'),
    //             new MenuItem(this.l('Measure Units'), '/app/master/measure-units', 'fas fa-ruler', 'Pages.Users'),
    //             new MenuItem(this.l('LabReport Types'), '/app/master/lab-report-type', 'fas fa-vial', 'Pages.Users'),
    //             new MenuItem(this.l('Test'), '/app/master/lab-test', 'fas fa-microscope', 'Pages.Users'),
    //             new MenuItem(this.l('LabTest Items'), '/app/master/lab-test-items', 'fas fa-flask-vial', 'Pages.Users'),
    //             new MenuItem(this.l('Test Result Limits'), '/app/master/test-result-limit', 'fas fa-flask-vial', 'Pages.Users'),
    //         ]),
    //         new MenuItem(this.l('Room Master'), '', 'fas fa-hospital-alt', 'Pages.Users', [
    //             new MenuItem(this.l('Rooms'), '/app/room', 'fas fa-door-open', 'Pages.Users'),
    //             new MenuItem(this.l('Room Facilities'), '/app/master/room-facilities', 'fas fa-hospital-alt', 'Pages.Users'),
    //                 new MenuItem(this.l('RoomTypes'), '/app/master/room-types', 'fas fa-bed', 'Pages.Users'),
    //             ]),
    //             new MenuItem(this.l('Doctor Master'), '/app/master/doctor-master', 'fas fa-user-md', 'Pages.Users'),
    //             new MenuItem(this.l('Appointment Types'), '/app/master/appointment-types', 'fas fa-calendar-alt', 'Pages.Users'),
    //             // new MenuItem(this.l('Departments'), '/app/master/departments', 'fas fa-building-user', 'Pages.Users'),
    //         ])

    //     ];
    // }

    getMenuItems(): MenuItem[] {
        return [
            new MenuItem(this.l('Dashboard'), '/app/home', 'fas fa-home'),

            // Admin Section
            new MenuItem(this.l('User Management'), '/app/users', 'fas fa-users', 'Pages.Users'),
            new MenuItem(this.l('Role Management'), '/app/roles', 'fas fa-user-shield', 'Pages.Roles'),
            new MenuItem(this.l('Tenant Management'), '/app/tenants', 'fas fa-city', 'Pages.Tenants'),
            new MenuItem(this.l('Patient Admission'), '/app/admission', 'fas fa-procedures', 'Pages.Users'),
            
            new MenuItem(this.l('Emergency Portal'), '', 'fas fa-ambulance', 'Pages.Users', [
                new MenuItem(this.l('Emergency Cases'), '/app/emergency/cases', 'fas fa-notes-medical', 'Pages.Users'),
                new MenuItem(this.l('Triage'), '/app/emergency/triage', 'fas fa-heartbeat', 'Pages.Users'),
                new MenuItem(this.l('Emergency Prescriptions'), '/app/emergency/prescriptions', 'fas fa-prescription', 'Pages.Users'),
                new MenuItem(this.l('Emergency Lab Order'), '/app/emergency/lab-order', 'fas fa-vials', 'Pages.Users'),
            ]),
            // Doctor Section
            new MenuItem(this.l('Doctor Portal'), '', 'fas fa-user-md', 'Pages.Doctors', [
                new MenuItem(this.l('Appointments'), '/app/doctors/view-appointments', 'fas fa-calendar-check', 'Pages.Doctors.Appointments.View'),
                new MenuItem(this.l('Assigned Patients'), '/app/doctors/patients', 'fas fa-user-injured', 'Pages.Doctors.Patients.View'),
                new MenuItem(this.l('Prescriptions'), '/app/doctors/prescriptions', 'fas fa-prescription', 'Pages.Doctors.Prescriptions.Create'),
                new MenuItem(this.l('Patient Visits'), '/app/doctors/visits', 'fas fa-stethoscope', 'Pages.Doctors.Visits.View'),
                new MenuItem(this.l('Lab Orders'), '/app/doctors/lab-order', 'fas fa-vials', 'Pages.Doctors.LabOrders.Create'),
            ]),

            // Nurse Section
            new MenuItem(this.l('Nurse Portal'), '', 'fas fa-user-nurse', 'Pages.Nurse', [
                new MenuItem(this.l('Manage Appointments'), '/app/nurse/appointments', 'fas fa-calendar-plus', 'Pages.Doctors.Appointments.Schedule'),
                new MenuItem(this.l('Patient Assignments'), '/app/nurse/assigned-patients', 'fas fa-procedures', 'Pages.Nurse.Patients.View'),
                new MenuItem(this.l('Vitals Monitoring'), '/app/nurse/vitals-notes', 'fas fa-heartbeat', 'Pages.Nurse.Vitals.View'),
                new MenuItem(this.l('Medication Orders'), '/app/nurse/order-medicine', 'fas fa-pills', 'Pages.Nurse.Medication.View'),
            ]),

            // Billing Staff Section
            new MenuItem(this.l('Billing Portal'), '', 'fas fa-file-invoice-dollar', 'Pages.BillingStaff', [
                new MenuItem(this.l('Invoices'), '/app/billing-staff/invoices', 'fas fa-receipt', 'Pages.BillingStaff.Invoices.View'),
                new MenuItem(this.l('Patient Deposits'), '/app/deposit', 'fas fa-hand-holding-usd', 'Pages.BillingStaff'),
            ]),

            // Lab Technician Section
            new MenuItem(this.l('Lab Technician Portal'), '', 'fas fa-vial', 'Pages.LabTechnician', [
                new MenuItem(this.l('Test Requests'), '/app/lab-technician/test-requests', 'fas fa-microscope', 'Pages.LabTechnician.TestRequests.Manage'),
                new MenuItem(this.l('Lab Receipts'), '/app/lab-technician/lab-receipts', 'fas fa-file-medical', 'Pages.LabTechnician'),
            ]),

            // Pharmacist Section
            new MenuItem(this.l('Pharmacy Portal'), '', 'fas fa-clinic-medical', 'Pages.Pharmacist', [
                new MenuItem(this.l('Drug Inventory'), '/app/pharmacist/pharmacist-inventory', 'fas fa-capsules', 'Pages.Pharmacist.Inventory.View'),
                new MenuItem(this.l('Prescription Fulfillment'), '/app/pharmacist/pharmacist-prescriptions', 'fas fa-prescription-bottle-alt', 'Pages.Pharmacist.Prescriptions.View'),
            ]),

            // Master Section
            new MenuItem(this.l('Masters'), '', 'fas fa-cogs', 'Pages.Users', [

                new MenuItem(this.l('Laboratory Settings'), '', 'fas fa-flask', 'Pages.Users', [
                    new MenuItem(this.l('Health Packages'), '/app/master/helth-package', 'fas fa-vial', 'Pages.Users'),
                    new MenuItem(this.l('Measurement Units'), '/app/master/measure-units', 'fas fa-ruler-combined', 'Pages.Users'),
                    new MenuItem(this.l('Lab Report Types'), '/app/master/lab-report-type', 'fas fa-notes-medical', 'Pages.Users'),
                    new MenuItem(this.l('Lab Tests'), '/app/master/lab-test', 'fas fa-microscope', 'Pages.Users'),
                    new MenuItem(this.l('Lab Test Items'), '/app/master/lab-test-items', 'fas fa-vials', 'Pages.Users'),
                    new MenuItem(this.l('Test Result Limits'), '/app/master/test-result-limit', 'fas fa-sliders-h', 'Pages.Users'),
                ]),

                new MenuItem(this.l('Room Management'), '', 'fas fa-hospital-alt', 'Pages.Users', [
                    new MenuItem(this.l('Room Facilities'), '/app/master/room-facilities', 'fas fa-procedures', 'Pages.Users'),
                    new MenuItem(this.l('Room Types'), '/app/master/room-types', 'fas fa-bed', 'Pages.Users'),
                    new MenuItem(this.l('Rooms'), '/app/room', 'fas fa-door-open', 'Pages.Users'),
                    new MenuItem(this.l('Beds'), '/app/master/beds', 'fas fa-procedures', 'Pages.Users'),
                ]),
                new MenuItem(this.l('Procedure Types'), '/app/master/emergency-procedure', 'fas fa-fas fa-syringe', 'Pages.Users'),
                new MenuItem(this.l('Departments Types'), '/app/master/department', 'fas fa-sitemap', 'Pages.Users'),
                new MenuItem(this.l('Doctor Master Data'), '/app/master/doctor-master', 'fas fa-user-md', 'Pages.Users'),
                new MenuItem(this.l('Appointment Types'), '/app/master/appointment-types', 'fas fa-calendar-alt', 'Pages.Users'),
                new MenuItem(this.l('Emergency Charges'), '/app/master/emergency-charges', 'fas fa-kit-medical', 'Pages.Users'),
            ])
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
