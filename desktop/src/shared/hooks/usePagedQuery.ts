import { useCallback, useEffect, useRef, useState } from 'react';
import I18next from '../i18n/Config';
import { ApiErrorException } from '../http/ApiErrorException';
import type { PagedResult } from '../http/ApiResponse';

export const DefaultPageSize = 20;

export interface PagedQueryOptions<TRow> {
  FetchPage: (Page: number, PageSize: number, Search: string) => Promise<PagedResult<TRow>>;
  PageSize?: number;
  IsEnabled?: boolean;
}

export interface PagedQueryResult<TRow> {
  Items: TRow[];
  TotalCount: number;
  Page: number;
  PageSize: number;
  Search: string;
  IsLoading: boolean;
  ErrorMessage: string | null;
  SetPage: (Page: number) => void;
  SetPageSize: (PageSize: number) => void;
  SetSearch: (Search: string) => void;
  Reload: () => void;
}

function ResolveErrorMessage(CaughtError: unknown): string {
  if (CaughtError instanceof ApiErrorException) {
    return CaughtError.Message;
  }

  if (CaughtError instanceof Error && CaughtError.message.length > 0) {
    return CaughtError.message;
  }

  return I18next.t('Errors.Unknown');
}

// Estado de paginación reutilizable: página, búsqueda, carga y errores.
export function usePagedQuery<TRow>(Options: PagedQueryOptions<TRow>): PagedQueryResult<TRow> {
  const InitialPageSize = Options.PageSize ?? DefaultPageSize;
  const IsEnabled = Options.IsEnabled ?? true;

  const [Page, SetPageNumber] = useState(1);
  const [PageSize, SetPageSizeNumber] = useState(InitialPageSize);
  const [Search, SetSearchValue] = useState('');
  const [Items, SetItems] = useState<TRow[]>([]);
  const [TotalCount, SetTotalCount] = useState(0);
  const [IsLoading, SetIsLoading] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [ReloadToken, SetReloadToken] = useState(0);

  // La función de carga puede cambiar de identidad en cada render del ViewModel
  const FetchPageReference = useRef(Options.FetchPage);
  FetchPageReference.current = Options.FetchPage;

  // Descarta respuestas viejas cuando el usuario cambia de página o de búsqueda
  const RequestReference = useRef(0);

  const Load = useCallback(async () => {
    if (!IsEnabled) {
      SetItems([]);
      SetTotalCount(0);
      SetIsLoading(false);
      return;
    }

    const CurrentRequest = RequestReference.current + 1;
    RequestReference.current = CurrentRequest;
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Result = await FetchPageReference.current(Page, PageSize, Search);

      if (RequestReference.current !== CurrentRequest) {
        return;
      }

      SetItems(Result.Items ?? []);
      SetTotalCount(Result.TotalCount ?? 0);
    } catch (CaughtError) {
      if (RequestReference.current === CurrentRequest) {
        SetErrorMessage(ResolveErrorMessage(CaughtError));
      }
    } finally {
      if (RequestReference.current === CurrentRequest) {
        SetIsLoading(false);
      }
    }
  }, [Page, PageSize, Search, IsEnabled, ReloadToken]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetPage = useCallback((NextPage: number) => {
    SetPageNumber(Math.max(1, NextPage));
  }, []);

  const SetPageSize = useCallback((NextPageSize: number) => {
    SetPageSizeNumber(Math.max(1, NextPageSize));
    SetPageNumber(1);
  }, []);

  const SetSearch = useCallback((NextSearch: string) => {
    SetSearchValue(NextSearch);
    SetPageNumber(1);
  }, []);

  const Reload = useCallback(() => {
    SetReloadToken((CurrentToken) => CurrentToken + 1);
  }, []);

  return {
    Items,
    TotalCount,
    Page,
    PageSize,
    Search,
    IsLoading,
    ErrorMessage,
    SetPage,
    SetPageSize,
    SetSearch,
    Reload,
  };
}
