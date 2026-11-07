import {
  ITAlert,
  ITCard,
  ITCheckbox,
  ITInput,
  ITPage,
  ITSlideToggle,
  ITStack,
  ITText,
} from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import { AppButton, FormField } from '../../../shared/components';
import { useRoleFormViewModel } from '../viewmodel/useRoleFormViewModel';

// Editor de rol: datos y permisos agrupados por módulo.
export function RoleFormPage() {
  const Params = useParams<{ Id?: string }>();
  const ParsedId = Params.Id === undefined ? Number.NaN : Number(Params.Id);
  const ViewModel = useRoleFormViewModel(Number.isFinite(ParsedId) ? ParsedId : undefined);
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  return (
    <ITPage
      title={Translation.t(ViewModel.IsEditing ? 'Roles.Edit' : 'Roles.New')}
      description={Translation.t('Roles.Subtitle')}
      maxWidth="6xl"
      loading={ViewModel.IsLoading}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
      actions={
        <AppButton
          Label={Translation.t('Common.Cancel')}
          Variant="text"
          OnClick={() => Navigator('/roles')}
        />
      }
    >
      <form className="USystemStackForm" onSubmit={HandleSubmit} noValidate>
        {ViewModel.IsSystem && (
          <ITAlert variant="info" title={Translation.t('Roles.SystemRole')}>
            {Translation.t('Roles.SystemRoleHint')}
          </ITAlert>
        )}

        <FormField
          Label={Translation.t('Roles.Fields.Name')}
          HtmlFor="RoleName"
          IsRequired
          ErrorMessage={ViewModel.NameError ?? undefined}
        >
          <ITInput
            name="RoleName"
            value={ViewModel.Name}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetName(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting}
          />
        </FormField>

        <FormField Label={Translation.t('Roles.Fields.Description')} HtmlFor="RoleDescription">
          <ITInput
            name="RoleDescription"
            type="textarea"
            value={ViewModel.Description}
            onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
              ViewModel.SetDescription(Event.target.value);
            }}
            disabled={ViewModel.IsSubmitting}
          />
        </FormField>

        <div className="USystemInlineActions">
          <ITText as="span" muted>
            {Translation.t('Roles.Fields.IsActive')}
          </ITText>
          <ITSlideToggle
            isOn={ViewModel.IsActive}
            onToggle={ViewModel.SetIsActive}
            disabled={ViewModel.IsSubmitting}
          />
          <ITText as="span" muted>
            {Translation.t('Roles.Permissions.Selected')}: {ViewModel.SelectedCount}
          </ITText>
        </div>

        <ITText as="h2" className="USystemSectionTitle">
          {Translation.t('Roles.Permissions.Title')}
        </ITText>
        <ITText as="p" muted>
          {Translation.t('Roles.Permissions.Hint')}
        </ITText>

        {ViewModel.Catalog.map((Module) => {
          const ModuleCodes = Module.Permissions.map((Permission) => Permission.Code);
          const AllSelected = ModuleCodes.every((Code) => ViewModel.SelectedCodes.includes(Code));

          return (
            <ITCard key={Module.Module} title={Module.Module}>
              <ITStack spacing={3}>
                <ITCheckbox
                  label={Translation.t('Roles.Permissions.SelectAll')}
                  checked={AllSelected}
                  onChange={(Checked: boolean) => ViewModel.ToggleModule(ModuleCodes, Checked)}
                  disabled={ViewModel.IsSubmitting}
                />

                {Module.Permissions.map((Permission) => (
                  <ITCheckbox
                    key={Permission.Code}
                    name={Permission.Code}
                    label={`${Permission.Code} — ${Permission.Description}`}
                    checked={ViewModel.SelectedCodes.includes(Permission.Code)}
                    onChange={(Checked: boolean) => ViewModel.ToggleCode(Permission.Code, Checked)}
                    disabled={ViewModel.IsSubmitting}
                  />
                ))}
              </ITStack>
            </ITCard>
          );
        })}

        <AppButton
          Type="submit"
          Label={Translation.t('Common.Save')}
          IsLoading={ViewModel.IsSubmitting}
        />
      </form>
    </ITPage>
  );
}
