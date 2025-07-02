import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { DatePickerModule } from '@node_modules/primeng/datepicker';
import { AppComponentBase } from '@shared/app-component-base';
import { AppointmentServiceProxy, CreateUpdateInvoiceDto, InvoiceItemDto, InvoiceItemType, InvoiceServiceProxy, InvoiceStatus, PatientDropDownDto, PatientServiceProxy, PaymentMethod } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';

import { AbpModalFooterComponent } from "../../../shared/components/modal/abp-modal-footer.component";
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { DropdownModule } from 'primeng/dropdown';
import { SelectModule } from 'primeng/select';
import moment from 'moment';

@Component({
  selector: 'app-create-invoice',
  imports: [FormsModule, CalendarModule, DropdownModule, CommonModule, SelectModule, AbpModalFooterComponent, AbpModalHeaderComponent],
  templateUrl: './create-invoice.component.html',
  styleUrl: './create-invoice.component.css',
  providers: [PatientServiceProxy, AppointmentServiceProxy, InvoiceServiceProxy]
})
export class CreateInvoiceComponent extends AppComponentBase implements OnInit {
  @ViewChild('invoiceForm') invoiceForm: NgForm;
  @Output() onSave = new EventEmitter<any>();

  saving = false;
  patients: PatientDropDownDto[] = [];
  appointments: any[] = [];
  invoiceData: any;
  // Add this to your component class
  moment = moment;

  statusOptions = [
    { label: 'Unpaid', value: InvoiceStatus._0 },
    { label: 'Paid', value: InvoiceStatus._1 },
    { label: 'Partial Paid', value: InvoiceStatus._2 }
  ];

  paymentMethodOptions = [
    { label: 'Cash', value: PaymentMethod._0 },
    { label: 'Card', value: PaymentMethod._1 }
  ];

  // Update your invoice model in the component
  invoice = {
    tenantId: abp.session.tenantId,
    patientId: null as number | null,
    appointmentId: null as number | null,
    dueDate: moment().add(15, 'days'),
    status: InvoiceStatus._0, // Unpaid
    paymentMethod: null as PaymentMethod | null,
    items: [] as InvoiceItemDto[]
  };

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _appointmentService: AppointmentServiceProxy,
    private _invoiceService: InvoiceServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this._patientService.patientDropDown()
      .subscribe({
        next: (res) => this.patients = res,
        error: () => this.notify.error('Failed to load patients')
      });
  }

  onPatientChange(patientId: number): void {
    this.invoice.appointmentId = null;
    this.invoiceData = null;
    this.invoice.items = [];

    if (patientId) {
      this.loadAppointments(patientId);
    } else {
      this.appointments = [];
    }
  }

  loadAppointments(patientId: number): void {
    this._appointmentService.getByPatient(patientId)
      .subscribe({
        next: (res) => {
          // Access appointments through res.result.items
          this.appointments = res.items.map(appointment => ({
            ...appointment,
            displayText: `${this.formatTime(appointment.startTime)} - ${this.formatTime(appointment.endTime)} - ${appointment.doctor?.fullName || 'Unknown Doctor'}`
          }));
        },
        error: (err) => {
          this.notify.error('Failed to load appointments');
          console.error('Error loading appointments:', err);
        }
      });
  }

  // Time formatting helper
  private formatTime(timeString: string): string {
    if (!timeString) return '';

    try {
      // Handle time strings like "11:00:53" or "15:10:36"
      const timeParts = timeString.split(':');
      const hours = parseInt(timeParts[0], 10);
      const minutes = timeParts[1];

      const period = hours >= 12 ? 'PM' : 'AM';
      const displayHours = hours % 12 || 12; // Convert 0 to 12 for 12-hour format

      return `${displayHours}:${minutes} ${period}`;
    } catch (e) {
      console.warn('Could not format time:', timeString);
      return timeString; // Return original if formatting fails
    }
  }

  onAppointmentChange(appointmentId: number): void {
    if (appointmentId) {
      this.loadInvoiceDetails(appointmentId);
    } else {
      this.invoiceData = null;
      this.invoice.items = [];
    }
  }

  loadInvoiceDetails(appointmentId: number): void {
    this._invoiceService.getInvoiceDetailsByAppointmentIdUsingSp(appointmentId)
      .subscribe({
        next: (result) => {
          this.invoiceData = result;
          this.prepareInvoiceItems();
        },
        error: () => this.notify.error('No prescription found for the given appointment')
      });
  }

  prepareInvoiceItems(): void {
    this.invoice.items = [];
    
    // Add consultation fee
    const consultationFee = this.invoiceData.consultationFee;
    this.invoice.items.push({
      itemType: InvoiceItemType._0, // Consultation
      description: 'Consultation Fee',
      unitPrice: consultationFee,
      quantity: 1,
      totalPrice: consultationFee * 1 // quantity is 1
    } as InvoiceItemDto);

    // Add lab tests
    this.invoiceData.labTests.forEach(test => {
      const testPrice = test.price;
      const testQuantity = 1;
      this.invoice.items.push({
        itemType: InvoiceItemType._2, // LabTest
        description: test.testName,
        unitPrice: testPrice,
        quantity: testQuantity,
        totalPrice: testPrice * testQuantity,
      } as InvoiceItemDto);
    });

    // Add medicines
    this.invoiceData.medicines.forEach(medicine => {
      const medicinePrice = medicine.price;
      const medicineQuantity = medicine.quantity;
      this.invoice.items.push({
        itemType: InvoiceItemType._1, // Medicine
        description: medicine.medicineName,
        unitPrice: medicinePrice,
        quantity: medicineQuantity,
        totalPrice: medicinePrice * medicineQuantity
      } as InvoiceItemDto);
    });
}

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  isSaveDisabled(): boolean {
    return !this.invoiceForm?.valid || this.saving || !this.invoiceData;
  }

  save(): void {
    this.saving = true;

    // Create the invoice items first
    const invoiceItems = this.invoice.items.map(item => {
        const itemDto = new InvoiceItemDto();
        itemDto.itemType = item.itemType;
        itemDto.description = item.description;
        itemDto.unitPrice = item.unitPrice;
        itemDto.quantity = item.quantity;
        return itemDto;
    });

    // Create the complete invoice DTO
    const input = new CreateUpdateInvoiceDto({
       id: 0,
        tenantId: abp.session.tenantId,
        patientId: this.invoice.patientId,
        appointmentId: this.invoice.appointmentId,
        invoiceDate: moment(), // Current date/time
        dueDate: moment(this.invoice.dueDate),
        status: this.invoice.status,
        paymentMethod: this.invoice.paymentMethod,
        items: invoiceItems,
        subTotal:this.invoiceData.subTotal , 
        gstAmount: this.invoiceData.gstAmount,
        totalAmount: this.invoiceData.totalAmount
    });
debugger
    this._invoiceService.create(input)
        .subscribe({
            next: () => {
                this.notify.success('Invoice created successfully');
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            error: (err) => {
                this.notify.error('Failed to create invoice');
                console.error('Invoice creation error:', err);
                this.saving = false;
            }
        });
}
}
