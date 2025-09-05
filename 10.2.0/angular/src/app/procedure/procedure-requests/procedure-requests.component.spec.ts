import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcedureRequestsComponent } from './procedure-requests.component';

describe('ProcedureRequestsComponent', () => {
  let component: ProcedureRequestsComponent;
  let fixture: ComponentFixture<ProcedureRequestsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProcedureRequestsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProcedureRequestsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
