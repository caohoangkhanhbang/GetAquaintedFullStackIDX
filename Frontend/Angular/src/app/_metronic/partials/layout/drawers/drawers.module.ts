import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { ActivityDrawerComponent } from './activity-drawer/activity-drawer.component';
import { MessengerDrawerComponent } from './messenger-drawer/messenger-drawer.component';
import { ChatInnerModule } from '../../content/chat-inner/chat-inner.module';
import { SharedModule } from "../../../shared/shared.module";
import { UserDrawerComponent } from './user-drawer/user-drawer.component';
import { TranslateModule } from '@ngx-translate/core';
import { AvatarModule } from 'ngx-avatars';
import { AngularModule } from 'src/app/angular.module';
import { NotifyDrawerComponent } from './notify-drawer/notify-drawer.component';

@NgModule({
  declarations: [
    ActivityDrawerComponent,
    MessengerDrawerComponent,
    UserDrawerComponent,//Thông tin người dùng
    NotifyDrawerComponent,//Danh sách thông báo
  ],
  imports: [CommonModule, InlineSVGModule, RouterModule, ChatInnerModule, SharedModule,
    TranslateModule,
    AvatarModule,
    AngularModule,
  ],
  exports: [
    ActivityDrawerComponent,
    MessengerDrawerComponent,
    UserDrawerComponent,//Thông tin người dùng
    NotifyDrawerComponent,//Danh sách thông báo
  ],
})
export class DrawersModule {}
