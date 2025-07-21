import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateRoomTypesComponent } from './createupdate-room-types.component';

describe('CreateupdateRoomTypesComponent', () => {
  let component: CreateupdateRoomTypesComponent;
  let fixture: ComponentFixture<CreateupdateRoomTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateRoomTypesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateRoomTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
