import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateRoomFacilitiesComponent } from './createupdate-room-facilities.component';

describe('CreateupdateRoomFacilitiesComponent', () => {
  let component: CreateupdateRoomFacilitiesComponent;
  let fixture: ComponentFixture<CreateupdateRoomFacilitiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateRoomFacilitiesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateRoomFacilitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
