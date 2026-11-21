import { useTranslation } from 'react-i18next';
import type { NavigationIconName } from '../../../app/layout/NavigationModules';
import { usePermissions } from '../../../shared/auth/usePermissions';
import { NormalizeLanguage } from '../../../shared/i18n/Config';
import { FormatCurrency, FormatNumber } from '../../../shared/utils/Format';
import { BuildTermSummary, type TermSummary } from '../model/TermSummary';
import { useDashboardViewModel } from './useDashboardViewModel';

export interface DashboardKpi {
  Id: string;
  LabelKey: string;
  Value: string;
  Hint: string | null;
  Icon: NavigationIconName;
  Path: string | null;
}

// Alumnos activos inscritos en el periodo en curso
export interface DashboardCoverage {
  Percent: number;
  Enrolled: string;
  Active: string;
}

export interface DashboardFinance {
  Amount: string;
  Count: string;
  Average: string;
  Path: string | null;
}

export interface DashboardProcedures {
  Requested: number;
  RequestedText: string;
  Issued: string;
  Path: string | null;
}

// ViewModel de la página de dashboard: arma secciones y cifras derivadas a partir de los indicadores.
export function useDashboardPageViewModel(Today: Date = new Date()) {
  const Translation = useTranslation();
  const Permissions = usePermissions();
  const Language = NormalizeLanguage(Translation.i18n.language);
  const Source = useDashboardViewModel();
  const Data = Source.Dashboard;

  const PathIf = (Path: string, Permission: string): string | null =>
    Permissions.Can(Permission) ? Path : null;
  const FormatCount = (Value: number): string => FormatNumber(Value, Language);

  const Academic: DashboardKpi[] =
    Data === null
      ? []
      : [
          {
            Id: 'students',
            LabelKey: 'Dashboard.Students',
            Value: FormatCount(Data.ActiveStudents),
            Hint: null,
            Icon: 'Students',
            Path: PathIf('/students', 'Students.View'),
          },
          {
            Id: 'teachers',
            LabelKey: 'Dashboard.Teachers',
            Value: FormatCount(Data.ActiveTeachers),
            Hint:
              Data.ActiveTeachers > 0
                ? Translation.t('Dashboard.StudentsPerTeacher', {
                    Value: FormatNumber(Data.ActiveStudents / Data.ActiveTeachers, Language, {
                      maximumFractionDigits: 1,
                    }),
                  })
                : null,
            Icon: 'Teachers',
            Path: PathIf('/teachers', 'Teachers.View'),
          },
          {
            Id: 'groups',
            LabelKey: 'Dashboard.Groups',
            Value: FormatCount(Data.OpenClassGroups),
            Hint: null,
            Icon: 'ClassGroups',
            Path: PathIf('/class-groups', 'ClassGroups.View'),
          },
          {
            Id: 'enrollments',
            LabelKey: 'Dashboard.TermEnrollments',
            Value: FormatCount(Data.TermEnrollments),
            Hint: Translation.t('Dashboard.TotalEnrollmentsHint', { Count: Data.TotalEnrollments }),
            Icon: 'Enrollments',
            Path: PathIf('/enrollments', 'Enrollments.View'),
          },
        ];

  const Coverage: DashboardCoverage | null =
    Data === null || Data.CurrentTermId === null || Data.ActiveStudents === 0
      ? null
      : {
          Percent: Math.min(Math.round((Data.TermEnrollments / Data.ActiveStudents) * 100), 100),
          Enrolled: FormatCount(Data.TermEnrollments),
          Active: FormatCount(Data.ActiveStudents),
        };

  const Finance: DashboardFinance | null =
    Data === null
      ? null
      : {
          Amount: FormatCurrency(Data.PendingAmount, Language),
          Count: FormatCount(Data.PendingCharges),
          Average: Data.PendingCharges > 0 ? FormatCurrency(Data.PendingAmount / Data.PendingCharges, Language) : '—',
          Path: PathIf('/charges', 'Payments.View'),
        };

  const Procedures: DashboardProcedures | null =
    Data === null
      ? null
      : {
          Requested: Data.RequestedProcedures,
          RequestedText: FormatCount(Data.RequestedProcedures),
          Issued: FormatCount(Data.IssuedProcedures),
          Path: PathIf('/procedures', 'Procedures.View'),
        };

  const Term: TermSummary | null = BuildTermSummary(Data, Language, Today);

  return {
    IsLoading: Source.IsLoading && Data === null,
    IsRefreshing: Source.IsLoading && Data !== null,
    ErrorMessage: Data === null ? Source.ErrorMessage : null,
    RefreshError: Data !== null ? Source.ErrorMessage : null,
    Reload: Source.Reload,
    LastUpdated:
      Source.LastUpdatedAt === null
        ? null
        : new Intl.DateTimeFormat(Language === 'en' ? 'en-US' : 'es-MX', {
            hour: '2-digit',
            minute: '2-digit',
          }).format(Source.LastUpdatedAt),
    HasData: Data !== null,
    Term,
    HasNoTerm: Data !== null && Data.CurrentTermId === null,
    Academic,
    Coverage,
    Finance,
    Procedures,
  };
}
