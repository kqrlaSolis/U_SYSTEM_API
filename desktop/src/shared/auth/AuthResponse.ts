// Usuario autenticado tal como lo devuelve la API (UserDto)
export interface AuthUser {
  Id: number;
  Username: string;
  FullName: string;
  Email: string;
  Roles: string[];
  Permissions: string[];
}

// Contrato de POST /api/v1/auth/login y /api/v1/auth/refresh
export interface AuthResponse {
  AccessToken: string;
  RefreshToken: string;
  AccessTokenExpiresAt: string;
  RefreshTokenExpiresAt: string;
  MustChangePassword: boolean;
  User: AuthUser;
}

export interface RefreshRequest {
  RefreshToken: string;
}
