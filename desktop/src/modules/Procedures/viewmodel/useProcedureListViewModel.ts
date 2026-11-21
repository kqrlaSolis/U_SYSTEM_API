import { useCallback, useState } from 'react';
import { usePermissions } from '../../../shared/auth/usePermissions';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { DefaultPageSize, usePagedQuery } from '../../../shared/http/usePagedQuery';
import type { Procedure } from '../model/ProcedureType';
import { ProceduresService } from '../model/ProceduresService';

// ViewModel de solicitudes: listado, alta y emisión de trámites.
export function useProcedureListViewModel() {
  const Permissions = usePermissions();
  // Listado paginado en el servidor; las acciones (alta, cancelar/emitir) recargan la página actual
  const Paged = usePagedQuery<Procedure>({
    FetchPage: ProceduresService.GetProcedures,
    PageSize: DefaultPageSize,
  });
  const [ProcedureTypeId, SetProcedureTypeId] = useState('1');
  const [StudentId, SetStudentId] = useState('1');
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  // Reintentar limpia también el error de la última acción
  const ReloadPage = Paged.Reload;
  const Load = useCallback(() => {
    SetErrorMessage(null);
    ReloadPage();
  }, [ReloadPage]);

  const Create = useCallback(async () => {
    const TypeId = Number(ProcedureTypeId);
    const ParsedStudentId = Number(StudentId);

    if (!Number.isFinite(TypeId) || TypeId <= 0 || !Number.isFinite(ParsedStudentId) || ParsedStudentId <= 0) {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      await ProceduresService.CreateProcedure(TypeId, ParsedStudentId);
      Load();
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [Load, ProcedureTypeId, StudentId]);

  const Issue = useCallback(
    async (Id: number) => {
      SetIsSubmitting(true);
      SetErrorMessage(null);

      try {
        await ProceduresService.Issue(Id, null);
        Load();
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      } finally {
        SetIsSubmitting(false);
      }
    },
    [Load],
  );

  return {
    CanRequest: Permissions.Can('Procedures.Request'),
    CanIssue: Permissions.Can('Procedures.Issue'),
    Items: Paged.Items,
    TotalCount: Paged.TotalCount,
    Page: Paged.Page,
    PageSize: Paged.PageSize,
    SetPage: Paged.SetPage,
    SetPageSize: Paged.SetPageSize,
    ProcedureTypeId,
    StudentId,
    IsLoading: Paged.IsLoading || IsSubmitting,
    ErrorMessage: ErrorMessage ?? Paged.ErrorMessage,
    SetProcedureTypeId,
    SetStudentId,
    Load,
    Create,
    Issue,
  };
}
