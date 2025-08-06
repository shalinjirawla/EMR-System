import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateTestResultLimitComponent } from './createupdate-test-result-limit.component';

describe('CreateupdateTestResultLimitComponent', () => {
  let component: CreateupdateTestResultLimitComponent;
  let fixture: ComponentFixture<CreateupdateTestResultLimitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateTestResultLimitComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateTestResultLimitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
