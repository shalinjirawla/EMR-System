import { Component } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { AddDataDialogComponent } from './add-data-dialog/add-data-dialog.component';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-master-table',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  providers: [DialogService],
  templateUrl: './master-table.component.html',
  styleUrls: ['./master-table.component.css']
})
export class MasterTableComponent {
  constructor(private dialogService: DialogService) {}

  createReport(): void {
    this.showCreateDialog('report');
  }

  createDiagnosis(): void {
    this.showCreateDialog('diagnosis');
  }

  private showCreateDialog(dataType: 'report' | 'diagnosis'): void {
    const dialogRef = this.dialogService.open(AddDataDialogComponent, {
      header: `Add New ${this.capitalizeFirstLetter(dataType)}`,
      width: '450px',
      data: { type: dataType },
      dismissableMask: true,
      styleClass: 'custom-dialog'
    });

    dialogRef.onClose.subscribe((shouldRefresh: boolean) => {
      if (shouldRefresh) {
        this.refresh();
      }
    });
  }

  private refresh(): void {
    // Implement your refresh logic here
    console.log('Refreshing data...');
  }

  private capitalizeFirstLetter(string: string): string {
    return string.charAt(0).toUpperCase() + string.slice(1);
  }
}