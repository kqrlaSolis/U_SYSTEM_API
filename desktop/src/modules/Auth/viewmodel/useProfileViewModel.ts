import { useCallback, useEffect, useRef, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { useAuth } from '../../../shared/auth/AuthContext';

// ViewModel del perfil propio: lee /auth/me una sola vez y expone roles y permisos frescos.
export function useProfileViewModel() {
  const Auth = useAuth();
  const { ReloadCurrentUser } = Auth;
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const HasLoaded = useRef(false);

  // Depende solo de ReloadCurrentUser (memoizado): depender del objeto Auth completo
  // volvía a disparar la carga cada vez que el contexto actualizaba el usuario (bucle infinito).
  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      await ReloadCurrentUser();
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [ReloadCurrentUser]);

  useEffect(() => {
    // StrictMode monta dos veces: solo la primera carga debe consultar la API
    if (HasLoaded.current) {
      return;
    }

    HasLoaded.current = true;
    void Load();
  }, [Load]);

  return {
    User: Auth.CurrentUser,
    IsLoading,
    ErrorMessage,
    Reload: Load,
  };
}
