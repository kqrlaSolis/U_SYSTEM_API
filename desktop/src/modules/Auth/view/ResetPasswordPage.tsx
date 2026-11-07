import { ITAlert, ITInput } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { AppButton, FormField } from '../../../shared/components';
import { useResetPasswordViewModel } from '../viewmodel/useResetPasswordViewModel';

// Restablecimiento con el token de un solo uso que entrega la API.
export function ResetPasswordPage() {
  const ViewModel = useResetPasswordViewModel();
  const Translation = useTranslation();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  return (
    <div className="USystemLogin">
      <form className="USystemLoginCard" onSubmit={HandleSubmit} noValidate>
        <h1 className="USystemLoginTitle">{Translation.t('ResetPassword.Title')}</h1>
        <p className="USystemLoginSubtitle">{Translation.t('ResetPassword.Subtitle')}</p>

        {ViewModel.ErrorMessage !== null && (
          <ITAlert variant="error" title={Translation.t('Common.Error')}>
            {ViewModel.ErrorMessage}
          </ITAlert>
        )}

        {ViewModel.IsDone && (
          <ITAlert variant="success" title={Translation.t('ResetPassword.DoneTitle')}>
            {Translation.t('ResetPassword.DoneHint')}
          </ITAlert>
        )}

        <FormField
          Label={Translation.t('ResetPassword.Email')}
          HtmlFor="ResetEmail"
          IsRequired
          ErrorMessage={ViewModel.Errors.Email}
        >
          <ITInput
            name="ResetEmail"
            type="email"
            value={ViewModel.Email}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetEmail(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting || ViewModel.IsDone}
          />
        </FormField>

        <FormField
          Label={Translation.t('ResetPassword.Token')}
          HtmlFor="ResetToken"
          IsRequired
          ErrorMessage={ViewModel.Errors.Token}
        >
          <ITInput
            name="ResetToken"
            value={ViewModel.Token}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetToken(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting || ViewModel.IsDone}
          />
        </FormField>

        <FormField
          Label={Translation.t('ResetPassword.New')}
          HtmlFor="ResetNewPassword"
          IsRequired
          ErrorMessage={ViewModel.Errors.NewPassword}
        >
          <ITInput
            name="ResetNewPassword"
            type="password"
            value={ViewModel.NewPassword}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetNewPassword(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting || ViewModel.IsDone}
          />
        </FormField>

        <FormField
          Label={Translation.t('ResetPassword.Confirm')}
          HtmlFor="ResetConfirmPassword"
          IsRequired
          ErrorMessage={ViewModel.Errors.ConfirmPassword}
        >
          <ITInput
            name="ResetConfirmPassword"
            type="password"
            value={ViewModel.ConfirmPassword}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetConfirmPassword(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting || ViewModel.IsDone}
          />
        </FormField>

        <AppButton
          Type="submit"
          Label={Translation.t('ResetPassword.Submit')}
          IsLoading={ViewModel.IsSubmitting}
          Disabled={ViewModel.IsDone}
        />

        <div className="USystemLoginFooter">
          <Link to="/login">{Translation.t('ResetPassword.Back')}</Link>
        </div>
      </form>
    </div>
  );
}
