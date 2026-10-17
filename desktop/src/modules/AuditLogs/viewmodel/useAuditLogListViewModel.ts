import { useCallback, useEffect, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { AuditLog, AuditLogFilters } from '../model/AuditLog';
import { AuditLogsService } from '../model/AuditLogsService';

const DefaultPageSize = 20;
const EmptyFilters: AuditLogFilters = { Module: '', Action: '', From: '', To: '' };

// ViewModel de la bitácora: filtros, paginación y detalle del registro.
export function useAuditLogListViewModel() {
  const [Items, SetItems] = useState<AuditLog[]>([]);
  const [TotalCount, SetTotalCount] = useState(0);
  const [Page, SetPage] = useState(1);
  const [Filters, SetFilters] = useState<AuditLogFilters>(EmptyFilters);
  const [AppliedFilters, SetAppliedFilters] = useState<AuditLogFilters>(EmptyFilters);
  const [Selected, SetSelected] = useState<AuditLog | null>(null);
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Result = await AuditLogsService.GetPaged(Page, DefaultPageSize, AppliedFilters);
      SetItems(Result.Items);
      SetTotalCount(Result.TotalCount);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [AppliedFilters, Page]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetFilter = useCallback((Field: keyof AuditLogFilters, Value: string) => {
    SetFilters((Current) => ({ ...Current, [Field]: Value }));
  }, []);

  const ApplyFilters = useCallback(() => {
    SetPage(1);
    SetAppliedFilters(Filters);
  }, [Filters]);

  const ClearFilters = useCallback(() => {
    SetFilters(EmptyFilters);
    SetPage(1);
    SetAppliedFilters(EmptyFilters);
  }, []);

  const SelectRow = useCallback(async (Row: AuditLog) => {
    try {
      const Detail = await AuditLogsService.GetById(Row.Id);
      SetSelected(Detail);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    }
  }, []);

  return {
    Items,
    TotalCount,
    Page,
    Filters,
    Selected,
    IsLoading,
    ErrorMessage,
    PageSize: DefaultPageSize,
    SetPage,
    SetFilter,
    ApplyFilters,
    ClearFilters,
    SelectRow,
    CloseDetail: () => SetSelected(null),
    Reload: Load,
  };
}
