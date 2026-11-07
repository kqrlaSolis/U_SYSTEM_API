import { HttpClient } from '../../../shared/http/HttpClient';
import type { Dashboard } from './Dashboard';

const DashboardPath = '/api/v1/reports/dashboard';

// Servicio de indicadores (solo lectura)
export const ReportsService = {
  async GetDashboard(): Promise<Dashboard> {
    return HttpClient.Get<Dashboard>(DashboardPath);
  },
};
