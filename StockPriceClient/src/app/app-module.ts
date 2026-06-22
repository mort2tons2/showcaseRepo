/* import { HttpClientModule } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';

@NgModule({
  declarations: [
    App
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
  ],
  bootstrap: [App]
})
export class AppModule { }
 */

/* import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

// Material Imports
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

// 1. IMPORT YOUR CUSTOM APP FILE AND CLASS
import { App } from './app';
//import { DashboardComponent } from './dashboard3/dashboard';
//import { Dashboard } from './dashboard/dashboard';

@NgModule({
  declarations: [
    App, // 2. Change AppComponent to App here
    // DashboardComponent,
    //Dashboard,
  ],
  imports: [BrowserModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  providers: [],
  bootstrap: [App], // 3. Change AppComponent to App here
})
export class AppModule {} */
