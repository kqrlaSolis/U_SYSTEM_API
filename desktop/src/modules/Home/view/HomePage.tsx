import { ITBadget, ITCard, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../../shared/auth/AuthContext';
import { usePermissions } from '../../../shared/hooks/usePermissions';
import { NormalizeLanguage } from '../../../shared/i18n/Config';
import { FormatDate } from '../../../shared/utils/Format';
import { useDashboardViewModel } from '../../Reports/viewmodel/useDashboardViewModel';

// Inicio: saludo, resumen del periodo y accesos a los módulos más usados.
export function HomePage() {
  const Translation = useTranslation();
  const Auth = useAuth();
  const Permissions = usePermissions();
  const Navigator = useNavigate();
  const CurrentUser = Auth.CurrentUser;
  const Language = NormalizeLanguage(Translation.i18n.language);
  const Today = FormatDate(new Date(), Language);
  const CanSeeReports = Permissions.Can('Reports.View');
  const Summary = useDashboardViewModel(CanSeeReports);
  const DashboardData = CanSeeReports ? Summary.Dashboard : null;

  const Modules = [
    { Path: '/dashboard', LabelKey: 'Nav.Dashboard', Permission: 'Reports.View' },
    { Path: '/students', LabelKey: 'Nav.Students', Permission: 'Students.View' },
    { Path: '/enrollments', LabelKey: 'Nav.Enrollments', Permission: 'Enrollments.View' },
    { Path: '/grades', LabelKey: 'Nav.Grades', Permission: 'Grades.View' },
    { Path: '/procedures', LabelKey: 'Nav.Procedures', Permission: 'Procedures.View' },
    { Path: '/charges', LabelKey: 'Nav.Payments', Permission: 'Payments.View' },
    { Path: '/teachers', LabelKey: 'Nav.Teachers', Permission: 'Teachers.View' },
    { Path: '/class-groups', LabelKey: 'Nav.ClassGroups', Permission: 'ClassGroups.View' },
    { Path: '/careers', LabelKey: 'Nav.Careers', Permission: 'Careers.View' },
    { Path: '/subjects', LabelKey: 'Nav.Subjects', Permission: 'Subjects.View' },
  ].filter((Module) => Permissions.Can(Module.Permission));

  const Roles = CurrentUser?.Roles ?? [];
  const FirstName = CurrentUser?.FullName.split(' ')[0] ?? '';

  return (
    <ITPage
      title={Translation.t('Home.Title')}
      description={Translation.t('Home.Subtitle')}
      maxWidth="7xl"
    >
      <ITStack spacing={4}>
        <ITCard>
          <div className="USystemHero">
            <div className="USystemHeroText">
              <h2 className="USystemHeroTitle">
                {CurrentUser === null
                  ? Translation.t('Home.Title')
                  : Translation.t('Home.Hello', { Name: FirstName })}
              </h2>
              <ITText as="p" muted>
                {Translation.t('Home.WelcomeHint')}
              </ITText>
              <div className="USystemInlineActions">
                {Roles.map((Role) => (
                  <ITBadget key={Role} label={Role} color="info" size="sm" />
                ))}
                <ITText as="span" muted>
                  {Today}
                </ITText>
              </div>
            </div>

            {/* <img className="USystemHeroBanner" src={BannerUrl} alt="" /> */}
          </div>
        </ITCard>

        {CanSeeReports && (
          <ITCard title={Translation.t('Home.QuickSummary')}>
            {DashboardData === null ? (
              <ITText as="p" muted>
                {Translation.t('Common.Loading')}
              </ITText>
            ) : (
              <dl className="USystemStatusList">
                <div>
                  <dt>{Translation.t('Dashboard.Students')}</dt>
                  <dd>{DashboardData.ActiveStudents}</dd>
                </div>
                <div>
                  <dt>{Translation.t('Dashboard.Groups')}</dt>
                  <dd>{DashboardData.OpenClassGroups}</dd>
                </div>
                <div>
                  <dt>{Translation.t('Dashboard.PendingCharges')}</dt>
                  <dd>{DashboardData.PendingCharges}</dd>
                </div>
                <div>
                  <dt>{Translation.t('Dashboard.Requested')}</dt>
                  <dd>{DashboardData.RequestedProcedures}</dd>
                </div>
              </dl>
            )}
          </ITCard>
        )}

        <ITCard title={Translation.t('Home.Modules')}>
          <div className="USystemModuleGrid">
            {Modules.map((Module) => (
              <button
                key={Module.Path}
                type="button"
                className="USystemModuleCard"
                onClick={() => Navigator(Module.Path)}
              >
                <span className="USystemModuleName">{Translation.t(Module.LabelKey)}</span>
                <span className="USystemModuleHint">{Translation.t('Home.OpenModule')}</span>
              </button>
            ))}
          </div>
        </ITCard>

        {/* <ITCard title={Translation.t('Home.TechDetails')}>
          <ITStack spacing={3}>
            <LottieAnimation
              Data={BannerAnimation}
              Height={120}
              AriaLabel={Translation.t('Common.AppName')}
            />
            <dl className="USystemStatusList">
              <div>
                <dt>{Translation.t('Home.ApiBaseUrl')}</dt>
                <dd>{ApiBaseUrl}</dd>
              </div>
              <div>
                <dt>{Translation.t('Home.AppVersion')}</dt>
                <dd>{AppVersion}</dd>
              </div>
              <div>
                <dt>{Translation.t('Home.Runtime')}</dt>
                <dd>
                  {IsDesktopRuntime
                    ? Translation.t('Home.RuntimeDesktop')
                    : Translation.t('Home.RuntimeBrowser')}
                </dd>
              </div>
              <div>
                <dt>{Translation.t('Home.Language')}</dt>
                <dd>{Language}</dd>
              </div>
            </dl>
          </ITStack>
        </ITCard> */}
      </ITStack>
    </ITPage>
  );
}
