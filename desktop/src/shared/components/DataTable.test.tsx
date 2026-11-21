import { fireEvent, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { RenderWithTheme } from '../../test/RenderWithTheme';
import { DataTable } from './DataTable';
import type { DataTableColumn } from './DataTable';

interface SampleRow {
  Id: number;
  Name: string;
}

const Columns: DataTableColumn<SampleRow>[] = [{ Field: 'Name', Header: 'Nombre' }];
const Rows: SampleRow[] = Array.from({ length: 25 }, (_, Index) => ({
  Id: Index + 1,
  Name: `Fila ${Index + 1}`,
}));

// Sin OnPageChange la tabla pagina en el cliente: no debe pintar las 25 filas de golpe.
describe('DataTable en modo cliente', () => {
  it('muestra la primera página y avanza a la siguiente', () => {
    RenderWithTheme(<DataTable<SampleRow> Columns={Columns} Rows={Rows} />);

    expect(screen.queryByText('Fila 1')).not.toBeNull();
    expect(screen.queryByText('Fila 20')).not.toBeNull();
    expect(screen.queryByText('Fila 21')).toBeNull();

    fireEvent.click(screen.getByRole('button', { name: /página siguiente|next page/i }));

    expect(screen.queryByText('Fila 21')).not.toBeNull();
    expect(screen.queryByText('Fila 25')).not.toBeNull();
    expect(screen.queryByText('Fila 1')).toBeNull();
  });
});
