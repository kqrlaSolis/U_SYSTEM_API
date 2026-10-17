import { ITAlert, ITInput, ITPage, ITText } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { AppButton, FormField } from '../../../shared/components';
import { useChangePasswordViewModel } from '../viewmodel/useChangePasswordViewModel';

// Cambio de contraseña: obligatorio cuando la API marca MustChangePassword.
export function ChangePasswordPage() {
  const ViewModel = useChangePasswordViewModel();
  const Translation = useTranslation();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  return (
    <ITPage
      title={Translation.t('ChangePassword.Title')}
      description={Translation.t('ChangePassword.Subtitle')}
      maxWidth="3xl"
    >
      <form className="USystemStackForm" onSubmit={HandleSubmit} noValidate>
        {ViewModel.ErrorMessage !== null && (
          <ITAlert variant="error" title={Translation.t('Common.Error')}>
            {ViewModel.ErrorMessage}
          </ITAlert>
        )}

        <FormField
          Label={Translation.t('ChangePassword.Current')}
          HtmlFor="CurrentPassword"
          IsRequired
          ErrorMessage={ViewModel.ValidationErrors.CurrentPassword}
        >
          <ITInput
            name="CurrentPassword"
            type="password"
            value={ViewModel.CurrentPassword}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetCurrentPassword(Event.target.value);
            }}
            autoFocus
            disabled={ViewModel.IsSubmitting}
          />
        </FormField>

        <FormField
          Label={Translation.t('ChangePassword.New')}
          HtmlFor="NewPassword"
          IsRequired
          ErrorMessage={ViewModel.ValidationErrors.NewPassword}
        >
          <ITInput
            name="NewPassword"
            type="password"
            value={ViewModel.NewPassword}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetNewPassword(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting}
          />
        </FormField>

        <FormField
          Label={Translation.t('ChangePassword.Confirm')}
          HtmlFor="ConfirmPassword"
          IsRequired
          ErrorMessage={ViewModel.ValidationErrors.ConfirmPassword}
        >
          <ITInput
            name="ConfirmPassword"
            type="password"
            value={ViewModel.ConfirmPassword}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetConfirmPassword(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting}
          />
        </FormField>

        <ITText as="p" muted>
          {Translation.t('ChangePassword.Policy')}
        </ITText>

        <AppButton
          Type="submit"
          Label={Translation.t('ChangePassword.Submit')}
          IsLoading={ViewModel.IsSubmitting}
        />
      </form>
    </ITPage>
  );
}
