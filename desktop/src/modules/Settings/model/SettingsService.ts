import { HttpClient } from '../../../shared/http/HttpClient';
import type { SystemSetting, UpdateSettingRequest } from './SystemSetting';

const SettingsPath = '/api/v1/settings';

// Servicio del módulo: sin estado de UI y sin conocer React
export const SettingsService = {
  async GetAll(): Promise<SystemSetting[]> {
    return HttpClient.Get<SystemSetting[]>(SettingsPath);
  },

  async Update(SettingKey: string, SettingValue: string): Promise<SystemSetting> {
    const Request: UpdateSettingRequest = { SettingValue };
    return HttpClient.Put<SystemSetting>(`${SettingsPath}/${encodeURIComponent(SettingKey)}`, Request);
  },
};
