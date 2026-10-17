import {
  ITAlert,
  ITCard,
  ITInput,
  ITMultiSelect,
  ITPage,
  ITSlideToggle,
  ITStack,
  ITText,
} from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import { AppButton, FormField } from '../../../shared/components';
import { useUserFormViewModel } from '../viewmodel/useUserFormViewModel';

// Alta y edición de usuarios del personal, con roles, estado y contraseña temporal.
export function UserFormPage() {
  const Params = useParams<{ Id: string }>();
  const ParsedId = Params.Id === undefined ? Number.NaN : Number(Params.Id);
  const ViewModel = useUserFormViewModel(Number.isFinite(ParsedId) ? ParsedId : undefined);
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  return (
    <ITPage
      title={
        ViewModel.IsEditing
          ? Translation.t('Users.Form.TitleEdit')
          : Translation.t('Users.Form.TitleNew')
      }
      description={Translation.t('Users.Form.Subtitle')}
      maxWidth="5xl"
      loading={ViewModel.IsLoading}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      onRetry={ViewModel.Reload}
      actions={
        <AppButton
          Label={Translation.t('Common.Cancel')}
          Variant="text"
          OnClick={() => Navigator('/users')}
        />
      }
    >
      <form className="USystemStackForm" onSubmit={HandleSubmit} noValidate>
        {ViewModel.TemporaryPassword !== null && (
          <ITAlert variant="warning" title={Translation.t('Users.Form.TemporaryPassword')}>
            {ViewModel.TemporaryPassword}
          </ITAlert>
        )}

        <ITCard title={Translation.t('Users.Form.Account')}>
          <ITStack spacing={4}>
            <FormField
              Label={Translation.t('Users.Columns.FirstName')}
              HtmlFor="FirstName"
              IsRequired
              ErrorMessage={ViewModel.ValidationErrors.FirstName}
            >
              <ITInput
                name="FirstName"
                value={ViewModel.Form.FirstName}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('FirstName', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField
              Label={Translation.t('Users.Columns.PaternalLastName')}
              HtmlFor="PaternalLastName"
              IsRequired
              ErrorMessage={ViewModel.ValidationErrors.PaternalLastName}
            >
              <ITInput
                name="PaternalLastName"
                value={ViewModel.Form.PaternalLastName}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('PaternalLastName', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField Label={Translation.t('Users.Columns.MaternalLastName')} HtmlFor="MaternalLastName">
              <ITInput
                name="MaternalLastName"
                value={ViewModel.Form.MaternalLastName}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('MaternalLastName', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField
              Label={Translation.t('Users.Columns.Email')}
              HtmlFor="Email"
              IsRequired
              ErrorMessage={ViewModel.ValidationErrors.Email}
            >
              <ITInput
                name="Email"
                type="email"
                value={ViewModel.Form.Email}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('Email', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField
              Label={Translation.t('Users.Columns.Username')}
              HtmlFor="Username"
              IsRequired
              ErrorMessage={ViewModel.ValidationErrors.Username}
            >
              <ITInput
                name="Username"
                value={ViewModel.Form.Username}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('Username', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            <FormField Label={Translation.t('Users.Form.Phone')} HtmlFor="Phone">
              <ITInput
                name="Phone"
                value={ViewModel.Form.Phone}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetField('Phone', Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            </FormField>

            {!ViewModel.IsEditing && (
              <FormField
                Label={Translation.t('Users.Form.TemporaryPassword')}
                HtmlFor="TemporaryPassword"
              >
                <ITInput
                  name="TemporaryPassword"
                  type="password"
                  value={ViewModel.Form.TemporaryPassword}
                  onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                    ViewModel.SetField('TemporaryPassword', Event.target.value);
                  }}
                  disabled={ViewModel.IsSubmitting}
                />
              </FormField>
            )}
          </ITStack>
        </ITCard>

        <ITCard title={Translation.t('Users.Form.Roles')}>
          <ITStack spacing={4}>
            <ITMultiSelect
              name="RoleIds"
              placeholder={Translation.t('Users.Form.RolesPlaceholder')}
              options={ViewModel.RoleOptions}
              value={ViewModel.SelectedRoleIds}
              onChange={(Values: (string | number)[]) => {
                ViewModel.SetSelectedRoleIds(Values.map((Value) => Number(Value)));
              }}
              disabled={ViewModel.IsSubmitting}
            />
            {ViewModel.ValidationErrors.RoleIds !== undefined && (
              <ITText as="span" className="USystemFieldError">
                {ViewModel.ValidationErrors.RoleIds}
              </ITText>
            )}

            {ViewModel.IsEditing && (
              <div className="USystemInlineActions">
                <ITText as="span" muted>
                  {Translation.t('Users.Form.Status')}:
                </ITText>
                <ITSlideToggle
                  isOn={ViewModel.IsActive}
                  onToggle={() => {
                    void ViewModel.ToggleStatus();
                  }}
                  disabled={ViewModel.IsSubmitting}
                />
                <AppButton
                  Label={Translation.t('Users.Form.ResetPassword')}
                  Variant="outlined"
                  Size="sm"
                  IsLoading={ViewModel.IsSubmitting}
                  OnClick={() => {
                    void ViewModel.ResetPassword();
                  }}
                />
              </div>
            )}
          </ITStack>
        </ITCard>

        <AppButton
          Type="submit"
          Label={Translation.t('Common.Save')}
          IsLoading={ViewModel.IsSubmitting}
        />
      </form>
    </ITPage>
  );
}
