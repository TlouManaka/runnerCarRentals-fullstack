import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    // The app is authenticated and data-driven, so render everything in the browser.
    path: '**',
    renderMode: RenderMode.Client
  }
];
