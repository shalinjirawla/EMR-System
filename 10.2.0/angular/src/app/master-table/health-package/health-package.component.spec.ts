import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HealthPackageComponent } from './health-package.component';

describe('HealthPackageComponent', () => {
  let component: HealthPackageComponent;
  let fixture: ComponentFixture<HealthPackageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HealthPackageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HealthPackageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
