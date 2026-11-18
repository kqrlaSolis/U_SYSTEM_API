import { useTranslation } from 'react-i18next';
import {
  GetNavigationModules,
  type NavigationIconName,
  type NavigationModule,
} from '../../../app/layout/NavigationModules';
import { useAuth } from '../../../shared/auth/AuthContext';
import { usePermissions } from '../../../shared/hooks/usePermissions';
import { NormalizeLanguage } from '../../../shared/i18n/Config';
import { FormatCurrency, FormatNumber } from '../../../shared/utils/Format';
import { useDashboardViewModel } from '../../Reports/viewmodel/useDashboardViewModel';
import { BuildTermSummary, type TermSummary } from '../../Reports/model/TermSummary';

export interface HomeIndicator {
  Id: string;
  LabelKey: string;
  Value: string;
  Hint: string | null;
  Icon: NavigationIconName;
  Path: string | null;
}

export interface HomeShortcut {
  Id: string;
  Path: string;
  LabelKey: string;
  Icon: NavigationIconName;
}

export interface HomeShortcutGroup {
  Id: string;
  TitleKey: string;
  Items: HomeShortcut[];
}

export interface HomeQuickAction {
  Id: string;
  Path: string;
  LabelKey: string;
}

// Agrupación del menú en el inicio; lo que no esté aquí cae en "Otros"
const ShortcutGroups: { Id: string; TitleKey: string; ModuleIds: string[] }[] = [
  {
    Id: 'school',
    TitleKey: 'Home.Groups.School',
    ModuleIds: ['students', 'enrollments', 'enrollment-wizard', 're-enrollments', 'class-groups', 'grades', 'tracking'],
  },
  {
    Id: 'planning',
    TitleKey: 'Home.Groups.Planning',
    ModuleIds: ['careers', 'subjects', 'study-plans', 'academic-terms', 'teachers', 'course-materials'],
  },
  {
    Id: 'services',
    TitleKey: 'Home.Groups.Services',
    ModuleIds: ['payment-concepts', 'procedure-types', 'notifications'],
  },
  {
    Id: 'admin',
    TitleKey: 'Home.Groups.Admin',
    ModuleIds: ['dashboard', 'users', 'roles', 'audit-logs', 'settings'],
  },
];

const QuickActions: (HomeQuickAction & { Permission: string })[] = [
  { Id: 'enroll', Path: '/enrollment-wizard', LabelKey: 'Home.Actions.Enroll', Permission: 'Enrollments.Create' },
  { Id: 'student', Path: '/students/new', LabelKey: 'Home.Actions.NewStudent', Permission: 'Students.Create' },
  { Id: 'grades', Path: '/grades', LabelKey: 'Home.Actions.CaptureGrades', Permission: 'Grades.Capture' },
];

function BuildInitials(FullName: string): string {
  return FullName.split(' ')
    .filter((Part) => Part.length > 0)
    .slice(0, 2)
    .map((Part) => Part[0].toUpperCase())
    .join('');
}

// ViewModel del inicio: saludo, periodo en curso, indicadores y accesos según los permisos del usuario.
export function useHomeViewModel(Today: Date = new Date()) {
  const Translation = useTranslation();
  const Auth = useAuth();
  const Permissions = usePermissions();
  const Language = NormalizeLanguage(Translation.i18n.language);
  const CurrentUser = Auth.CurrentUser;

  const CanSeeReports = Permissions.Can('Reports.View');
  const Summary = useDashboardViewModel(CanSeeReports);
  const Dashboard = CanSeeReports ? Summary.Dashboard : null;

  const FullName = CurrentUser?.FullName ?? '';
  const FirstName = FullName.split(' ')[0] ?? '';

  const LongDate = new Intl.DateTimeFormat(Language === 'en' ? 'en-US' : 'es-MX', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  }).format(Today);

  const PathIf = (Path: string, Permission: string): string | null =>
    Permissions.Can(Permission) ? Path : null;

  const Indicators: HomeIndicator[] =
    Dashboard === null
      ? []
      : [
          {
            Id: 'students',
            LabelKey: 'Dashboard.Students',
            Value: FormatNumber(Dashboard.ActiveStudents, Language),
            Hint: null,
            Icon: 'Students',
            Path: PathIf('/students', 'Students.View'),
          },
          {
            Id: 'groups',
            LabelKey: 'Dashboard.Groups',
            Value: FormatNumber(Dashboard.OpenClassGroups, Language),
            Hint: Translation.t('Home.TeachersHint', { Count: Dashboard.ActiveTeachers }),
            Icon: 'ClassGroups',
            Path: PathIf('/class-groups', 'ClassGroups.View'),
          },
          {
            Id: 'charges',
            LabelKey: 'Dashboard.PendingCharges',
            Value: FormatNumber(Dashboard.PendingCharges, Language),
            Hint: FormatCurrency(Dashboard.PendingAmount, Language),
            Icon: 'Payments',
            Path: PathIf('/charges', 'Payments.View'),
          },
          {
            Id: 'procedures',
            LabelKey: 'Home.PendingProcedures',
            Value: FormatNumber(Dashboard.RequestedProcedures, Language),
            Hint: Translation.t('Home.IssuedHint', { Count: Dashboard.IssuedProcedures }),
            Icon: 'Procedures',
            Path: PathIf('/procedures', 'Procedures.View'),
          },
        ];

  const Term: TermSummary | null = BuildTermSummary(Dashboard, Language, Today);

  const Visible = GetNavigationModules().filter(
    (Module) => Module.Id !== 'home' && (Module.Permission === null || Permissions.Can(Module.Permission)),
  );
  const Grouped = new Set(ShortcutGroups.flatMap((Group) => Group.ModuleIds));
  const ToShortcut = (Module: NavigationModule): HomeShortcut => ({
    Id: Module.Id,
    Path: Module.Path,
    LabelKey: Module.LabelKey,
    Icon: Module.Icon,
  });

  const Groups: HomeShortcutGroup[] = [
    ...ShortcutGroups.map((Group) => ({
      Id: Group.Id,
      TitleKey: Group.TitleKey,
      Items: Group.ModuleIds.map((Id) => Visible.find((Module) => Module.Id === Id))
        .filter((Module): Module is NavigationModule => Module !== undefined)
        .map(ToShortcut),
    })),
    {
      Id: 'other',
      TitleKey: 'Home.Groups.Other',
      Items: Visible.filter((Module) => !Grouped.has(Module.Id)).map(ToShortcut),
    },
  ].filter((Group) => Group.Items.length > 0);

  return {
    FirstName,
    Initials: BuildInitials(FullName),
    Roles: CurrentUser?.Roles ?? [],
    LongDate,
    CanSeeReports,
    IsLoadingSummary: CanSeeReports && Summary.IsLoading && Summary.Dashboard === null,
    SummaryError: CanSeeReports ? Summary.ErrorMessage : null,
    ReloadSummary: Summary.Reload,
    Indicators,
    Term,
    HasNoTerm: Dashboard !== null && Dashboard.CurrentTermId === null,
    QuickActions: QuickActions.filter((Action) => Permissions.Can(Action.Permission)),
    Groups,
  };
}
