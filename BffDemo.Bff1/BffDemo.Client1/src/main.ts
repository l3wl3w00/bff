import { bootstrapApplication } from '@angular/platform-browser';
import { createAppConfig} from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, createAppConfig("http://localhost:4200"))
  .catch((err) => console.error(err));
