import { act, renderHook, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { SystemSetting } from '../model/SystemSetting';
import { useSettingsViewModel } from './useSettingsViewModel';

const Mocks = vi.hoisted(() => ({ GetAll: vi.fn(), Update: vi.fn() }));

vi.mock('../model/SettingsService', () => ({
  SettingsService: {
    GetAll: Mocks.GetAll,
    Update: Mocks.Update,
  },
}));

const SampleSettings: SystemSetting[] = [
  {
    Id: 1,
    SettingKey: 'Login.MaxFailedAttempts',
    SettingValue: '5',
    ValueType: 'Int',
    Category: 'Security',
    Description: 'Failed attempts before locking the account',
  },
  {
    Id: 2,
    SettingKey: 'Grades.MaxScore',
    SettingValue: '10',
    ValueType: 'Decimal',
    Category: 'Grades',
    Description: 'Maximum grade',
  },
];

describe('useSettingsViewModel', () => {
  beforeEach(() => {
    Mocks.GetAll.mockReset();
    Mocks.Update.mockReset();
  });

  it('agrupa los parámetros por categoría al cargar', async () => {
    Mocks.GetAll.mockResolvedValue(SampleSettings);

    const RenderedHook = renderHook(() => useSettingsViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    expect(RenderedHook.result.current.Groups).toHaveLength(2);
    expect(RenderedHook.result.current.Groups[0].Category).toBe('Grades');
    expect(RenderedHook.result.current.GetValue(SampleSettings[0])).toBe('5');
  });

  it('guarda un parámetro y actualiza su valor', async () => {
    Mocks.GetAll.mockResolvedValue(SampleSettings);
    Mocks.Update.mockResolvedValue({ ...SampleSettings[0], SettingValue: '3' });

    const RenderedHook = renderHook(() => useSettingsViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    act(() => {
      RenderedHook.result.current.SetValue('Login.MaxFailedAttempts', '3');
    });

    expect(RenderedHook.result.current.IsDirty(SampleSettings[0])).toBe(true);

    await act(async () => {
      await RenderedHook.result.current.Save(SampleSettings[0]);
    });

    expect(Mocks.Update).toHaveBeenCalledWith('Login.MaxFailedAttempts', '3');
    expect(RenderedHook.result.current.SavedKey).toBe('Login.MaxFailedAttempts');

    const UpdatedItem = RenderedHook.result.current.Groups
      .flatMap((Group) => Group.Items)
      .find((Item) => Item.SettingKey === 'Login.MaxFailedAttempts');

    expect(UpdatedItem?.SettingValue).toBe('3');
    expect(UpdatedItem !== undefined && RenderedHook.result.current.IsDirty(UpdatedItem)).toBe(false);
  });

  it('expone el error de la API al guardar', async () => {
    Mocks.GetAll.mockResolvedValue(SampleSettings);
    Mocks.Update.mockRejectedValue(new Error('Valor inválido para Login.MaxFailedAttempts.'));

    const RenderedHook = renderHook(() => useSettingsViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    await act(async () => {
      await RenderedHook.result.current.Save(SampleSettings[0]);
    });

    expect(RenderedHook.result.current.ErrorMessage).toBe(
      'Valor inválido para Login.MaxFailedAttempts.',
    );
  });
});
