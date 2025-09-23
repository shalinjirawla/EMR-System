import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicineStockComponent } from './medicine-stock.component';

describe('MedicineStockComponent', () => {
  let component: MedicineStockComponent;
  let fixture: ComponentFixture<MedicineStockComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicineStockComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicineStockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
