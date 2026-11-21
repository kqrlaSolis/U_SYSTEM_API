import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { DefaultPageSize, usePagedQuery } from '../../../shared/http/usePagedQuery';
import type { Charge } from '../model/PaymentConcept';
import { PaymentsService } from '../model/PaymentsService';

// ViewModel de cargos: listado, alta y cancelación.
export function useChargeListViewModel() {
  // Listado paginado en el servidor; las acciones (alta, cancelar/emitir) recargan la página actual
  const Paged = usePagedQuery<Charge>({
    FetchPage: PaymentsService.GetCharges,
    PageSize: DefaultPageSize,
  });
  const [StudentId, SetStudentId] = useState('1');
  const [AcademicTermId, SetAcademicTermId] = useState('1');
  const [ConceptId, SetConceptId] = useState('');
  const [Amount, SetAmount] = useState('0');
  const [Discount, SetDiscount] = useState('0');
  const [Surcharge, SetSurcharge] = useState('0');
  const [DueDate, SetDueDate] = useState('');
  const [IsSubmitting, SetIsSubmitting] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  // Reintentar limpia también el error de la última acción
  const ReloadPage = Paged.Reload;
  const Load = useCallback(() => {
    SetErrorMessage(null);
    ReloadPage();
  }, [ReloadPage]);

  const Create = useCallback(async () => {
    const ParsedConceptId = Number(ConceptId);

    if (!Number.isFinite(ParsedConceptId) || ParsedConceptId <= 0 || DueDate.trim() === '') {
      return;
    }

    SetIsSubmitting(true);
    SetErrorMessage(null);

    try {
      await PaymentsService.CreateCharge({
        StudentId: Number(StudentId),
        AcademicTermId: Number(AcademicTermId),
        PaymentConceptId: ParsedConceptId,
        Amount: Number(Amount === '' ? '0' : Amount),
        Discount: Number(Discount === '' ? '0' : Discount),
        Surcharge: Number(Surcharge === '' ? '0' : Surcharge),
        DueDate,
      });
      Load();
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsSubmitting(false);
    }
  }, [AcademicTermId, Amount, ConceptId, Discount, DueDate, StudentId, Surcharge, Load]);

  const Cancel = useCallback(
    async (Id: number) => {
      SetIsSubmitting(true);
      SetErrorMessage(null);

      try {
        await PaymentsService.CancelCharge(Id);
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
    Items: Paged.Items,
    TotalCount: Paged.TotalCount,
    Page: Paged.Page,
    PageSize: Paged.PageSize,
    SetPage: Paged.SetPage,
    SetPageSize: Paged.SetPageSize,
    StudentId,
    AcademicTermId,
    ConceptId,
    Amount,
    Discount,
    Surcharge,
    DueDate,
    IsLoading: Paged.IsLoading || IsSubmitting,
    ErrorMessage: ErrorMessage ?? Paged.ErrorMessage,
    SetStudentId,
    SetAcademicTermId,
    SetConceptId,
    SetAmount,
    SetDiscount,
    SetSurcharge,
    SetDueDate,
    Load,
    Create,
    Cancel,
  };
}
