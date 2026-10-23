import { act, renderHook, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { CrudConfig, CrudRow } from './CrudTypes';
import { useCrudListViewModel } from './useCrudListViewModel';

interface TestRow extends CrudRow {
  Name: string;
}

interface TestRequest {
  Name: string;
}

const Mocks = {
  List: vi.fn(),
  Load: vi.fn(),
  Create: vi.fn(),
  Update: vi.fn(),
  Remove: vi.fn(),
};

const Config: CrudConfig<TestRow, TestRequest> = {
  Route: '/tests',
  TitleKey: 'Careers.Title',
  SubtitleKey: 'Careers.Subtitle',
  NewKey: 'Careers.New',
  EditKey: 'Careers.Edit',
  EmptyKey: 'Careers.Empty',
  SearchKey: 'Careers.Search',
  PermissionView: 'Careers.View',
  PermissionCreate: 'Careers.Create',
  PermissionEdit: 'Careers.Edit',
  PermissionDelete: 'Careers.Delete',
  Columns: [{ Field: 'Name', HeaderKey: 'Careers.Columns.Name' }],
  Fields: [{ Name: 'Name', LabelKey: 'Careers.Columns.Name', Required: true }],
  EmptyForm: { Name: '' },
  List: Mocks.List,
  Load: Mocks.Load,
  Create: Mocks.Create,
  Update: Mocks.Update,
  Remove: Mocks.Remove,
  ToForm: (Row) => ({ Name: Row.Name }),
  FromForm: (Values) => ({ Name: Values.Name }),
};

describe('useCrudListViewModel', () => {
  beforeEach(() => {
    Mocks.List.mockReset();
    Mocks.Remove.mockReset();
  });

  it('carga la primera página y pagina contra el servicio', async () => {
    Mocks.List.mockResolvedValue({
      Items: [{ Id: 1, Name: 'LAE' }],
      TotalCount: 1,
      Page: 1,
      PageSize: 20,
    });

    const RenderedHook = renderHook(() => useCrudListViewModel(Config));

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    expect(RenderedHook.result.current.Items).toHaveLength(1);
    expect(Mocks.List).toHaveBeenCalledWith(1, 20, '', {});

    act(() => {
      RenderedHook.result.current.SetPage(2);
    });

    await waitFor(() => {
      expect(Mocks.List).toHaveBeenLastCalledWith(2, 20, '', {});
    });
  });

  it('sin permisos no expone acciones de alta, edición ni baja', async () => {
    Mocks.List.mockResolvedValue({ Items: [], TotalCount: 0, Page: 1, PageSize: 20 });

    const RenderedHook = renderHook(() => useCrudListViewModel(Config));

    await waitFor(() => {
      expect(RenderedHook.result.current.IsLoading).toBe(false);
    });

    expect(RenderedHook.result.current.CanCreate).toBe(false);
    expect(RenderedHook.result.current.CanEdit).toBe(false);
    expect(RenderedHook.result.current.CanDelete).toBe(false);
  });

  it('expone el mensaje de error del servicio', async () => {
    Mocks.List.mockRejectedValue(new Error('No se pudo conectar con el servidor'));

    const RenderedHook = renderHook(() => useCrudListViewModel(Config));

    await waitFor(() => {
      expect(RenderedHook.result.current.ErrorMessage).toBe(
        'No se pudo conectar con el servidor',
      );
    });
  });

  it('elimina a través del servicio y recarga el listado', async () => {
    Mocks.List.mockResolvedValue({ Items: [{ Id: 7, Name: 'LAE' }], TotalCount: 1, Page: 1, PageSize: 20 });
    Mocks.Remove.mockResolvedValue(undefined);

    const RenderedHook = renderHook(() => useCrudListViewModel(Config));

    await waitFor(() => {
      expect(RenderedHook.result.current.Items).toHaveLength(1);
    });

    await act(async () => {
      await RenderedHook.result.current.Remove({ Id: 7, Name: 'LAE' });
    });

    expect(Mocks.Remove).toHaveBeenCalledWith(7);
    expect(Mocks.List).toHaveBeenCalledTimes(2);
  });
});
