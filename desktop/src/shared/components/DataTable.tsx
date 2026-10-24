import { ITEmptyState, ITLoader, ITPagination, ITTable } from '@axzydev/axzy_ui_system';
import type { Column } from '@axzydev/axzy_ui_system';
import type { ReactNode } from 'react';

export type DataTableColumnType = 'string' | 'number' | 'boolean' | 'date' | 'catalog' | 'actions';

export interface DataTableColumn<TRow> {
  Field: string;
  Header: string;
  Type?: DataTableColumnType;
  Sortable?: boolean;
  Width?: number | string;
  Align?: 'left' | 'center' | 'right';
  Render?: (Row: TRow) => ReactNode;
}

export interface DataTableProps<TRow extends object> {
  Columns: DataTableColumn<TRow>[];
  Rows: TRow[];
  TotalCount: number;
  Page: number;
  PageSize?: number;
  IsLoading?: boolean;
  EmptyMessage?: string;
  Title?: string;
  Variant?: 'default' | 'striped' | 'bordered';
  Size?: 'sm' | 'md' | 'lg';
  OnPageChange?: (Page: number) => void;
  OnPageSizeChange?: (PageSize: number) => void;
  PageSizeOptions?: number[];
  OnRowClick?: (Row: TRow) => void;
}

const DefaultPageSize = 20;

// La paginación es del servidor: el ViewModel pide una página y aquí solo se
// pinta ITTable con esas filas + ITPagination. El pie interno de ITTable se
// oculta con el slot `design.footer` (API pública de la librería) para no
// mostrar dos paginadores.
export function DataTable<TRow extends object>({
  Columns,
  Rows,
  TotalCount,
  Page,
  PageSize = DefaultPageSize,
  IsLoading = false,
  EmptyMessage,
  Title,
  Variant = 'default',
  Size = 'md',
  OnPageChange,
  OnPageSizeChange,
  PageSizeOptions,
  OnRowClick,
}: DataTableProps<TRow>) {
  const TotalPages = Math.max(1, Math.ceil(TotalCount / PageSize));

  const TableColumns: Array<Column<Record<string, unknown>>> = Columns.map((DataColumn) => ({
    key: DataColumn.Field,
    label: DataColumn.Header,
    type: DataColumn.Type ?? 'string',
    sortable: DataColumn.Sortable,
    width: DataColumn.Width,
    align: DataColumn.Align,
    render: DataColumn.Render as ((Row: Record<string, unknown>) => ReactNode) | undefined,
  }));

  if (IsLoading) {
    return (
      <div className="USystemTableState">
        <ITLoader size="md" />
      </div>
    );
  }

  if (Rows.length === 0) {
    return (
      <div className="USystemTableState">
        <ITEmptyState title={EmptyMessage ?? ''} />
      </div>
    );
  }

  return (
    <div className="USystemDataTable">
      <ITTable
        // Si cambia el tamaño de página hay que remontar la tabla: ITTable guarda
        // su propio itemsPerPage inicial.
        key={`DataTable-${PageSize}`}
        columns={TableColumns}
        data={Rows as unknown as Array<Record<string, unknown>>}
        title={Title}
        variant={Variant}
        size={Size}
        showViewToggle={false}
        defaultItemsPerPage={PageSize}
        itemsPerPageOptions={[PageSize]}
        onRowClick={
          OnRowClick === undefined
            ? undefined
            : (Row) => {
                OnRowClick(Row as unknown as TRow);
              }
        }
        design={{ footer: { style: { display: 'none' } } }}
      />

      {OnPageChange !== undefined && TotalCount > 0 && (
        <div className="USystemTablePagination">
          <ITPagination
            currentPage={Page}
            totalPages={TotalPages}
            onPageChange={OnPageChange}
            totalItems={TotalCount}
            itemsPerPage={PageSize}
            itemsPerPageOptions={PageSizeOptions}
            onItemsPerPageChange={OnPageSizeChange}
          />
        </div>
      )}
    </div>
  );
}
