import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BedsComponent } from './beds.component';

describe('BedsComponent', () => {
  let component: BedsComponent;
  let fixture: ComponentFixture<BedsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BedsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BedsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
