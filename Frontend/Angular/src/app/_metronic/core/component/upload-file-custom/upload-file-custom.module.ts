import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

//Directive Module
import { DndDirectiveModule } from './directives/dnd.directive.module';

//Component Module
import { UploadFileCustomComponent } from './upload-file-custom.component';

//Material
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';

//Pipes
import { SafePipe } from './pipes/safe.pipe';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
    declarations: [
        UploadFileCustomComponent,
        SafePipe
    ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        DndDirectiveModule,

        //Material
        MatButtonModule,
        MatExpansionModule,
        MatTooltipModule,
        MatTableModule,
        MatCheckboxModule,
        MatRadioModule,
        MatIconModule,
    ],
    providers: [],
    exports: [
        UploadFileCustomComponent
    ]
})
export class UploadFileCustomModule { }