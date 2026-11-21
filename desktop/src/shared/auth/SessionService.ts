import { HttpClient, RefreshTokenPath } from '../http/HttpClient';
import type { AuthResponse, AuthUser, RefreshRequest } from './AuthResponse';
import type { LoginRequest } from './LoginRequest';

const LoginPath = '/api/v1/auth/login';
const LogoutPath = '/api/v1/auth/logout';
const MePath = '/api/v1/auth/me';

// Llamadas de la sesión (iniciar, renovar, cerrar y leer el usuario actual). Las usa AuthContext.
// Cambio y recuperación de contraseña viven en modules/Auth/model/PasswordService.ts.
export const SessionService = {
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
};
