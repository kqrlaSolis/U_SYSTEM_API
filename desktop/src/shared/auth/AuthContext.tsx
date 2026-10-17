import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import type { AuthResponse, AuthUser } from '../../modules/Auth/model/AuthResponse';
import { AuthService } from '../../modules/Auth/model/AuthService';
import { AuthStorage } from '../../modules/Auth/model/AuthStorage';
import type { LoginRequest } from '../../modules/Auth/model/LoginRequest';
import { HttpClient } from '../http/HttpClient';

export interface AuthContextValue {
  CurrentUser: AuthUser | null;
  IsAuthenticated: boolean;
  IsLoading: boolean;
  MustChangePassword: boolean;
  SignIn: (Request: LoginRequest) => Promise<void>;
  SignOut: () => Promise<void>;
  MarkPasswordChanged: () => void;
  ReloadCurrentUser: () => Promise<void>;
}

export interface AuthProviderProps {
  children: ReactNode;
}

// Valor por defecto: sin proveedor no hay sesión. Así usePermissions y useAuth
// se pueden usar en pruebas sin montar toda la aplicación.
const EmptyAuthValue: AuthContextValue = {
  CurrentUser: null,
  IsAuthenticated: false,
  IsLoading: false,
  MustChangePassword: false,
  SignIn: async () => {
    throw new Error('AuthProviderIsMissing');
  },
  SignOut: async () => undefined,
  MarkPasswordChanged: () => undefined,
  ReloadCurrentUser: async () => undefined,
};

const AuthContext = createContext<AuthContextValue>(EmptyAuthValue);

export function AuthProvider({ children }: AuthProviderProps) {
  const [CurrentUser, SetCurrentUser] = useState<AuthUser | null>(null);
  const [MustChangePassword, SetMustChangePassword] = useState(false);
  const [IsLoading, SetIsLoading] = useState(true);

  const ClearSession = useCallback(() => {
    AuthStorage.Clear();
    SetCurrentUser(null);
    SetMustChangePassword(false);
  }, []);

  const ApplySession = useCallback((Response: AuthResponse) => {
    AuthStorage.SetAccessToken(Response.AccessToken);
    AuthStorage.SetRefreshToken(Response.RefreshToken);
    SetCurrentUser(Response.User);
    SetMustChangePassword(Response.MustChangePassword);
  }, []);

  // HttpClient consulta el token en cada petición y pide el refresh en un 401
  useEffect(() => {
    HttpClient.ConfigureAuth({
      GetAccessToken: () => AuthStorage.GetAccessToken(),
      RefreshAccessToken: async () => {
        const StoredRefreshToken = AuthStorage.GetRefreshToken();

        if (StoredRefreshToken === null) {
          return null;
        }

        try {
          const Refreshed: AuthResponse = await AuthService.RefreshAsync(StoredRefreshToken);
          AuthStorage.SetAccessToken(Refreshed.AccessToken);
          AuthStorage.SetRefreshToken(Refreshed.RefreshToken);
          SetCurrentUser(Refreshed.User);
          return Refreshed.AccessToken;
        } catch {
          ClearSession();
          return null;
        }
      },
    });

    return () => {
      HttpClient.ConfigureAuth(null);
    };
  }, [ClearSession]);

  // Al arrancar se intenta restaurar la sesión con el refresh token guardado
  useEffect(() => {
    let IsMounted = true;

    const RestoreSession = async () => {
      const StoredRefreshToken = AuthStorage.GetRefreshToken();

      if (StoredRefreshToken === null) {
        SetIsLoading(false);
        return;
      }

      try {
        const Refreshed: AuthResponse = await AuthService.RefreshAsync(StoredRefreshToken);

        if (!IsMounted) {
          return;
        }

        ApplySession(Refreshed);
      } catch {
        if (IsMounted) {
          ClearSession();
        }
      } finally {
        if (IsMounted) {
          SetIsLoading(false);
        }
      }
    };

    void RestoreSession();

    return () => {
      IsMounted = false;
    };
  }, [ApplySession, ClearSession]);

  // El cambio de contraseña limpia el aviso de primer ingreso
  const MarkPasswordChanged = useCallback(() => {
    SetMustChangePassword(false);
  }, []);

  // Vuelve a leer el perfil de la API (roles y permisos frescos)
  const ReloadCurrentUser = useCallback(async () => {
    const Profile = await AuthService.GetMeAsync();
    SetCurrentUser(Profile);
  }, []);

  const SignIn = useCallback(
    async (Request: LoginRequest) => {
      const Response: AuthResponse = await AuthService.LoginAsync(Request);
      ApplySession(Response);
    },
    [ApplySession],
  );

  const SignOut = useCallback(async () => {
    const StoredRefreshToken = AuthStorage.GetRefreshToken();

    try {
      if (StoredRefreshToken !== null) {
        await AuthService.LogoutAsync(StoredRefreshToken);
      }
    } catch {
      // Aunque la API falle, la sesión local se cierra
    }

    ClearSession();
  }, [ClearSession]);

  const Value = useMemo<AuthContextValue>(
    () => ({
      CurrentUser,
      IsAuthenticated: CurrentUser !== null,
      IsLoading,
      MustChangePassword,
      SignIn,
      SignOut,
      MarkPasswordChanged,
      ReloadCurrentUser,
    }),
    [CurrentUser, IsLoading, MustChangePassword, SignIn, SignOut, MarkPasswordChanged, ReloadCurrentUser],
  );

  return <AuthContext.Provider value={Value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  return useContext(AuthContext);
}
