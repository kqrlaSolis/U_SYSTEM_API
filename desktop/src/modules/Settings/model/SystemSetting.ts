// Parámetro editable del sistema (SystemSettings).
export interface SystemSetting {
  Id: number;
  SettingKey: string;
  SettingValue: string;
  ValueType: string;
  Category: string;
  Description: string | null;
}

export interface UpdateSettingRequest {
  SettingValue: string;
}
