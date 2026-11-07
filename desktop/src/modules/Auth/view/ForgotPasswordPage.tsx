import { ITAlert, ITInput } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { AppButton, FormField } from '../../../shared/components';
import { useForgotPasswordViewModel } from '../viewmodel/useForgotPasswordViewModel';

// Solicitud de recuperación: siempre responde igual para no revelar cuentas.
export function ForgotPasswordPage() {
  const ViewModel = useForgotPasswordViewModel();
  const Translation = useTranslation();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  return (
    <div className="USystemLogin">
      <form className="USystemLoginCard" onSubmit={HandleSubmit} noValidate>
        <h1 className="USystemLoginTitle">{Translation.t('ForgotPassword.Title')}</h1>
        <p className="USystemLoginSubtitle">{Translation.t('ForgotPassword.Subtitle')}</p>

        {ViewModel.ErrorMessage !== null && (
          <ITAlert variant="error" title={Translation.t('Common.Error')}>
            {ViewModel.ErrorMessage}
          </ITAlert>
        )}

        {ViewModel.IsSent && (
          <ITAlert variant="success" title={Translation.t('ForgotPassword.SentTitle')}>
            {Translation.t('ForgotPassword.SentHint')}
          </ITAlert>
        )}

        <FormField
          Label={Translation.t('ForgotPassword.Email')}
          HtmlFor="ForgotEmail"
          IsRequired
          ErrorMessage={ViewModel.EmailError ?? undefined}
        >
          <ITInput
            name="ForgotEmail"
            type="email"
            value={ViewModel.Email}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetEmail(Event.target.value);
            }}
            autoFocus
            disabled={ViewModel.IsSubmitting || ViewModel.IsSent}
          />
        </FormField>

        <AppButton
          Type="submit"
          Label={Translation.t('ForgotPassword.Submit')}
          IsLoading={ViewModel.IsSubmitting}
          Disabled={ViewModel.IsSent}
        />

        <div className="USystemLoginFooter">
          <span>{Translation.t('ForgotPassword.DevHint')}</span>
          <Link to="/login">{Translation.t('ForgotPassword.Back')}</Link>
        </div>
      </form>
    </div>
  );
}
