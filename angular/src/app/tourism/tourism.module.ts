import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TourismRoutingModule } from './tourism-routing.module';
import { TourismComponent } from './tourism.component';


@NgModule({
  declarations: [
    TourismComponent
  ],
  imports: [
    CommonModule,
    TourismRoutingModule
  ]
})
export class TourismModule { }
