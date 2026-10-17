import { act, renderHook, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { User } from '../model/User';
import { useUserListViewModel } from './useUserListViewModel';

const Mocks = vi.hoisted(() => ({ GetPaged: vi.fn() }));

vi.mock('../model/UsersService', () => ({
  UsersService: {
    GetPaged: Mocks.GetPaged,
    GetById: vi.fn(),
  },
}));

const SampleUsers: User[] = [
  {
    Id: 1,
    FirstName: 'Ana',
    PaternalLastName: 'López',
    MaternalLastName: null,
    FullName: 'Ana López',
    Email: 'ana@usystem.local',
    Username: 'ana',
    Phone: null,
    IsActive: true,
    MustChangePassword: false,
    CreatedAt: '2026-01-01T00:00:00Z',
    Roles: ['Administrator'],
    Permissions: ['Users.View'],
  },
  {
    Id: 2,
    FirstName: 'Luis',
    PaternalLastName: 'Pérez',
    MaternalLastName: null,
    FullName: 'Luis Pérez',
    Email: 'luis@usystem.local',
    Username: 'luis',
    Phone: null,
    IsActive: false,
    MustChangePassword: true,
    CreatedAt: '2026-01-02T00:00:00Z',
    Roles: ['ReadOnly'],
    Permissions: ['Users.View'],
  },
];

describe('useUserListViewModel', () => {
  beforeEach(() => {
    Mocks.GetPaged.mockReset();
  });

  it('carga la primera página al montar', async () => {
    Mocks.GetPaged.mockResolvedValue({
      Items: SampleUsers,
      TotalCount: 2,
      Page: 1,
      PageSize: 20,
    });

    const RenderedHook = renderHook(() => useUserListViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    expect(RenderedHook.result.current.Items).toHaveLength(2);
    expect(RenderedHook.result.current.TotalCount).toBe(2);
    expect(RenderedHook.result.current.ErrorMessage).toBeNull();
    expect(Mocks.GetPaged).toHaveBeenCalledWith(1, 20, '');
  });

  it('consulta de nuevo al cambiar de página y de búsqueda', async () => {
    Mocks.GetPaged.mockResolvedValue({
      Items: SampleUsers,
      TotalCount: 40,
      Page: 1,
      PageSize: 20,
    });

    const RenderedHook = renderHook(() => useUserListViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    act(() => {
      RenderedHook.result.current.SetPage(2);
    });

    await waitFor(() => {
      expect(Mocks.GetPaged).toHaveBeenLastCalledWith(2, 20, '');
    });

    act(() => {
      RenderedHook.result.current.SetSearch('ana');
    });

    await waitFor(() => {
      expect(Mocks.GetPaged).toHaveBeenLastCalledWith(1, 20, 'ana');
    });
  });

  it('expone el mensaje de error cuando el servicio falla', async () => {
    Mocks.GetPaged.mockRejectedValue(new Error('No se pudo conectar con el servidor'));

    const RenderedHook = renderHook(() => useUserListViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.ErrorMessage).toBe(
        'No se pudo conectar con el servidor',
      );
    });

    expect(RenderedHook.result.current.IsLoading).toBe(false);
    expect(RenderedHook.result.current.Items).toHaveLength(0);
  });

  it('sin sesión no expone permisos de alta ni de edición', async () => {
    Mocks.GetPaged.mockResolvedValue({
      Items: [],
      TotalCount: 0,
      Page: 1,
      PageSize: 20,
    });

    const RenderedHook = renderHook(() => useUserListViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    expect(RenderedHook.result.current.CanCreate).toBe(false);
    expect(RenderedHook.result.current.CanEdit).toBe(false);
  });
});
