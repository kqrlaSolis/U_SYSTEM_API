import { renderHook, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { AuthUser } from '../../Auth/model/AuthResponse';
import type { Dashboard } from '../model/Dashboard';
import { useDashboardPageViewModel } from './useDashboardPageViewModel';

const Mocks = vi.hoisted(() => ({
  CurrentUser: null as AuthUser | null,
  GetDashboard: vi.fn(),
}));

vi.mock('../../../shared/auth/AuthContext', () => ({
  useAuth: () => ({ CurrentUser: Mocks.CurrentUser }),
}));

vi.mock('../model/ReportsService', () => ({
  ReportsService: { GetDashboard: Mocks.GetDashboard },
}));

const BuildUser = (Permissions: string[]): AuthUser => ({
  Id: 1,
  Username: 'finanzas',
  FullName: 'Luis Gómez',
  Email: 'luis@usystem.local',
  Roles: ['Finance'],
  Permissions,
});

const SampleDashboard: Dashboard = {
  CurrentTermId: 3,
  CurrentTermCode: '2026-3',
  CurrentTermName: 'Septiembre - Diciembre 2026',
  CurrentTermStatus: 'InProgress',
  CurrentTermStartDate: '2026-09-01',
  CurrentTermEndDate: '2026-12-10',
  ActiveStudents: 400,
  ActiveTeachers: 32,
  OpenClassGroups: 28,
  TermEnrollments: 360,
  PendingCharges: 50,
  PendingAmount: 150000,
  RequestedProcedures: 6,
  IssuedProcedures: 14,
  TotalEnrollments: 1200,
};

describe('useDashboardPageViewModel', () => {
  beforeEach(() => {
    Mocks.GetDashboard.mockReset();
  });

  it('calcula cobertura y promedio por cargo, y solo enlaza lo permitido', async () => {
    Mocks.CurrentUser = BuildUser(['Reports.View', 'Payments.View']);
    Mocks.GetDashboard.mockResolvedValue(SampleDashboard);

    const { result: Result } = renderHook(() => useDashboardPageViewModel(new Date(2026, 10, 7)));

    await waitFor(() => expect(Result.current.HasData).toBe(true));

    expect(Result.current.Coverage?.Percent).toBe(90);
    expect(Result.current.Finance?.Average).toContain('3,000.00');
    expect(Result.current.Finance?.Path).toBe('/charges');
    expect(Result.current.Procedures?.Path).toBeNull();
    expect(Result.current.Academic.map((Kpi) => Kpi.Path)).toEqual([null, null, null, null]);
    expect(Result.current.Term?.Progress?.DaysLeft).toBe(33);
    expect(Result.current.LastUpdated).not.toBeNull();
  });

  it('sin periodo en curso no calcula cobertura', async () => {
    Mocks.CurrentUser = BuildUser(['Reports.View']);
    Mocks.GetDashboard.mockResolvedValue({ ...SampleDashboard, CurrentTermId: null, TermEnrollments: 0 });

    const { result: Result } = renderHook(() => useDashboardPageViewModel(new Date(2026, 10, 7)));

    await waitFor(() => expect(Result.current.HasData).toBe(true));

    expect(Result.current.HasNoTerm).toBe(true);
    expect(Result.current.Term).toBeNull();
    expect(Result.current.Coverage).toBeNull();
  });
});
