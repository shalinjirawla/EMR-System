import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabOrdersComponent } from './lab-orders.component';

describe('LabOrdersComponent', () => {
  let component: LabOrdersComponent;
  let fixture: ComponentFixture<LabOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabOrdersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
