import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestResultLimitComponent } from './test-result-limit.component';

describe('TestResultLimitComponent', () => {
  let component: TestResultLimitComponent;
  let fixture: ComponentFixture<TestResultLimitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestResultLimitComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestResultLimitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
