import { HttpErrorResponse } from '@angular/common/http';

export function getErrorMessage(error: unknown, fallback = 'Something went wrong. Please try again.'): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as { detail?: unknown } | null;

    if (body && typeof body.detail === 'string' && body.detail.length > 0) {
      return body.detail;
    }

    if (error.status === 401) {
      return 'Incorrect email or password.';
    }

    if (error.status === 0) {
      return 'The API is not reachable. Make sure the back-end is running.';
    }
  }

  return fallback;
}
