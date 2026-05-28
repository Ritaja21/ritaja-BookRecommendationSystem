import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerDasboardComponent } from './customer-dasboard.component';

describe('CustomerDasboardComponent', () => {
  let component: CustomerDasboardComponent;
  let fixture: ComponentFixture<CustomerDasboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerDasboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerDasboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
