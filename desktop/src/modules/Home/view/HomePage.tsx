import { ITAlert, ITBadget, ITCard, ITPage, ITSkeleton, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { GetNavigationIcon } from '../../../app/layout/NavigationIcons';
import { AppButton, KpiTile } from '../../../shared/components';
import { TermProgressCard } from '../../Reports/view/TermProgressCard';
import { useHomeViewModel } from '../viewmodel/useHomeViewModel';

// Inicio: saludo y acciones del día, indicadores, periodo en curso y accesos agrupados por área.
export function HomePage() {
  const Translation = useTranslation();
  const Navigator = useNavigate();
  const ViewModel = useHomeViewModel();

  return (
    <ITPage maxWidth="7xl">
      <div className="USystemHome">
        <section className="USystemHomeHero">
          <div className="USystemHomeGreeting">
            {ViewModel.Initials.length > 0 && (
              <span className="USystemHomeAvatar" aria-hidden="true">
                {ViewModel.Initials}
              </span>
            )}
            <div className="USystemHomeGreetingText">
              <span className="USystemHomeDate">{ViewModel.LongDate}</span>
              <h1 className="USystemHomeTitle">
                {ViewModel.FirstName.length > 0
                  ? Translation.t('Home.Hello', { Name: ViewModel.FirstName })
                  : Translation.t('Home.Title')}
              </h1>
              {ViewModel.Roles.length > 0 && (
                <div className="USystemHomeRoles">
                  {ViewModel.Roles.map((Role) => (
                    <ITBadget key={Role} label={Role} color="info" size="sm" />
                  ))}
                </div>
              )}
            </div>
          </div>

          {ViewModel.QuickActions.length > 0 && (
            <div className="USystemHomeActions">
              {ViewModel.QuickActions.map((Action, Index) => (
                <AppButton
                  key={Action.Id}
                  Label={Translation.t(Action.LabelKey)}
                  Variant={Index === 0 ? 'filled' : 'outlined'}
                  Size="sm"
                  OnClick={() => Navigator(Action.Path)}
                />
              ))}
            </div>
          )}
        </section>

        {ViewModel.CanSeeReports && (
          <section aria-label={Translation.t('Home.QuickSummary')}>
            {ViewModel.SummaryError !== null ? (
              <ITAlert variant="warning" title={Translation.t('Home.SummaryError')}>
                <div className="USystemInlineActions">
                  <span>{ViewModel.SummaryError}</span>
                  <AppButton
                    Label={Translation.t('Common.Retry')}
                    Variant="text"
                    Size="sm"
                    OnClick={() => void ViewModel.ReloadSummary()}
                  />
                </div>
              </ITAlert>
            ) : (
              <div className="USystemKpis">
                {ViewModel.IsLoadingSummary
                  ? [0, 1, 2, 3].map((Item) => (
                      <ITSkeleton key={Item} variant="rectangular" height={116} className="USystemKpiSkeleton" />
                    ))
                  : ViewModel.Indicators.map((Indicator) => (
                      <KpiTile
                        key={Indicator.Id}
                        Label={Translation.t(Indicator.LabelKey)}
                        Value={Indicator.Value}
                        Hint={Indicator.Hint}
                        Icon={GetNavigationIcon(Indicator.Icon)}
                        OnClick={Indicator.Path === null ? null : () => Navigator(Indicator.Path as string)}
                      />
                    ))}
              </div>
            )}
          </section>
        )}

        <div className="USystemHomeColumns">
          <ITCard title={Translation.t('Home.Shortcuts')} className="USystemHomeShortcuts">
            <div className="USystemHomeGroups">
              {ViewModel.Groups.map((Group) => (
                <section key={Group.Id} className="USystemHomeGroup">
                  <h2 className="USystemHomeGroupTitle">{Translation.t(Group.TitleKey)}</h2>
                  <div className="USystemHomeShortcutGrid">
                    {Group.Items.map((Item) => (
                      <button
                        key={Item.Id}
                        type="button"
                        className="USystemHomeShortcut"
                        onClick={() => Navigator(Item.Path)}
                      >
                        <span className="USystemHomeShortcutIcon">{GetNavigationIcon(Item.Icon)}</span>
                        <span className="USystemHomeShortcutName">{Translation.t(Item.LabelKey)}</span>
                      </button>
                    ))}
                  </div>
                </section>
              ))}
            </div>
          </ITCard>

          <div className="USystemHomeSide">
            {ViewModel.CanSeeReports && (
              <TermProgressCard
                Term={ViewModel.Term}
                IsLoading={ViewModel.IsLoadingSummary}
                HasNoTerm={ViewModel.HasNoTerm}
              />
            )}

            <ITCard title={Translation.t('Home.Account')}>
              <div className="USystemHomeAccount">
                <ITText as="p" muted>
                  {Translation.t('Home.AccountHint')}
                </ITText>
                <div className="USystemInlineActions">
                  <AppButton
                    Label={Translation.t('Home.Profile')}
                    Variant="outlined"
                    Size="sm"
                    OnClick={() => Navigator('/profile')}
                  />
                  <AppButton
                    Label={Translation.t('Home.ChangePassword')}
                    Variant="text"
                    Size="sm"
                    OnClick={() => Navigator('/change-password')}
                  />
                </div>
              </div>
            </ITCard>
          </div>
        </div>
      </div>
    </ITPage>
  );
}
