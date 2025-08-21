import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateupdateBedsComponent } from './createupdate-beds.component';

describe('CreateupdateBedsComponent', () => {
  let component: CreateupdateBedsComponent;
  let fixture: ComponentFixture<CreateupdateBedsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateupdateBedsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateupdateBedsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
