import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.services';

export const authGuard: CanActivateFn = () => {
  const authservice = inject(AuthService);
  const router = inject(Router);

  if (authservice.isLoggedIn()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
