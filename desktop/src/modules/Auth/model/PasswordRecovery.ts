// Contratos de recuperación de contraseña
export interface ForgotPasswordRequest {
  Email: string;
}

export interface ResetPasswordRequest {
  Email: string;
  Token: string;
  NewPassword: string;
}
