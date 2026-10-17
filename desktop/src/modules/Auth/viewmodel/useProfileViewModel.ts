import { useCallback, useEffect, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { useAuth } from '../../../shared/auth/AuthContext';

// ViewModel del perfil propio: lee /auth/me y expone roles y permisos frescos.
export function useProfileViewModel() {
  const Auth = useAuth();
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      await Auth.ReloadCurrentUser();
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [Auth]);

  useEffect(() => {
    void Load();
  }, [Load]);

  return {
    User: Auth.CurrentUser,
    IsLoading,
    ErrorMessage,
    Reload: Load,
  };
}
