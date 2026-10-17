import { act, renderHook, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { AuditLog } from '../model/AuditLog';
import { useAuditLogListViewModel } from './useAuditLogListViewModel';

const Mocks = vi.hoisted(() => ({ GetPaged: vi.fn(), GetById: vi.fn() }));

vi.mock('../model/AuditLogsService', () => ({
  AuditLogsService: {
    GetPaged: Mocks.GetPaged,
    GetById: Mocks.GetById,
  },
}));

const SampleEntry: AuditLog = {
  Id: 1,
  UserId: 1,
  Username: 'Administrator',
  Action: 'Create',
  Module: 'Careers',
  Entity: 'Career',
  EntityId: '1',
  OldValue: null,
  NewValue: '{"Code":"LAE"}',
  IpAddress: '127.0.0.1',
  CreatedAt: '2026-01-01T00:00:00Z',
};

describe('useAuditLogListViewModel', () => {
  beforeEach(() => {
    Mocks.GetPaged.mockReset();
    Mocks.GetById.mockReset();
  });

  it('carga la bitácora al montar', async () => {
    Mocks.GetPaged.mockResolvedValue({ Items: [SampleEntry], TotalCount: 1, Page: 1, PageSize: 20 });

    const RenderedHook = renderHook(() => useAuditLogListViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    expect(RenderedHook.result.current.Items).toHaveLength(1);
    expect(Mocks.GetPaged).toHaveBeenCalledWith(1, 20, {
      Module: '',
      Action: '',
      From: '',
      To: '',
    });
  });

  it('aplica filtros desde la primera página', async () => {
    Mocks.GetPaged.mockResolvedValue({ Items: [], TotalCount: 0, Page: 1, PageSize: 20 });

    const RenderedHook = renderHook(() => useAuditLogListViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    act(() => {
      RenderedHook.result.current.SetPage(3);
    });

    act(() => {
      RenderedHook.result.current.SetFilter('Module', 'Roles');
    });

    await waitFor(() => {
      expect(RenderedHook.result.current.Filters.Module).toBe('Roles');
    });

    act(() => {
      RenderedHook.result.current.ApplyFilters();
    });

    await waitFor(() => {
      expect(Mocks.GetPaged).toHaveBeenLastCalledWith(1, 20, {
        Module: 'Roles',
        Action: '',
        From: '',
        To: '',
      });
    });
  });

  it('abre el detalle con el registro consultado', async () => {
    Mocks.GetPaged.mockResolvedValue({ Items: [SampleEntry], TotalCount: 1, Page: 1, PageSize: 20 });
    Mocks.GetById.mockResolvedValue(SampleEntry);

    const RenderedHook = renderHook(() => useAuditLogListViewModel());

    await waitFor(() => {
      expect(RenderedHook.result.current.Items).toHaveLength(1);
    });

    await act(async () => {
      await RenderedHook.result.current.SelectRow(SampleEntry);
    });

    expect(RenderedHook.result.current.Selected?.Id).toBe(1);
    expect(Mocks.GetById).toHaveBeenCalledWith(1);
  });
});
