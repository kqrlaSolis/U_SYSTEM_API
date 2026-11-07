import { useCallback, useEffect, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { Procedure } from '../model/ProcedureType';
import { ProceduresService } from '../model/ProceduresService';

// ViewModel de solicitudes: listado, alta y emisión de trámites.
export function useProcedureListViewModel() {
  const [Items, SetItems] = useState<Procedure[]>([]);
  const [ProcedureTypeId, SetProcedureTypeId] = useState('1');
  const [StudentId, SetStudentId] = useState('1');
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Page = await ProceduresService.GetProcedures(1, 100, '');
      SetItems(Page.Items);
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void Load();
  }, [Load]);

  const Create = useCallback(async () => {
    const TypeId = Number(ProcedureTypeId);
    const ParsedStudentId = Number(StudentId);

    if (!Number.isFinite(TypeId) || TypeId <= 0 || !Number.isFinite(ParsedStudentId) || ParsedStudentId <= 0) {
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      await ProceduresService.CreateProcedure(TypeId, ParsedStudentId);
      await Load();
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [Load, ProcedureTypeId, StudentId]);

  const Issue = useCallback(
    async (Id: number) => {
      SetIsLoading(true);
      SetErrorMessage(null);

      try {
        await ProceduresService.Issue(Id, null);
        await Load();
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      } finally {
        SetIsLoading(false);
      }
    },
    [Load],
  );

  return {
    Items,
    ProcedureTypeId,
    StudentId,
    IsLoading,
    ErrorMessage,
    SetProcedureTypeId,
    SetStudentId,
    Load,
    Create,
    Issue,
  };
}
