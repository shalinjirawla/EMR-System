import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateHealthPackageComponent } from './createupdate-health-package.component';

describe('CreateupdateHealthPackageComponent', () => {
  let component: CreateupdateHealthPackageComponent;
  let fixture: ComponentFixture<CreateupdateHealthPackageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateHealthPackageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateHealthPackageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
