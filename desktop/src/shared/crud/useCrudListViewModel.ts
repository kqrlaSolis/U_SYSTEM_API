import { useCallback, useEffect, useState } from 'react';
import { GetErrorMessage } from '../http/GetErrorMessage';
import { usePermissions } from '../hooks/usePermissions';
import type { CrudConfig, CrudRow } from './CrudTypes';

const DefaultPageSize = 20;

// ViewModel genérico de listado: paginación, búsqueda, permisos y baja.
export function useCrudListViewModel<TRow extends CrudRow, TRequest>(
  Config: CrudConfig<TRow, TRequest>,
) {
  const Permissions = usePermissions();
  const [Items, SetItems] = useState<TRow[]>([]);
  const [TotalCount, SetTotalCount] = useState(0);
  const [Page, SetPage] = useState(1);
  const [Search, SetSearch] = useState('');
  const [Filters, SetFilters] = useState<Record<string, string>>({});
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Result = await Config.List(Page, DefaultPageSize, Search, Filters);
      SetItems(Result.Items);
      SetTotalCount(Result.TotalCount);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [Config, Filters, Page, Search]);

  useEffect(() => {
    void Load();
  }, [Load]);

  // Cambiar un filtro siempre vuelve a la primera página
  const SetFilter = useCallback((Field: string, Value: string) => {
    SetFilters((Current) => ({ ...Current, [Field]: Value }));
    SetPage(1);
  }, []);

  const ClearFilters = useCallback(() => {
    SetFilters({});
    SetPage(1);
  }, []);

  const Remove = useCallback(
    async (Row: TRow) => {
      if (Config.Remove === undefined) {
        return;
      }

      try {
        await Config.Remove(Row.Id);
        await Load();
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      }
    },
    [Config, Load],
  );

  return {
    Items,
    TotalCount,
    Page,
    Search,
    IsLoading,
    ErrorMessage,
    PageSize: DefaultPageSize,
    CanCreate: Permissions.Can(Config.PermissionCreate),
    CanEdit: Permissions.Can(Config.PermissionEdit),
    CanDelete:
      Config.PermissionDelete !== undefined && Permissions.Can(Config.PermissionDelete) && Config.Remove !== undefined,
    Filters,
    SetPage,
    SetSearch,
    SetFilter,
    ClearFilters,
    Reload: Load,
    Remove,
  };
}
