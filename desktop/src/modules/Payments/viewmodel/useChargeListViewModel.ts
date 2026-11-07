import { useCallback, useEffect, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { Charge } from '../model/PaymentConcept';
import { PaymentsService } from '../model/PaymentsService';

// ViewModel de cargos: listado, alta y cancelación.
export function useChargeListViewModel() {
  const [Items, SetItems] = useState<Charge[]>([]);
  const [StudentId, SetStudentId] = useState('1');
  const [AcademicTermId, SetAcademicTermId] = useState('1');
  const [ConceptId, SetConceptId] = useState('');
  const [Amount, SetAmount] = useState('0');
  const [Discount, SetDiscount] = useState('0');
  const [Surcharge, SetSurcharge] = useState('0');
  const [DueDate, SetDueDate] = useState('');
  const [IsLoading, SetIsLoading] = useState(true);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Page = await PaymentsService.GetCharges(1, 100, '');
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
    const ParsedConceptId = Number(ConceptId);

    if (!Number.isFinite(ParsedConceptId) || ParsedConceptId <= 0 || DueDate.trim() === '') {
      return;
    }

    SetIsLoading(true);
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
      await Load();
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [AcademicTermId, Amount, ConceptId, Discount, DueDate, StudentId, Surcharge, Load]);

  const Cancel = useCallback(
    async (Id: number) => {
      SetIsLoading(true);
      SetErrorMessage(null);

      try {
        await PaymentsService.CancelCharge(Id);
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
    StudentId,
    AcademicTermId,
    ConceptId,
    Amount,
    Discount,
    Surcharge,
    DueDate,
    IsLoading,
    ErrorMessage,
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
