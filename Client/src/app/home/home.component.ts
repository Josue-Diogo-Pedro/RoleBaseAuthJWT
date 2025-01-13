import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HousingLocationComponent } from '../housing-location/housing-location.component';
import {HousingLocation} from '../housinglocation';
import { HousingService } from '../housing.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    HousingLocationComponent
  ],
  template: `
    <section>
      <form>
        <input type="text" placeholder="Filter by city" #filter (input)="filterResults(filter.value)">
        <!-- <button class="primary" type="button" (click)="filterResults(filter.value)">Search</button> -->
      </form>
    </section>
    <section class="results">
      <app-housing-location *ngFor="let housingLocation of filteredLocationList" 
      [housingLocation]="housingLocation"></app-housing-location>
    </section>
  `,
  styleUrls: ['./home.component.css'],
})

export class HomeComponent{

  housingService: HousingService = inject(HousingService);
  constructor() {
    this.housingService.getAllHousingLocations().then((housingLocationList: HousingLocation[]) =>{
      this.housingLocationList = housingLocationList;
      this.filteredLocationList = this.housingLocationList;
    });
  }

  housingLocationList: HousingLocation[] = [];
  filteredLocationList: HousingLocation[] = [];

  filterResults(text: string){

    if(!text){
      this.filteredLocationList = this.housingLocationList;
      return;
    }

    this.filteredLocationList = this.housingLocationList.filter(housing => 
      housing?.city.toLocaleLowerCase().includes(text.toLocaleLowerCase()));
  }

}


/*
Copyright Google LLC. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at https://angular.io/license
*/