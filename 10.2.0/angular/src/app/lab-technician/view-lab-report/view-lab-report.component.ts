import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@node_modules/@angular/common';
import { FormsModule } from '@node_modules/@angular/forms';
import { TagModule } from 'primeng/tag';
import { LabReportResultItemServiceProxy, ViewLabReportDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-view-lab-report',
  imports: [CommonModule, FormsModule,TagModule,DatePipe],
  templateUrl: './view-lab-report.component.html',
  styleUrl: './view-lab-report.component.css',
  providers: [LabReportResultItemServiceProxy]
})
export class ViewLabReportComponent implements OnInit {
  id: number;
  report!: ViewLabReportDto;
  constructor(private _labReportResultItemService: LabReportResultItemServiceProxy,
    private cd: ChangeDetectorRef) {

  }
  ngOnInit(): void {
    this.GetData();
  }
  getSeverity(flag: string | undefined): string {
    switch (flag?.toLowerCase()) {
      case 'high': return 'danger';
      case 'low': return 'warn';
      case 'normal': return 'success';
      default: return 'info';
    }
  }
  calculateAgeDisplay(dob: string | Date): string {
    const birthDate = new Date(dob);
    const today = new Date();

    const diffInMs = today.getTime() - birthDate.getTime();
    const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

    const years = today.getFullYear() - birthDate.getFullYear();
    const months =
      today.getMonth() - birthDate.getMonth() +
      years * 12 -
      (today.getDate() < birthDate.getDate() ? 1 : 0);

    if (years >= 1) {
      return `${years} year${years > 1 ? 's' : ''}`;
    } else if (months >= 1) {
      return `${months} month${months > 1 ? 's' : ''}`;
    } else {
      return `${diffInDays} day${diffInDays > 1 ? 's' : ''}`;
    }
  }
  GetData() {
    this._labReportResultItemService.viewLabReport(this.id).subscribe(res => {
      this.report = res;
      this.cd.detectChanges();
    })
  }
  hasAnyRange(items: any[]): boolean {
  return items?.some(item => item.minValue !== null && item.maxValue !== null);
}
}
