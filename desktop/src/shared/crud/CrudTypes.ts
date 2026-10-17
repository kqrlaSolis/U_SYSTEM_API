import type { ReactNode } from 'react';
import type { DataTableColumn } from '../components';
import type { PagedResult } from '../http/ApiResponse';

// Tipos del CRUD genérico: cada módulo solo declara columnas, campos y servicios.

export interface CrudRow {
  Id: number;
}

export type CrudFieldType = 'text' | 'number' | 'email' | 'password' | 'switch';

export interface CrudField {
  Name: string;
  LabelKey: string;
  Type?: CrudFieldType;
  Required?: boolean;
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

  List: (Page: number, PageSize: number, Search: string) => Promise<PagedResult<TRow>>;
  Load: (Id: number) => Promise<TRow>;
  Create: (Request: TRequest) => Promise<TRow>;
  Update: (Id: number, Request: TRequest) => Promise<TRow>;
  Remove?: (Id: number) => Promise<void>;

  ToForm: (Row: TRow) => Record<string, string>;
  FromForm: (Values: Record<string, string>) => TRequest;
}
