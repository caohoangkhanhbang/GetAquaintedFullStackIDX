import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TutorialListingComponent } from './tutorial-listing.component';

describe('UserListingComponent', () => {
  let component: TutorialListingComponent;
  let fixture: ComponentFixture<TutorialListingComponent>;
  let v: any;
  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TutorialListingComponent]
    });
    fixture = TestBed.createComponent(v);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
