// Contrato de POST /api/v1/auth/login (PascalCase, igual que la API).
// Acepta el nombre de usuario o el correo.
export interface LoginRequest {
  UsernameOrEmail: string;
  Password: string;
}
