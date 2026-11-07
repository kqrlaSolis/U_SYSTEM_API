import { useCallback, useEffect, useState } from 'react';
import { usePermissions } from '../../../shared/hooks/usePermissions';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { StudyPlan } from '../model/StudyPlan';
import { StudyPlansService } from '../model/StudyPlansService';

const DefaultPageSize = 20;

// ViewModel del listado de planes: paginación, vigencia y baja.
export function useStudyPlanListViewModel() {
  const Permissions = usePermissions();
  const [Items, SetItems] = useState<StudyPlan[]>([]);
  const [TotalCount, SetTotalCount] = useState(0);
  const [Page, SetPage] = useState(1);
  const [Search, SetSearch] = useState('');
  const [Filters, SetFilters] = useState<Record<string, string>>({});
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [BusyId, SetBusyId] = useState<number | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Result = await StudyPlansService.GetPaged(Page, DefaultPageSize, Search, Filters);
      SetItems(Result.Items);
      SetTotalCount(Result.TotalCount);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [Filters, Page, Search]);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetCurrent = useCallback(
    async (Row: StudyPlan) => {
      SetBusyId(Row.Id);
      SetErrorMessage(null);

      try {
        await StudyPlansService.SetCurrent(Row.Id);
        await Load();
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      } finally {
        SetBusyId(null);
      }
    },
    [Load],
  );

  const Remove = useCallback(
    async (Row: StudyPlan) => {
      SetBusyId(Row.Id);
      SetErrorMessage(null);

      try {
        await StudyPlansService.Delete(Row.Id);
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
    IsLoading,
    ErrorMessage,
    BusyId,
    PageSize: DefaultPageSize,
    CanCreate: Permissions.Can('StudyPlans.Create'),
    CanEdit: Permissions.Can('StudyPlans.Edit'),
    CanDelete: Permissions.Can('StudyPlans.Delete'),
    Filters,
    SetPage,
    SetSearch,
    SetFilter: (Field: string, Value: string) => {
      SetFilters((Current) => ({ ...Current, [Field]: Value }));
      SetPage(1);
    },
    ClearFilters: () => {
      SetFilters({});
      SetPage(1);
    },
    SetCurrent,
    Remove,
    Reload: Load,
  };
}
