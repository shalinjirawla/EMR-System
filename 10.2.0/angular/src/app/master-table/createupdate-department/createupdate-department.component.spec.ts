import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateDepartmentComponent } from './createupdate-department.component';

describe('CreateupdateDepartmentComponent', () => {
  let component: CreateupdateDepartmentComponent;
  let fixture: ComponentFixture<CreateupdateDepartmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateDepartmentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateDepartmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
