import { ITPage, ITSlideToggle, ITStack, ITInput } from '@axzydev/axzy_ui_system';
import type React from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import { AppButton, FormField } from '../components';
import type { CrudConfig, CrudRow } from './CrudTypes';
import { useCrudFormViewModel } from './useCrudFormViewModel';

export interface CrudFormPageProps<TRow extends CrudRow, TRequest> {
  Config: CrudConfig<TRow, TRequest>;
}

// Pantalla de alta/edición genérica: un ITPage con los campos declarados en la config.
export function CrudFormPage<TRow extends CrudRow, TRequest>({
  Config,
}: CrudFormPageProps<TRow, TRequest>) {
  const Params = useParams<{ Id?: string }>();
  const ParsedId = Params.Id === undefined ? Number.NaN : Number(Params.Id);
  const Id = Number.isFinite(ParsedId) ? ParsedId : undefined;
  const ViewModel = useCrudFormViewModel(Config, Id);
  const Translation = useTranslation();
  const Navigator = useNavigate();

  const HandleSubmit = (Event: React.FormEvent<HTMLFormElement>) => {
    Event.preventDefault();
    void ViewModel.Submit();
  };

  return (
    <ITPage
      title={Translation.t(ViewModel.IsEditing ? Config.EditKey : Config.NewKey)}
      description={Translation.t(Config.SubtitleKey)}
      maxWidth="5xl"
      loading={ViewModel.IsLoading}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      errorActionLabel={Translation.t('Common.Retry')}
      onRetry={ViewModel.Reload}
      actions={
        <AppButton
          Label={Translation.t('Common.Cancel')}
          Variant="text"
          OnClick={() => Navigator(Config.Route)}
        />
      }
    >
      <form className="USystemStackForm" onSubmit={HandleSubmit} noValidate>
        {Config.Fields.map((Field) => (
          <FormField
            key={Field.Name}
            Label={Translation.t(Field.LabelKey)}
            HtmlFor={Field.Name}
            IsRequired={Field.Required}
            ErrorMessage={ViewModel.Errors[Field.Name]}
          >
            {Field.Type === 'switch' ? (
              <ITSlideToggle
                isOn={ViewModel.Values[Field.Name] === 'true'}
                onToggle={(Checked: boolean) => {
                  ViewModel.SetValue(Field.Name, Checked ? 'true' : 'false');
                }}
                disabled={ViewModel.IsSubmitting}
              />
            ) : (
              <ITInput
                name={Field.Name}
                type={Field.Type ?? 'text'}
                value={ViewModel.Values[Field.Name] ?? ''}
                onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                  ViewModel.SetValue(Field.Name, Event.target.value);
                }}
                disabled={ViewModel.IsSubmitting}
              />
            )}
          </FormField>
        ))}

        <ITStack spacing={2}>
          <AppButton
            Type="submit"
            Label={Translation.t('Common.Save')}
            IsLoading={ViewModel.IsSubmitting}
          />
        </ITStack>
      </form>
    </ITPage>
  );
}
