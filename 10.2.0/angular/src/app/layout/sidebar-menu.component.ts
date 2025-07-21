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

    getMenuItems(): MenuItem[] {
        return [
            new MenuItem(this.l('HomePage'), '/app/home', 'fas fa-home'),

            // Admin Section
            new MenuItem(this.l('Users'), '/app/users', 'fas fa-users', 'Pages.Users'),
            new MenuItem(this.l('Roles'), '/app/roles', 'fas fa-user-shield', 'Pages.Roles'),
            new MenuItem(this.l('Tenants'), '/app/tenants', 'fas fa-building', 'Pages.Tenants'),
            //new MenuItem(this.l('Rooms'), '/app/room', 'fas fa-hotel', 'Pages.Users'),
            new MenuItem(this.l('Admit Patient'), '/app/admission', 'fas fa-hospital-user', 'Pages.Users'),
            new MenuItem(this.l('Deposit'), '/app/deposit', 'fas fa-money-bill', 'Pages.Users'),
            new MenuItem(this.l('Rooms'), '/app/room', 'fas fa-hotel', 'Pages.Users'),
            new MenuItem(this.l('Master'), '', 'fas fa-building', 'Pages.Users',[
                new MenuItem(this.l('Room Facilities'), '/app/master/room-facilities', 'fas fa-hospital', 'Pages.Users'),
                //new MenuItem(this.l('Departments'), '/app/master/departments', 'fas fa-building-user', 'Pages.Users'),
                new MenuItem(this.l('RoomTypes'), '/app/master/room-types', 'fas fa-hotel', 'Pages.Users'),
                new MenuItem(this.l('LabTest Types'), '/app/master/lab-report-type', 'fas fa-microscope', 'Pages.Users'),
                new MenuItem(this.l('Doctor Master'), '/app/master/doctor-master', 'fas fa-user-md', 'Pages.Users'),
            ]),

            // Doctor Section
            new MenuItem(this.l('Doctor'), '', 'fa fa-user-md', 'Pages.Doctors', [
                new MenuItem(this.l('View Appointments'), '/app/doctors/view-appointments', 'fas fa-calendar-check', 'Pages.Doctors.Appointments.View'),
                new MenuItem(this.l('Patients'), '/app/doctors/patients', 'fas fa-user-injured', 'Pages.Doctors.Patients.View'),
                new MenuItem(this.l('Prescriptions'), '/app/doctors/prescriptions', 'fas fa-file-prescription', 'Pages.Doctors.Prescriptions.Create'),
                new MenuItem(this.l('Visits'), '/app/doctors/visits', 'fas fa-procedures', 'Pages.Doctors.Visits.View'),
                new MenuItem(this.l('Lab Orders'), '/app/doctors/lab-order', 'fas fa-vials', 'Pages.Doctors.LabOrders.Create'),
            ]),

            // Nurse Section
            new MenuItem(this.l('Nurse'), '', 'fas fa-user-nurse', 'Pages.Nurse', [
                new MenuItem(this.l('Appointments'), '/app/nurse/appointments', 'fas fa-procedures', 'Pages.Doctors.Appointments.Schedule'),
                new MenuItem(this.l('Assigned Patients'), '/app/nurse/assigned-patients', 'fas fa-procedures', 'Pages.Nurse.Patients.View'),
                new MenuItem(this.l('Vitals'), '/app/nurse/vitals-notes', 'fas fa-notes-medical', 'Pages.Nurse.Vitals.View'),
                // new MenuItem(this.l('Add Vitals'), '/app/nurse/vitals-notes/add', 'fas fa-plus-circle', 'Pages.Nurse.Vitals.Create'),
                new MenuItem(this.l('Order Medication'), '/app/nurse/order-medicine', 'fas fa-pills', 'Pages.Nurse.Medication.View'),
            ]),

            // Billing Staff Section
            new MenuItem(this.l('Billing Staff'), '', 'fas fa-file-invoice-dollar', 'Pages.BillingStaff', [
                new MenuItem(this.l('Invoices'), '/app/billing-staff/invoices', 'fas fa-file-invoice', 'Pages.BillingStaff.Invoices.View'),
                new MenuItem(this.l('Payments'), '/app/billing-staff/payments', 'fas fa-credit-card', 'Pages.BillingStaff.Payments.View'),
                new MenuItem(this.l('Insurance'), '/app/billing-staff/insurance', 'fas fa-file-invoice-dollar', 'Pages.BillingStaff.Insurance.View'),
                new MenuItem(this.l('Reports'), '/app/billing-staff/reports', 'fas fa-chart-line', 'Pages.BillingStaff.Reports.View'),
            ]),

            // Lab Technician Section
            new MenuItem(this.l('Lab Technician'), '', 'fas fa-flask', 'Pages.LabTechnician', [
                // new MenuItem(this.l('Inventory'), '/app/lab-technician/inventory', 'fas fa-boxes', 'Pages.LabTechnician.Inventory.View'),
                // new MenuItem(this.l('Reports'), '/app/lab-technician/reports', 'fas fa-file-medical-alt', 'Pages.LabTechnician.Reports.View'),
                new MenuItem(this.l('Test Requests'), '/app/lab-technician/test-requests', 'fas fa-microscope', 'Pages.LabTechnician.TestRequests.Manage'),
            ]),

            // Pharmacist Section
            new MenuItem(this.l('Pharmacist'), '', 'fas fa-user-md', 'Pages.Pharmacist', [
                new MenuItem(this.l('Inventory'), '/app/pharmacist/pharmacist-inventory', 'fas fa-capsules', 'Pages.Pharmacist.Inventory.View'),
                new MenuItem(this.l('Prescriptions'), '/app/pharmacist/pharmacist-prescriptions', 'fas fa-prescription-bottle', 'Pages.Pharmacist.Prescriptions.View'),
            ]),
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
