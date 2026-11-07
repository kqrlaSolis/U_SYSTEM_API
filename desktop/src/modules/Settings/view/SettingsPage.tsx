import { ITAlert, ITCard, ITInput, ITPage, ITStack, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import { AppButton } from '../../../shared/components';
import { useSettingsViewModel } from '../viewmodel/useSettingsViewModel';

// Configuración del sistema: parámetros agrupados por categoría con guardado por fila.
export function SettingsPage() {
  const ViewModel = useSettingsViewModel();
  const Translation = useTranslation();

  return (
    <ITPage
      title={Translation.t('Settings.Title')}
      description={Translation.t('Settings.Subtitle')}
      maxWidth="7xl"
      loading={ViewModel.IsLoading}
      error={ViewModel.ErrorMessage ?? undefined}
      errorTitle={Translation.t('Common.Error')}
      onRetry={ViewModel.Reload}
    >
      <ITStack spacing={4}>
        {ViewModel.SavedKey !== null && (
          <ITAlert variant="success" title={Translation.t('Common.Saved')}>
            {ViewModel.SavedKey}
          </ITAlert>
        )}

        {ViewModel.Groups.map((Group) => (
          <ITCard key={Group.Category} title={Group.Category}>
            <ITStack spacing={3}>
              {Group.Items.map((Setting) => (
                <div key={Setting.SettingKey} className="USystemSettingRow">
                  <div className="USystemSettingInfo">
                    <ITText as="span" className="USystemSettingKey">
                      {Setting.SettingKey}
                    </ITText>
                    <ITText as="span" muted>
                      {Setting.Description ?? Setting.ValueType}
                    </ITText>
                  </div>

                  <div className="USystemSettingEditor">
                    <ITInput
                      name={`Setting-${Setting.SettingKey}`}
                      value={ViewModel.GetValue(Setting)}
                      onChange={(Event: React.ChangeEvent<HTMLInputElement>) => {
                        ViewModel.SetValue(Setting.SettingKey, Event.target.value);
                      }}
                      disabled={!ViewModel.CanEdit || ViewModel.SavingKey === Setting.SettingKey}
                    />
                    <AppButton
                      Label={Translation.t('Common.Save')}
                      Size="sm"
                      IsLoading={ViewModel.SavingKey === Setting.SettingKey}
                      Disabled={!ViewModel.CanEdit || !ViewModel.IsDirty(Setting)}
                      OnClick={() => {
                        void ViewModel.Save(Setting);
                      }}
                    />
                  </div>
                </div>
              ))}
            </ITStack>
          </ITCard>
        ))}
      </ITStack>
    </ITPage>
  );
}
