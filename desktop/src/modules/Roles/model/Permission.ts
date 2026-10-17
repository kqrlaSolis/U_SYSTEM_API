// Catálogo de permisos por módulo (GET /api/v1/permissions)
export interface Permission {
  Id: number;
  Code: string;
  Module: string;
  Description: string;
}

export interface PermissionModule {
  Module: string;
  Permissions: Permission[];
}

// Contrato de alta y edición de roles; el alta ignora IsActive
export interface RoleRequest {
  Name: string;
  Description: string | null;
  IsActive: boolean;
  PermissionCodes: string[];
}
