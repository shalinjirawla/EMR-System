import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacistInventoryComponent } from './pharmacist-inventory.component';

describe('PharmacistInventoryComponent', () => {
  let component: PharmacistInventoryComponent;
  let fixture: ComponentFixture<PharmacistInventoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PharmacistInventoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PharmacistInventoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
