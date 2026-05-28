import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.services';

export const customerGuard: CanActivateFn = () => {
  const authservice = inject(AuthService);
  const router = inject(Router);
  const role = authservice.getRole();

  if (role === 'Customer') {
    return true;
  }

  router.navigate(['/']);
  return false;
};
