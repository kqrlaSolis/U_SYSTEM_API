import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { AuditLog, AuditLogFilters } from './AuditLog';

const AuditLogsPath = '/api/v1/audit-logs';

// Servicio del módulo: sin estado de UI y sin conocer React
export const AuditLogsService = {
  async GetPaged(
    Page: number,
    PageSize: number,
    Filters: AuditLogFilters,
  ): Promise<PagedResult<AuditLog>> {
    return HttpClient.Get<PagedResult<AuditLog>>(AuditLogsPath, {
      Page,
      PageSize,
      Module: Filters.Module,
      Action: Filters.Action,
      From: Filters.From,
      To: Filters.To,
    });
  },

  async GetById(Id: number): Promise<AuditLog> {
    return HttpClient.Get<AuditLog>(`${AuditLogsPath}/${Id}`);
  },
};
