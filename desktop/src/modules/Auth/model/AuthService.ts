import { HttpClient, RefreshTokenPath } from '../../../shared/http/HttpClient';
import type { AuthResponse, AuthUser, RefreshRequest } from './AuthResponse';
import type { ChangePasswordRequest } from './ChangePasswordRequest';
import type { ForgotPasswordRequest, ResetPasswordRequest } from './PasswordRecovery';
import type { LoginRequest } from './LoginRequest';

const LoginPath = '/api/v1/auth/login';
const LogoutPath = '/api/v1/auth/logout';
const MePath = '/api/v1/auth/me';
const ChangePasswordPath = '/api/v1/auth/change-password';
const ForgotPasswordPath = '/api/v1/auth/forgot-password';
const ResetPasswordPath = '/api/v1/auth/reset-password';

// Servicio del módulo: sin estado de UI y sin conocer React
export const AuthService = {
  async LoginAsync(Request: LoginRequest): Promise<AuthResponse> {
    return HttpClient.Post<AuthResponse>(LoginPath, Request);
  },

  async RefreshAsync(RefreshToken: string): Promise<AuthResponse> {
    const Request: RefreshRequest = { RefreshToken };
    return HttpClient.Post<AuthResponse>(RefreshTokenPath, Request);
  },

  async LogoutAsync(RefreshToken: string): Promise<void> {
    const Request: RefreshRequest = { RefreshToken };
    await HttpClient.Post<void>(LogoutPath, Request);
  },

  async GetMeAsync(): Promise<AuthUser> {
    return HttpClient.Get<AuthUser>(MePath);
  },

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
