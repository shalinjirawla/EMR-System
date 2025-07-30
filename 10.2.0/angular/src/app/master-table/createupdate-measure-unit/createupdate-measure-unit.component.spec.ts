import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateMeasureUnitComponent } from './createupdate-measure-unit.component';

describe('CreateupdateMeasureUnitComponent', () => {
  let component: CreateupdateMeasureUnitComponent;
  let fixture: ComponentFixture<CreateupdateMeasureUnitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateMeasureUnitComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateMeasureUnitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
