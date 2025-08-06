import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateLabTestItemsComponent } from './createupdate-lab-test-items.component';

describe('CreateupdateLabTestItemsComponent', () => {
  let component: CreateupdateLabTestItemsComponent;
  let fixture: ComponentFixture<CreateupdateLabTestItemsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateLabTestItemsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateLabTestItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
