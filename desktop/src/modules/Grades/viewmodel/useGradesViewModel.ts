import { useCallback, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import type { Grade, GradeItemRequest } from '../model/Grade';
import { GradesService } from '../model/GradesService';

// ViewModel de captura: carga el grupo y guarda la captura masiva en formato CSV.
export function useGradesViewModel() {
  const [ClassGroupId, SetClassGroupId] = useState('1');
  const [Items, SetItems] = useState<Grade[]>([]);
  const [Csv, SetCsv] = useState('');
  const [IsLoading, SetIsLoading] = useState(false);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);
  const [SavedMessage, SetSavedMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    const GroupId = Number(ClassGroupId);
    if (!Number.isFinite(GroupId) || GroupId <= 0) {
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);
    SetSavedMessage(null);

    try {
      const Grades = await GradesService.GetByClassGroup(GroupId);
      SetItems(Grades);
      SetCsv(
        Grades.map(
          (Grade) =>
            `${Grade.EnrollmentId};${Grade.Partial1 ?? ''};${Grade.Partial2 ?? ''};${Grade.Partial3 ?? ''};${Grade.Ordinary ?? ''};${Grade.FinalScore ?? ''};${Grade.Status}`,
        ).join('\n'),
      );
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [ClassGroupId]);

  const Save = useCallback(async () => {
    const GroupId = Number(ClassGroupId);
    if (!Number.isFinite(GroupId) || GroupId <= 0) {
      return;
    }

    const Grades: GradeItemRequest[] = Csv
      .split('\n')
      .map((Line) => Line.trim())
      .filter((Line) => Line.length > 0)
      .map((Line) => {
        const [EnrollmentId, P1, P2, P3, Ordinary, Final, Status] = Line.split(';').map((Cell) => Cell.trim());
        const ToNumber = (Value: string | undefined) => (Value === undefined || Value === '' ? null : Number(Value));
        return {
          EnrollmentId: Number(EnrollmentId),
          Partial1: ToNumber(P1),
          Partial2: ToNumber(P2),
          Partial3: ToNumber(P3),
          Ordinary: ToNumber(Ordinary),
          FinalScore: ToNumber(Final),
          Status: Status ?? 'InProgress',
        };
      })
      .filter((Item) => Number.isFinite(Item.EnrollmentId) && Item.EnrollmentId > 0);

    if (Grades.length === 0) {
      SetErrorMessage('Validation.Required');
      return;
    }

    SetIsLoading(true);
    SetErrorMessage(null);
    SetSavedMessage(null);

    try {
      await GradesService.Save(GroupId, Grades);
      SetSavedMessage('Saved');
      await Load();
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, [ClassGroupId, Csv, Load]);

  return {
    ClassGroupId,
    Items,
    Csv,
    IsLoading,
    ErrorMessage,
    SavedMessage,
    SetClassGroupId,
    SetCsv,
    Load,
    Save,
  };
}
