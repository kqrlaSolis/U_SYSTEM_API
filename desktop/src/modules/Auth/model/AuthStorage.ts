// Almacenamiento de tokens del cliente.
// El access token vive SOLO en memoria: al cerrar la aplicación desaparece.
const RefreshTokenStorageKey = 'USystem.RefreshToken';

let AccessToken: string | null = null;

function ReadRefreshToken(): string | null {
  try {
    return window.localStorage.getItem(RefreshTokenStorageKey);
  } catch {
    return null;
  }
}

export const AuthStorage = {
  GetAccessToken(): string | null {
    return AccessToken;
  },

  SetAccessToken(Token: string): void {
    AccessToken = Token;
  },

  ClearAccessToken(): void {
    AccessToken = null;
  },

  // Fase 0: el refresh token se guarda en localStorage porque todavía no existe
  // almacenamiento seguro en el preload. TODO(Fase 1): moverlo a safeStorage de
  // Electron expuesto por window.USystem y borrar la copia del renderer.
  GetRefreshToken(): string | null {
    const StoredToken = ReadRefreshToken();
    return StoredToken !== null && StoredToken.length > 0 ? StoredToken : null;
  },

  SetRefreshToken(Token: string): void {
    try {
      window.localStorage.setItem(RefreshTokenStorageKey, Token);
    } catch {
      // Sin almacenamiento disponible la sesión dura lo que dure la ventana
    }
  },

  Clear(): void {
    AccessToken = null;
    try {
      window.localStorage.removeItem(RefreshTokenStorageKey);
    } catch {
      // Nada que limpiar si el almacenamiento no está disponible
    }
  },
};
