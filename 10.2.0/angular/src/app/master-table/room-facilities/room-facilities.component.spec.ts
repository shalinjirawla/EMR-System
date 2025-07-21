import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomFacilitiesComponent } from './room-facilities.component';

describe('RoomFacilitiesComponent', () => {
  let component: RoomFacilitiesComponent;
  let fixture: ComponentFixture<RoomFacilitiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomFacilitiesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoomFacilitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
