import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InsuranceMasterComponent } from './insurance-master.component';

describe('InsuranceMasterComponent', () => {
  let component: InsuranceMasterComponent;
  let fixture: ComponentFixture<InsuranceMasterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsuranceMasterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InsuranceMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
