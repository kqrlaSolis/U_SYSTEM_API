// Rol del sistema con sus conteos (contrato de /api/v1/roles)
export interface Role {
  Id: number;
  Name: string;
  Description: string | null;
  IsSystem: boolean;
  IsActive: boolean;
  UserCount: number;
  PermissionCount: number;
  PermissionCodes: string[];
}
