import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { CommonModule, DatePipe } from '@angular/common';
import { CollectionStatus, PharmacistPrescriptionsServiceProxy, ViewPharmacistPrescriptionsDto } from '../../../shared/service-proxies/service-proxies';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
@Component({
  selector: 'app-view-pharmacist-prescription',
  imports: [CommonModule, FormsModule, TableModule, TagModule, DatePipe, CardModule],
  templateUrl: './view-pharmacist-prescription.component.html',
  styleUrl: './view-pharmacist-prescription.component.css',
  providers: [PharmacistPrescriptionsServiceProxy]
})
export class ViewPharmacistPrescriptionComponent implements OnInit {
  id: number;
  prescription!: ViewPharmacistPrescriptionsDto;
  constructor(private _pharmacistPrescriptionsService: PharmacistPrescriptionsServiceProxy,
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
    this._pharmacistPrescriptionsService.viewPharmacistPrescriptionsReceipt(this.id).subscribe(res => {
      this.prescription = res;
      this.cd.detectChanges();
    })
  }
  statusOptions = [
    { label: 'NotPickedUp', value: CollectionStatus._0 },
    { label: 'PickedUp', value: CollectionStatus._1 },
  ];
  getStatusLabel(value: number): string {
    const status = this.statusOptions.find(s => s.value === value);
    return status ? status.label : '';
  }
  getStatusSeverity(value: number): 'info' | 'warn' | 'success' {
    switch (value) {
      case CollectionStatus._0: return 'warn';        
      case CollectionStatus._1: return 'success';   
      default: return 'info';
    }
  }
}
