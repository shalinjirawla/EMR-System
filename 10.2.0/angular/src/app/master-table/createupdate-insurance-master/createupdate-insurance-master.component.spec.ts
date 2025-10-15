import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateInsuranceMasterComponent } from './createupdate-insurance-master.component';

describe('CreateupdateInsuranceMasterComponent', () => {
  let component: CreateupdateInsuranceMasterComponent;
  let fixture: ComponentFixture<CreateupdateInsuranceMasterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateInsuranceMasterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateInsuranceMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
