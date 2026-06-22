/* import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app-module';

platformBrowser()
  .bootstrapModule(AppModule, {})
  .catch((err) => console.error(err));
 */

import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app'; // Adjust path to your App component if needed

bootstrapApplication(App, {
  providers: [], // Global services/routing go here if needed later
}).catch((err) => console.error(err));

/* import {bootstrapApplication} from '@angular/platform-browser';
import {VERSION as CDK_VERSION} from '@angular/cdk';
import {VERSION as MAT_VERSION} from '@angular/material/core';
//import {CardMediaSizeExample} from './example/card-media-size-example';

console.info('Angular CDK version', CDK_VERSION.full);
console.info('Angular Material version', MAT_VERSION.full);

bootstrapApplication(CardMediaSizeExample).catch(err => console.error(err)) */
