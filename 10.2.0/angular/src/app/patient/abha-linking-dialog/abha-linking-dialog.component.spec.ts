import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AbhaLinkingDialogComponent } from './abha-linking-dialog.component';

describe('AbhaLinkingDialogComponent', () => {
  let component: AbhaLinkingDialogComponent;
  let fixture: ComponentFixture<AbhaLinkingDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AbhaLinkingDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AbhaLinkingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
