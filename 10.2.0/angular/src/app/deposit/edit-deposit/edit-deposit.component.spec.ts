import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditDepositComponent } from './edit-deposit.component';

describe('EditDepositComponent', () => {
  let component: EditDepositComponent;
  let fixture: ComponentFixture<EditDepositComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditDepositComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditDepositComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
