import type { ReactNode } from 'react';
import type { DataTableColumn } from '../components';
import type { FilterField } from '../components/FilterBar';
import type { PagedResult } from '../http/ApiResponse';

// Tipos del CRUD genérico: cada módulo solo declara columnas, campos y servicios.

export interface CrudRow {
  Id: number;
}

export type CrudFieldType = 'text' | 'number' | 'email' | 'password' | 'switch' | 'select';

export interface CrudOption {
  value: string;
  label: string;
  // Compatibilidad con OptionType de AXZY UI (índice de cadenas)
  [Key: string]: string;
}

export interface CrudField {
  Name: string;
  LabelKey: string;
  Type?: CrudFieldType;
  Required?: boolean;
  // Opciones de un campo select (valores estáticos del catálogo)
  Options?: CrudOption[];
}

export interface CrudColumn<TRow> {
  Field: string;
  HeaderKey: string;
  Type?: DataTableColumn<TRow>['Type'];
  Render?: (Row: TRow) => ReactNode;
}

export interface CrudConfig<TRow extends CrudRow, TRequest> {
  // Ruta base de la pantalla (minúsculas con guiones)
  Route: string;
  TitleKey: string;
  SubtitleKey: string;
  NewKey: string;
  EditKey: string;
  EmptyKey: string;
  SearchKey: string;

  PermissionView: string;
  PermissionCreate: string;
  PermissionEdit: string;
  // Vacío cuando el módulo no permite eliminar (solo desactivar)
  PermissionDelete?: string;

  Columns: CrudColumn<TRow>[];
  Fields: CrudField[];
  EmptyForm: Record<string, string>;

  // Filtros opcionales de la tabla (se envían al API como query string)
  Filters?: FilterField[];

  List: (
    Page: number,
    PageSize: number,
    Search: string,
    Filters: Record<string, string>,
  ) => Promise<PagedResult<TRow>>;
  Load: (Id: number) => Promise<TRow>;
  Create: (Request: TRequest) => Promise<TRow>;
  Update: (Id: number, Request: TRequest) => Promise<TRow>;
  Remove?: (Id: number) => Promise<void>;

  // Acciones extra por fila (reciben la fila y un recargar del listado)
  RowActions?: (Row: TRow, Reload: () => Promise<void>) => ReactNode;

  ToForm: (Row: TRow) => Record<string, string>;
  FromForm: (Values: Record<string, string>) => TRequest;
}
