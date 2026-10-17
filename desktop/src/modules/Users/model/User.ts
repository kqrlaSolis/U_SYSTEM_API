// Contrato que devuelve la API (PascalCase)
export interface User {
  Id: number;
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string | null;
  FullName: string;
  Email: string;
  Username: string;
  Phone: string | null;
  IsActive: boolean;
  MustChangePassword: boolean;
  CreatedAt: string;
  Roles: string[];
  Permissions: string[];
}
