import { useCallback, useEffect, useState } from 'react';
import { usePermissions } from '../../../shared/hooks/usePermissions';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { AcademicTerm } from '../model/AcademicTerm';
import { AcademicTermsService } from '../model/AcademicTermsService';

const DefaultPageSize = 20;

// ViewModel del calendario: listado, periodo en curso y apertura/cierre.
export function useAcademicTermListViewModel() {
  const Permissions = usePermissions();
  const [Items, SetItems] = useState<AcademicTerm[]>([]);
  const [TotalCount, SetTotalCount] = useState(0);
  const [Page, SetPage] = useState(1);
  const [Search, SetSearch] = useState('');
  const [Current, SetCurrent] = useState<AcademicTerm | null>(null);
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [BusyId, SetBusyId] = useState<number | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const [Result, CurrentTerm] = await Promise.all([
        AcademicTermsService.GetPaged(Page, DefaultPageSize, Search),
        AcademicTermsService.GetCurrent(),
      ]);
      SetItems(Result.Items);
      SetTotalCount(Result.TotalCount);
      SetCurrent(CurrentTerm);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [Page, Search]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const Run = useCallback(
    async (Row: AcademicTerm, Action: 'Open' | 'Close') => {
      SetBusyId(Row.Id);
      SetErrorMessage(null);

      try {
        if (Action === 'Open') {
          await AcademicTermsService.Open(Row.Id);
        } else {
          await AcademicTermsService.Close(Row.Id);
        }

        await Load();
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      } finally {
        SetBusyId(null);
      }
    },
    [Load],
  );

  return {
    Items,
    TotalCount,
    Page,
    Search,
    Current,
    IsLoading,
    ErrorMessage,
    BusyId,
    PageSize: DefaultPageSize,
    CanCreate: Permissions.Can('AcademicTerms.Create'),
    CanEdit: Permissions.Can('AcademicTerms.Edit'),
    CanOpen: Permissions.Can('AcademicTerms.Open'),
    CanClose: Permissions.Can('AcademicTerms.Close'),
    SetPage,
    SetSearch,
    Open: (Row: AcademicTerm) => Run(Row, 'Open'),
    Close: (Row: AcademicTerm) => Run(Row, 'Close'),
    Reload: Load,
  };
}
