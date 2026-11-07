import { ITAlert, ITInput } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Navigate, useLocation } from 'react-router-dom';
import LogoUrl from '../../../assets/brand/UsystemBannerAlt.svg';
import { useAuth } from '../../../shared/auth/AuthContext';
import { AppButton, FormField } from '../../../shared/components';
import { ChangeLanguage, NormalizeLanguage } from '../../../shared/i18n/Config';
import { useLoginViewModel } from '../viewmodel/useLoginViewModel';

export function LoginPage() {
  const ViewModel = useLoginViewModel();
  const Translation = useTranslation();
  const Auth = useAuth();
  const Location = useLocation();
  const Language = NormalizeLanguage(Translation.i18n.language);

  const From = (Location.state as { From?: string } | null)?.From ?? '/';

  if (Auth.IsAuthenticated) {
    return <Navigate to={From} replace />;
  }

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  const ToggleLanguage = () => {
    void ChangeLanguage(Language === 'en' ? 'es' : 'en');
  };

  return (
    <div className="USystemLogin">
      <form className="USystemLoginCard" onSubmit={HandleSubmit} noValidate>
        <img className="USystemLoginBanner" src={LogoUrl} alt={Translation.t('Common.AppName')} />

        <h1 className="USystemLoginTitle">{Translation.t('Login.Title')}</h1>
        <p className="USystemLoginSubtitle">{Translation.t('Login.Subtitle')}</p>

        {ViewModel.ErrorMessage !== null && (
          <ITAlert variant="error" title={Translation.t('Common.Error')}>
            {ViewModel.ErrorMessage}
          </ITAlert>
        )}

        {Auth.MustChangePassword && (
          <ITAlert variant="warning">{Translation.t('Login.MustChangePassword')}</ITAlert>
        )}

        <FormField
          Label={Translation.t('Login.Username')}
          HtmlFor="LoginUsername"
          IsRequired
          ErrorMessage={ViewModel.ValidationErrors.Username}
        >
          <ITInput
            name="LoginUsername"
            type="text"
            value={ViewModel.Username}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetUsername(Event.target.value);
            }}
            autoFocus
            disabled={ViewModel.IsSubmitting}
          />
        </FormField>

        <FormField
          Label={Translation.t('Login.Password')}
          HtmlFor="LoginPassword"
          IsRequired
          ErrorMessage={ViewModel.ValidationErrors.Password}
        >
          <ITInput
            name="LoginPassword"
            type="password"
            value={ViewModel.Password}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetPassword(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting}
          />
        </FormField>

        <AppButton
          Type="submit"
          Label={
            ViewModel.IsSubmitting
              ? Translation.t('Login.Submitting')
              : Translation.t('Login.Submit')
          }
          IsLoading={ViewModel.IsSubmitting}
        />

        <Link to="/forgot-password" className="USystemLoginLink">
          {Translation.t('Login.ForgotPassword')}
        </Link>

        <div className="USystemLoginFooter">
          <AppButton
            Variant="text"
            Size="sm"
            Label={Language === 'es' ? 'EN' : 'ES'}
            AriaLabel={Translation.t('Common.Language')}
            OnClick={ToggleLanguage}
          />
          <span>{Translation.t('Login.Hint')}</span>
        </div>
      </form>
    </div>
  );
}
