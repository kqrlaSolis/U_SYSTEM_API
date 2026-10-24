import { useCallback, useEffect, useMemo, useState } from 'react';
import { GetErrorMessage } from '../../../shared/http/GetErrorMessage';
import { usePermissions } from '../../../shared/hooks/usePermissions';
import { SettingsService } from '../model/SettingsService';
import type { SystemSetting } from '../model/SystemSetting';

export interface SettingGroup {
  Category: string;
  Items: SystemSetting[];
}

// ViewModel de configuración: agrupa por categoría y guarda un parámetro a la vez.
export function useSettingsViewModel() {
  const Permissions = usePermissions();
  const [Items, SetItems] = useState<SystemSetting[]>([]);
  const [Drafts, SetDrafts] = useState<Record<string, string>>({});
  const [IsLoading, SetIsLoading] = useState(true);
  const [SavingKey, SetSavingKey] = useState<string | null>(null);
  const [SavedKey, SetSavedKey] = useState<string | null>(null);
  const [ErrorMessage, SetErrorMessage] = useState<string | null>(null);

  const Load = useCallback(async () => {
    SetIsLoading(true);
    SetErrorMessage(null);

    try {
      const Settings = await SettingsService.GetAll();
      SetItems(Settings);
      SetDrafts(
        Settings.reduce<Record<string, string>>((Accumulator, Setting) => {
          Accumulator[Setting.SettingKey] = Setting.SettingValue;
          return Accumulator;
        }, {}),
      );
    } catch (CaughtError) {
      SetErrorMessage(GetErrorMessage(CaughtError));
    } finally {
      SetIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void Load();
  }, [Load]);

  const SetValue = useCallback((SettingKey: string, Value: string) => {
    SetDrafts((Current) => ({ ...Current, [SettingKey]: Value }));
    SetSavedKey(null);
  }, []);

  const IsDirty = useCallback(
    (Setting: SystemSetting) => (Drafts[Setting.SettingKey] ?? '') !== Setting.SettingValue,
    [Drafts],
  );

  const Save = useCallback(
    async (Setting: SystemSetting) => {
      const Value = Drafts[Setting.SettingKey] ?? '';
      SetSavingKey(Setting.SettingKey);
      SetErrorMessage(null);

      try {
        const Updated = await SettingsService.Update(Setting.SettingKey, Value);
        SetItems((Current) =>
          Current.map((Item) => (Item.SettingKey === Updated.SettingKey ? Updated : Item)),
        );
        SetDrafts((Current) => ({ ...Current, [Updated.SettingKey]: Updated.SettingValue }));
        SetSavedKey(Updated.SettingKey);
      } catch (CaughtError) {
        SetErrorMessage(GetErrorMessage(CaughtError));
      } finally {
        SetSavingKey(null);
      }
    },
    [Drafts],
  );

  const Groups = useMemo<SettingGroup[]>(() => {
    const ByCategory = new Map<string, SystemSetting[]>();

    for (const Setting of Items) {
      const Current = ByCategory.get(Setting.Category) ?? [];
      Current.push(Setting);
      ByCategory.set(Setting.Category, Current);
    }

    return Array.from(ByCategory.entries())
      .sort(([Left], [Right]) => Left.localeCompare(Right))
      .map(([Category, GroupItems]) => ({ Category, Items: GroupItems }));
  }, [Items]);

  return {
    Groups,
    IsLoading,
    SavingKey,
    SavedKey,
    ErrorMessage,
    CanEdit: Permissions.Can('Settings.Edit'),
    GetValue: (Setting: SystemSetting) => Drafts[Setting.SettingKey] ?? Setting.SettingValue,
    IsDirty,
    SetValue,
    Save,
    Reload: Load,
  };
}
