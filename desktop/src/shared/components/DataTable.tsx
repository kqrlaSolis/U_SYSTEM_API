import { ITEmptyState, ITLoader, ITPagination, ITTable } from '@axzydev/axzy_ui_system';
import type { Column } from '@axzydev/axzy_ui_system';
import { useState } from 'react';
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
  // Paginación en el servidor: pasar TotalCount, Page y OnPageChange.
  // Sin OnPageChange la tabla pagina en el cliente las filas que recibe.
  TotalCount?: number;
  Page?: number;
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
const DefaultPageSizeOptions = [10, 20, 50];

// Dos modos de paginación:
// - Servidor (OnPageChange definido): el ViewModel pide una página y aquí solo
//   se pinta esa página.
// - Cliente (sin OnPageChange): para listas que la API entrega completas
//   (calificaciones de un grupo, kardex, checklist…); la tabla corta las filas.
// En ambos casos se pinta ITTable + ITPagination. El pie interno de ITTable se
// oculta con el slot `design.footer` (API pública de la librería) para no
// mostrar dos paginadores.
export function DataTable<TRow extends object>({
  Columns,
  Rows,
  TotalCount,
  Page,
  PageSize,
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
  const IsClientSide = OnPageChange === undefined;
  const [ClientPage, SetClientPage] = useState(1);
  const [ClientPageSize, SetClientPageSize] = useState(PageSize ?? DefaultPageSize);

  const EffectivePageSize = IsClientSide ? ClientPageSize : (PageSize ?? DefaultPageSize);
  const EffectiveTotal = IsClientSide ? Rows.length : (TotalCount ?? Rows.length);
  const TotalPages = Math.max(1, Math.ceil(EffectiveTotal / EffectivePageSize));
  // Si la lista se achica (otro grupo, otro alumno) no se queda en una página vacía
  const EffectivePage = IsClientSide ? Math.min(ClientPage, TotalPages) : (Page ?? 1);
  const VisibleRows = IsClientSide
    ? Rows.slice((EffectivePage - 1) * EffectivePageSize, EffectivePage * EffectivePageSize)
    : Rows;

  const HandlePageChange = IsClientSide ? SetClientPage : OnPageChange;
  const HandlePageSizeChange = IsClientSide
    ? (NextPageSize: number) => {
        SetClientPageSize(NextPageSize);
        SetClientPage(1);
      }
    : OnPageSizeChange;
  const EffectivePageSizeOptions =
    PageSizeOptions ?? (IsClientSide ? DefaultPageSizeOptions : undefined);

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
        key={`DataTable-${EffectivePageSize}`}
        columns={TableColumns}
        data={VisibleRows as unknown as Array<Record<string, unknown>>}
        title={Title}
        variant={Variant}
        size={Size}
        showViewToggle={false}
        defaultItemsPerPage={EffectivePageSize}
        itemsPerPageOptions={[EffectivePageSize]}
        onRowClick={
          OnRowClick === undefined
            ? undefined
            : (Row) => {
                OnRowClick(Row as unknown as TRow);
              }
        }
        design={{ footer: { style: { display: 'none' } } }}
      />

      {EffectiveTotal > 0 && (
        <div className="USystemTablePagination">
          <ITPagination
            currentPage={EffectivePage}
            totalPages={TotalPages}
            onPageChange={HandlePageChange}
            totalItems={EffectiveTotal}
            itemsPerPage={EffectivePageSize}
            itemsPerPageOptions={EffectivePageSizeOptions}
            onItemsPerPageChange={HandlePageSizeChange}
          />
        </div>
      )}
    </div>
  );
}
