import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { ControlValueAccessor, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-nurse',
  imports: [FormsModule,CommonModule],
  templateUrl: './create-nurse.component.html',
  styleUrl: './create-nurse.component.css'
})
export class CreateNurseComponent implements ControlValueAccessor {
   @Output() nurseDataChange = new EventEmitter<any>();
    
nurseData = {
  phoneNumber: '',
  gender: '',
  shiftTiming: '',
  department: '',
  qualification: '',
  yearsOfExperience: 0
};

genders = ['Male', 'Female', 'Other'];

  private onChange: any = () => {};
  private onTouched: any = () => {};

  writeValue(value: any): void {
    if (value) {
      this.nurseData = value;
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  updateData() {
    this.onChange(this.nurseData);
    this.onTouched();
    this.nurseDataChange.emit(this.nurseData);
  }
}
