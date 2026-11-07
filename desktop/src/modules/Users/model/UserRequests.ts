// Contratos de escritura de /api/v1/users
export interface CreateUserRequest {
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string | null;
  Email: string;
  Username: string;
  Phone: string | null;
  TemporaryPassword: string | null;
  RoleIds: number[];
}

export interface UpdateUserRequest {
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string | null;
  Email: string;
  Username: string;
  Phone: string | null;
}

export interface CreatedUserResponse {
  User: {
    Id: number;
    Username: string;
    FullName: string;
    Roles: string[];
  };
  TemporaryPassword: string | null;
}

export interface TemporaryPasswordResponse {
  TemporaryPassword: string;
}
