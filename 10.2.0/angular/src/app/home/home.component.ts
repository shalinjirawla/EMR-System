import { Component, Injector, ChangeDetectionStrategy, ChangeDetectorRef, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ButtonModule } from 'primeng/button';
import { ChartModule } from 'primeng/chart';
import { DashboardSummaryDto, HomeServiceProxy } from '@shared/service-proxies/service-proxies';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { TableModule } from 'primeng/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
@Component({
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    animations: [appModuleAnimation()],
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [ChartModule, ButtonModule, AvatarModule, AvatarGroupModule,
        CommonModule, FormsModule, DatePipe,TableModule,RouterLink],
    providers: [HomeServiceProxy]
})
export class HomeComponent extends AppComponentBase implements OnInit {
    shownLoginName!: string
    chartData: any;
    chartOptions: any;
    patientCount = 108;
    percentageChange = 20;
    summaryData: DashboardSummaryDto;
    patientsChartData: { labels: any[]; datasets: { data: any[]; fill: boolean; borderColor: string; tension: number; }[]; };
    doctorsChartData: { labels: any[]; datasets: { data: any[]; fill: boolean; borderColor: string; tension: number; }[]; };
    nursesChartData: { labels: any[]; datasets: { data: any[]; fill: boolean; borderColor: string; tension: number; }[]; };
    appointmentsChartData: { labels: any[]; datasets: { data: any[]; fill: boolean; borderColor: string; tension: number; }[]; };
    departmentChartData: {
        labels: any[]; datasets: { data: any[]; backgroundColor: string[];hoverBackgroundColor:string[]; borderColor?: string; fill?: boolean; tension?: number; }[];
    };

    departmentChartOptions: { cutout: string; plugins: { legend: { position: string; labels: { usePointStyle: boolean; pointStyle: string; }; }; }; };
    constructor(injector: Injector,
        private _homeService: HomeServiceProxy,
        protected cd: ChangeDetectorRef,
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.shownLoginName = this.appSession.user.name;
        this.loadSummary();
    }
    loadSummary() {
        this._homeService.getSummary().subscribe((data) => {
            this.summaryData = data;

            const mapToChart = (chartData: any[]) => ({
                labels: chartData.map(c => c.label),
                datasets: [
                    {
                        data: chartData.map(c => c.value),
                        fill: false,
                        borderColor: '#42A5F5',
                        tension: 0.4
                    }
                ]
            });
            this.patientsChartData = mapToChart(data.patientsChart);
            this.appointmentsChartData = mapToChart(data.appointmentsChart);
            this.doctorsChartData = mapToChart(data.doctorsChart);
            this.nursesChartData = mapToChart(data.nursesChart);


            this.departmentChartData = {
                labels: data.departmentWiseAppointments.map(d => d.label),
                datasets: [
                    {
                        data: data.departmentWiseAppointments.map(d => d.value),
                        backgroundColor: [
                            '#42A5F5',
                            '#212121',
                            '#EC407A',
                            '#FF9800',
                            '#FFB74D',
                            '#3F51B5'
                        ],
                        hoverBackgroundColor: [
                            '#64B5F6',
                            '#424242',
                            '#F06292',
                            '#FFB74D',
                            '#FFD54F',
                            '#5C6BC0'
                        ]
                    }
                ]
            };

            this.departmentChartOptions = {
                cutout: '70%',
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            usePointStyle: true,
                            pointStyle: 'circle'
                        }
                    }
                }
            };

            this.cd.detectChanges();
            console.log("totalAppointmentList : ", data.totalMedicineList)
        });
    }
    getShortName(fullName: string) {
        if (!fullName) return '';
        return fullName.trim().charAt(0).toUpperCase();
    }

}
