import { ITBadget, ITCard, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import BannerUrl from '../../../assets/brand/UsystemBanner.svg';
import BannerAnimation from '../../../assets/lottie/UsystemBanner.json';
import { useAuth } from '../../../shared/auth/AuthContext';
import { LottieAnimation } from '../../../shared/components';
import { NormalizeLanguage } from '../../../shared/i18n/Config';
import { ApiBaseUrl } from '../../../shared/http/HttpClient';
import { FormatDate } from '../../../shared/utils/Format';

// Panel de bienvenida: identidad visual (banner + animación) y estado, sin reglas de negocio.
export function HomePage() {
  const Translation = useTranslation();
  const Auth = useAuth();
  const CurrentUser = Auth.CurrentUser;
  const Language = NormalizeLanguage(Translation.i18n.language);
  const IsDesktopRuntime = typeof window !== 'undefined' && window.USystem !== undefined;
  const AppVersion = window.USystem?.AppVersion ?? '—';
  const Today = FormatDate(new Date(), Language);

  return (
    <ITPage
      title={Translation.t('Home.Title')}
      description={Translation.t('Home.Subtitle')}
      maxWidth="6xl"
    >
      <ITStack spacing={4}>
        <ITCard>
          <div className="USystemHero">
            <div className="USystemHeroText">
              <h2 className="USystemHeroTitle">
                {CurrentUser === null
                  ? Translation.t('Home.Title')
                  : Translation.t('Home.Greeting', { Name: CurrentUser.FullName })}
              </h2>
              <ITText as="p" muted>
                {Translation.t('Home.Subtitle')}
              </ITText>
              <div>
                <ITBadget label={Translation.t('Home.PhaseValue')} color="success" size="sm" />
              </div>
            </div>

            <img className="USystemHeroBanner" src={BannerUrl} alt="" />
          </div>
        </ITCard>

        <ITCard title={Translation.t('Home.SystemStatus')}>
          <ITStack spacing={4}>
            <LottieAnimation
              Data={BannerAnimation}
              Height={140}
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
              <div>
                <dt>{Translation.t('Home.Today')}</dt>
                <dd>{Today}</dd>
              </div>
              <div>
                <dt>{Translation.t('Home.Phase')}</dt>
                <dd>
                  <ITBadget label={Translation.t('Home.PhaseValue')} color="info" size="sm" />
                </dd>
              </div>
            </dl>
          </ITStack>
        </ITCard>
      </ITStack>
    </ITPage>
  );
}
