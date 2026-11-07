import { ITAlert, ITCard, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useDashboardViewModel } from '../viewmodel/useDashboardViewModel';

// Dashboard con los indicadores clave del sistema.
export function DashboardPage() {
  const ViewModel = useDashboardViewModel();
  const Translation = useTranslation();
  const Data = ViewModel.Dashboard;

  const FormatAmount = (Amount: number): string =>
    new Intl.NumberFormat(Translation.i18n.language === 'en' ? 'en-US' : 'es-MX', {
      style: 'currency',
      currency: 'MXN',
    }).format(Amount);

  return (
    <ITPage
      title={Translation.t('Dashboard.Title')}
      description={Translation.t('Dashboard.Subtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading && Data === null}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
    >
      {Data !== null && (
        <ITStack spacing={4}>
          {Data.CurrentTermId === null ? (
            <ITAlert variant="warning" title={Translation.t('Dashboard.NoTerm')}>
              {Translation.t('Dashboard.NoTermHint')}
            </ITAlert>
          ) : (
            <ITAlert variant="info" title={Translation.t('Dashboard.CurrentTerm')}>
              {Data.CurrentTermCode} · {Data.CurrentTermName} ·{' '}
              {Translation.t(`AcademicTerms.Status.${Data.CurrentTermStatus}`)} ·{' '}
              {Data.CurrentTermStartDate} → {Data.CurrentTermEndDate}
            </ITAlert>
          )}
          <ITCard title={Translation.t('Dashboard.Academic')}>
            <dl className="USystemStatusList">
              <div>
                <dt>{Translation.t('Dashboard.Students')}</dt>
                <dd>{Data.ActiveStudents}</dd>
              </div>
              <div>
                <dt>{Translation.t('Dashboard.Teachers')}</dt>
                <dd>{Data.ActiveTeachers}</dd>
              </div>
              <div>
                <dt>{Translation.t('Dashboard.Groups')}</dt>
                <dd>{Data.OpenClassGroups}</dd>
              </div>
              <div>
                <dt>{Translation.t('Dashboard.Enrollments')}</dt>
                <dd>{Data.TotalEnrollments}</dd>
              </div>
              <div>
                <dt>{Translation.t('Dashboard.TermEnrollments')}</dt>
                <dd>{Data.TermEnrollments}</dd>
              </div>
            </dl>
          </ITCard>

          <ITCard title={Translation.t('Dashboard.Finance')}>
            <dl className="USystemStatusList">
              <div>
                <dt>{Translation.t('Dashboard.PendingCharges')}</dt>
                <dd>{Data.PendingCharges}</dd>
              </div>
              <div>
                <dt>{Translation.t('Dashboard.PendingAmount')}</dt>
                <dd>{FormatAmount(Data.PendingAmount)}</dd>
              </div>
            </dl>
          </ITCard>

          <ITCard title={Translation.t('Dashboard.Procedures')}>
            <dl className="USystemStatusList">
              <div>
                <dt>{Translation.t('Dashboard.Requested')}</dt>
                <dd>{Data.RequestedProcedures}</dd>
              </div>
              <div>
                <dt>{Translation.t('Dashboard.Issued')}</dt>
                <dd>{Data.IssuedProcedures}</dd>
              </div>
            </dl>
            <ITText as="p" muted>
              {Translation.t('Dashboard.Hint')}
            </ITText>
          </ITCard>
        </ITStack>
      )}
    </ITPage>
  );
}
