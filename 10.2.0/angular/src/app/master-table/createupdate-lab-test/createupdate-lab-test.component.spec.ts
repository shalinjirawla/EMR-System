import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateLabTestComponent } from './createupdate-lab-test.component';

describe('CreateupdateLabTestComponent', () => {
  let component: CreateupdateLabTestComponent;
  let fixture: ComponentFixture<CreateupdateLabTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateLabTestComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateLabTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
