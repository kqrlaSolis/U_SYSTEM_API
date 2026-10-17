import { useAuth } from '../auth/AuthContext';

export interface PermissionsApi {
  Permissions: string[];
  Can: (Permission: string) => boolean;
  CanAny: (PermissionList: string[]) => boolean;
}

// El permiso se evalúa en la UI, pero la API siempre es la autoridad final.
export function usePermissions(): PermissionsApi {
  const Auth = useAuth();
  const UserPermissions = Auth.CurrentUser?.Permissions ?? [];

  const Can = (Permission: string): boolean => UserPermissions.includes(Permission);

  const CanAny = (PermissionList: string[]): boolean =>
    PermissionList.some((Permission) => UserPermissions.includes(Permission));

  return {
    Permissions: UserPermissions,
    Can,
    CanAny,
  };
}
