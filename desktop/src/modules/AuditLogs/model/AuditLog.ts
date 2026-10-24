// Registro de bitácora (GET /api/v1/audit-logs)
export interface AuditLog {
  Id: number;
  UserId: number | null;
  Username: string | null;
  Action: string;
  Module: string;
  Entity: string;
  EntityId: string | null;
  OldValue: string | null;
  NewValue: string | null;
  IpAddress: string | null;
  CreatedAt: string;
}

// Filtros del listado; las fechas viajan como ISO (yyyy-mm-dd)
export interface AuditLogFilters {
  Module: string;
  Action: string;
  From: string;
  To: string;
}
