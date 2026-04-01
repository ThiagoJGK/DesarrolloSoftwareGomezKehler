import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TourismComponent } from './tourism.component';
import { ListDestinationsComponent } from './list-destinations/list-destinations.component';
import { UserDashboardComponent } from './user-dashboard/user-dashboard.component';
import { DestinationDetailComponent } from './destination-detail/destination-detail.component';

const routes: Routes = [
  { path: '', component: ListDestinationsComponent },
  { path: 'dashboard', component: UserDashboardComponent },
  { path: 'destination/:id', component: DestinationDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TourismRoutingModule { }
