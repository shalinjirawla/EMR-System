import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { RoomRoutingModule } from './room-routing.module';

@NgModule({
  imports: [
    SharedModule,
    CommonModule,
    RoomRoutingModule,
  ],
})
export class RoomModule {}
