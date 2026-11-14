import { FormatDate } from '../../../shared/utils/Format';
import type { Dashboard } from './Dashboard';
import { CalculateTermProgress, ParseLocalDate, type TermProgress } from './TermProgress';

// Periodo en curso listo para mostrar: textos formateados y avance calculado.
export interface TermSummary {
  Code: string;
  Name: string;
  StatusKey: string;
  StartDate: string;
  EndDate: string;
  Progress: TermProgress | null;
}

function FormatTermDate(Value: string | null, Language: string): string {
  const Parsed = ParseLocalDate(Value);
  return Parsed === null ? '' : FormatDate(Parsed, Language);
}

export function BuildTermSummary(Data: Dashboard | null, Language: string, Today: Date): TermSummary | null {
  if (Data === null || Data.CurrentTermId === null) {
    return null;
  }

  return {
    Code: Data.CurrentTermCode ?? '',
    Name: Data.CurrentTermName ?? '',
    StatusKey: `AcademicTerms.Status.${Data.CurrentTermStatus ?? 'Open'}`,
    StartDate: FormatTermDate(Data.CurrentTermStartDate, Language),
    EndDate: FormatTermDate(Data.CurrentTermEndDate, Language),
    Progress: CalculateTermProgress(Data.CurrentTermStartDate, Data.CurrentTermEndDate, Today),
  };
}
