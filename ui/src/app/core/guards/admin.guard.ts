import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.services';

export const adminGuard: CanActivateFn = () => {
  const authservice = inject(AuthService);
  const router = inject(Router);
  const role = authservice.getRole();

  if (role === 'Admin') {
    return true;
  }

  router.navigate(['/']);
  return false;
};
