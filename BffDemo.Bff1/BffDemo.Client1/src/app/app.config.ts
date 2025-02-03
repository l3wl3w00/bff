import {ApplicationConfig, importProvidersFrom, provideZoneChangeDetection} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import {AuthModule, LogLevel} from 'angular-auth-oidc-client';
import { APP_INITIALIZER } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import {provideHttpClient, withFetch} from '@angular/common/http';

function initAuthFactory(oidcSecurityService: OidcSecurityService) {
  return () => {
    // This call will load the configuration (and perform a silent check if configured)
    return oidcSecurityService.checkAuth().toPromise();
  };
}

export function createAppConfig(redirectUrl: string): ApplicationConfig {
  return {
    providers: [
      provideZoneChangeDetection({ eventCoalescing: true }),
      provideRouter(routes),
      provideClientHydration(),
      provideHttpClient(withFetch()),
      importProvidersFrom(
        AuthModule.forRoot({
          config: {
            // URL of your IdentityServer
            authority: 'https://localhost:5000',
            // The URL the user is returned to after login
            redirectUrl: redirectUrl,
            // Client ID as registered in your IdentityServer
            clientId: 'client1',
            // The scopes you want to request
            scope: 'openid profile client1',
            // Use the authorization code flow with PKCE
            responseType: 'code',
            // The URL the user is returned to after logout
            postLogoutRedirectUri: redirectUrl,
            // Enable silent renew if desired (make sure to add a silent renew HTML file if needed)
            silentRenew: false,
            // Optional: set the log level for debugging
            logLevel: LogLevel.Debug,
          },
        })
      ),
      {
        provide: APP_INITIALIZER,
        useFactory: initAuthFactory,
        deps: [OidcSecurityService],
        multi: true,
      },
    ]
  };
}
