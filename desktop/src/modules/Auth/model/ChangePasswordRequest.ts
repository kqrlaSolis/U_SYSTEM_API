// Contrato de POST /api/v1/auth/change-password
export interface ChangePasswordRequest {
  CurrentPassword: string;
  NewPassword: string;
}
