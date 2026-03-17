import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NgbModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';



@NgModule({
  imports: [
    CommonModule,
    NgbModule,
    NgbTooltipModule,
  ],
  providers: [
   
  ],
  exports:[
    CommonModule,
    NgbModule,
    NgbTooltipModule,
  ],
})
export class AngularModule { }
