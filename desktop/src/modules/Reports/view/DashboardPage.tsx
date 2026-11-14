import { ITAlert, ITBadget, ITCard, ITPage, ITProgress, ITSkeleton, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { GetNavigationIcon } from '../../../app/layout/NavigationIcons';
import { AppButton, KpiTile } from '../../../shared/components';
import { useDashboardPageViewModel } from '../viewmodel/useDashboardPageViewModel';
import { TermProgressCard } from './TermProgressCard';

// Dashboard: periodo en curso, indicadores académicos, cobertura, finanzas y trámites.
export function DashboardPage() {
  const ViewModel = useDashboardPageViewModel();
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const Actions = (
    <div className="USystemInlineActions">
      {ViewModel.LastUpdated !== null && (
        <ITText as="span" muted>
          {Translation.t('Dashboard.UpdatedAt', { Time: ViewModel.LastUpdated })}
        </ITText>
      )}
      <AppButton
        Label={Translation.t('Dashboard.Refresh')}
        Variant="outlined"
        Size="sm"
        IsLoading={ViewModel.IsRefreshing}
        OnClick={() => void ViewModel.Reload()}
      />
    </div>
  );

  return (
    <ITPage
      title={Translation.t('Dashboard.Title')}
      description={Translation.t('Dashboard.Subtitle')}
      actions={Actions}
      maxWidth="7xl"
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
    >
      <div className="USystemDashboard">
        {ViewModel.RefreshError !== null && (
          <ITAlert variant="warning" title={Translation.t('Dashboard.RefreshError')}>
            {ViewModel.RefreshError}
          </ITAlert>
        )}

        {ViewModel.HasNoTerm ? (
          <ITAlert variant="warning" title={Translation.t('Dashboard.NoTerm')}>
            {Translation.t('Dashboard.NoTermHint')}
          </ITAlert>
        ) : (
          <TermProgressCard Term={ViewModel.Term} IsLoading={ViewModel.IsLoading} HasNoTerm={false} Wide />
        )}

        <section className="USystemDashboardSection" aria-labelledby="DashboardAcademic">
          <h2 id="DashboardAcademic" className="USystemDashboardSectionTitle">
            {Translation.t('Dashboard.Academic')}
          </h2>
          <div className="USystemKpis">
            {ViewModel.IsLoading
              ? [0, 1, 2, 3].map((Item) => (
                  <ITSkeleton key={Item} variant="rectangular" height={116} className="USystemKpiSkeleton" />
                ))
              : ViewModel.Academic.map((Kpi) => (
                  <KpiTile
                    key={Kpi.Id}
                    Label={Translation.t(Kpi.LabelKey)}
                    Value={Kpi.Value}
                    Hint={Kpi.Hint}
                    Icon={GetNavigationIcon(Kpi.Icon)}
                    OnClick={Kpi.Path === null ? null : () => Navigator(Kpi.Path as string)}
                  />
                ))}
          </div>
        </section>

        <div className="USystemDashboardCards">
          <ITCard title={Translation.t('Dashboard.Coverage')}>
            {ViewModel.IsLoading ? (
              <ITSkeleton variant="text" count={3} />
            ) : ViewModel.Coverage === null ? (
              <ITText as="p" muted>
                {Translation.t('Dashboard.CoverageEmpty')}
              </ITText>
            ) : (
              <div className="USystemDashboardMetric">
                <span className="USystemDashboardFigure">{ViewModel.Coverage.Percent} %</span>
                <ITProgress value={ViewModel.Coverage.Percent} color="success" size="md" />
                <ITText as="p" muted>
                  {Translation.t('Dashboard.CoverageHint', {
                    Enrolled: ViewModel.Coverage.Enrolled,
                    Active: ViewModel.Coverage.Active,
                  })}
                </ITText>
              </div>
            )}
          </ITCard>

          <ITCard title={Translation.t('Dashboard.Finance')}>
            {ViewModel.Finance === null ? (
              <ITSkeleton variant="text" count={3} />
            ) : (
              <div className="USystemDashboardMetric">
                <span className="USystemDashboardLabel">{Translation.t('Dashboard.PendingAmount')}</span>
                <span className="USystemDashboardFigure">{ViewModel.Finance.Amount}</span>
                <dl className="USystemDashboardPairs">
                  <div>
                    <dt>{Translation.t('Dashboard.PendingCharges')}</dt>
                    <dd>{ViewModel.Finance.Count}</dd>
                  </div>
                  <div>
                    <dt>{Translation.t('Dashboard.AverageCharge')}</dt>
                    <dd>{ViewModel.Finance.Average}</dd>
                  </div>
                </dl>
                {ViewModel.Finance.Path !== null && (
                  <AppButton
                    Label={Translation.t('Dashboard.SeeCharges')}
                    Variant="text"
                    Size="sm"
                    OnClick={() => Navigator(ViewModel.Finance?.Path as string)}
                  />
                )}
              </div>
            )}
          </ITCard>

          <ITCard title={Translation.t('Dashboard.Procedures')}>
            {ViewModel.Procedures === null ? (
              <ITSkeleton variant="text" count={3} />
            ) : (
              <div className="USystemDashboardMetric">
                <dl className="USystemDashboardPairs">
                  <div>
                    <dt>{Translation.t('Dashboard.Requested')}</dt>
                    <dd className="USystemDashboardPairFigure">
                      {ViewModel.Procedures.RequestedText}
                      {ViewModel.Procedures.Requested > 0 && (
                        <ITBadget label={Translation.t('Dashboard.NeedsAttention')} color="warning" size="sm" />
                      )}
                    </dd>
                  </div>
                  <div>
                    <dt>{Translation.t('Dashboard.Issued')}</dt>
                    <dd className="USystemDashboardPairFigure">{ViewModel.Procedures.Issued}</dd>
                  </div>
                </dl>
                {ViewModel.Procedures.Path !== null && (
                  <AppButton
                    Label={Translation.t('Dashboard.SeeProcedures')}
                    Variant="text"
                    Size="sm"
                    OnClick={() => Navigator(ViewModel.Procedures?.Path as string)}
                  />
                )}
              </div>
            )}
          </ITCard>
        </div>

        <ITText as="p" muted>
          {Translation.t('Dashboard.Hint')}
        </ITText>
      </div>
    </ITPage>
  );
}
