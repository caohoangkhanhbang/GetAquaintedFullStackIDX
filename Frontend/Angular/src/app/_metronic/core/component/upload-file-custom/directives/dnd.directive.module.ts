import { NgModule } from '@angular/core';
import { DndDirective } from './dnd.directive';

@NgModule({
  declarations: [DndDirective],
  exports: [DndDirective]
})

export class DndDirectiveModule { }