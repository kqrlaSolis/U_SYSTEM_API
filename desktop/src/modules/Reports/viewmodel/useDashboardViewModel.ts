import { useCallback, useEffect, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { Dashboard } from '../model/Dashboard';
import { ReportsService } from '../model/ReportsService';

// ViewModel del dashboard: carga los indicadores al entrar.
export function useDashboardViewModel(Enabled = true) {
  const [Dashboard, SetDashboard] = useState<Dashboard | null>(null);
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    if (!Enabled) {
      SetIsLoading(false);
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      SetDashboard(await ReportsService.GetDashboard());
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [Enabled]);

  useEffect(() => {
    void Load();
  }, [Load]);

  return {
    Dashboard,
    IsLoading,
    ErrorMessage,
    Reload: Load,
  };
}
