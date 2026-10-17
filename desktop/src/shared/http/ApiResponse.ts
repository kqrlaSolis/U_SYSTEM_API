// Contrato estándar de la API (JSON en PascalCase). Ver docs/API/Conventions.md.
export interface ApiResponse<T> {
  Success: boolean;
  Message: string;
  Data: T;
  Errors: ApiError[];
  TraceId: string | null;
}

export interface ApiError {
  Code: string;
  Field: string;
  Message: string;
}

export interface PagedResult<T> {
  Items: T[];
  TotalCount: number;
  Page: number;
  PageSize: number;
}
