import { HttpClient } from '../../../shared/http/HttpClient';
import type { ChangePasswordRequest } from './ChangePasswordRequest';
import type { ForgotPasswordRequest, ResetPasswordRequest } from './PasswordRecovery';

const ChangePasswordPath = '/api/v1/auth/change-password';
const ForgotPasswordPath = '/api/v1/auth/forgot-password';
const ResetPasswordPath = '/api/v1/auth/reset-password';

// Cambio y recuperación de contraseña. La sesión (login, refresh, logout) está en shared/auth.
export const PasswordService = {
  async ChangePasswordAsync(Request: ChangePasswordRequest): Promise<void> {
    await HttpClient.Post<boolean>(ChangePasswordPath, Request);
  },

  // La API responde igual exista o no el correo (no revela cuentas)
  async ForgotPasswordAsync(Request: ForgotPasswordRequest): Promise<void> {
    await HttpClient.Post<boolean>(ForgotPasswordPath, Request);
  },

  async ResetPasswordAsync(Request: ResetPasswordRequest): Promise<void> {
    await HttpClient.Post<boolean>(ResetPasswordPath, Request);
  },
};
