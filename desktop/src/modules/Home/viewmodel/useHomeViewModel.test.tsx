import { renderHook, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { AuthUser } from '../../Auth/model/AuthResponse';
import type { Dashboard } from '../../Reports/model/Dashboard';
import { useHomeViewModel } from './useHomeViewModel';

const Mocks = vi.hoisted(() => ({
  CurrentUser: null as AuthUser | null,
  GetDashboard: vi.fn(),
}));

vi.mock('../../../shared/auth/AuthContext', () => ({
  useAuth: () => ({ CurrentUser: Mocks.CurrentUser }),
}));

vi.mock('../../Reports/model/ReportsService', () => ({
  ReportsService: { GetDashboard: Mocks.GetDashboard },
}));

const BuildUser = (Permissions: string[]): AuthUser => ({
  Id: 1,
  Username: 'registro',
  FullName: 'Ana Pérez López',
  Email: 'ana@usystem.local',
  Roles: ['Registrar'],
  Permissions,
});

const SampleDashboard: Dashboard = {
  CurrentTermId: 3,
  CurrentTermCode: '2026-3',
  CurrentTermName: 'Septiembre - Diciembre 2026',
  CurrentTermStatus: 'InProgress',
  CurrentTermStartDate: '2026-09-01',
  CurrentTermEndDate: '2026-12-10',
  ActiveStudents: 420,
  ActiveTeachers: 35,
  OpenClassGroups: 28,
  TermEnrollments: 390,
  PendingCharges: 57,
  PendingAmount: 182500,
  RequestedProcedures: 6,
  IssuedProcedures: 14,
  TotalEnrollments: 1200,
};

describe('useHomeViewModel', () => {
  beforeEach(() => {
    Mocks.GetDashboard.mockReset();
  });

  it('sin Reports.View no pide indicadores y solo muestra los accesos permitidos', () => {
    Mocks.CurrentUser = BuildUser(['Students.View', 'Students.Create']);

    const { result: Result } = renderHook(() => useHomeViewModel(new Date(2026, 10, 7)));

    expect(Mocks.GetDashboard).not.toHaveBeenCalled();
    expect(Result.current.CanSeeReports).toBe(false);
    expect(Result.current.FirstName).toBe('Ana');
    expect(Result.current.Initials).toBe('AP');
    expect(Result.current.Groups.flatMap((Group) => Group.Items.map((Item) => Item.Id))).toEqual(['students']);
    expect(Result.current.QuickActions.map((Action) => Action.Id)).toEqual(['student']);
  });

  it('con Reports.View arma indicadores y el avance del periodo', async () => {
    Mocks.CurrentUser = BuildUser(['Reports.View', 'Students.View', 'Payments.View']);
    Mocks.GetDashboard.mockResolvedValue(SampleDashboard);

    const { result: Result } = renderHook(() => useHomeViewModel(new Date(2026, 10, 7)));

    await waitFor(() => expect(Result.current.Indicators).toHaveLength(4));

    const ById = Object.fromEntries(Result.current.Indicators.map((Indicator) => [Indicator.Id, Indicator]));
    expect(ById.students.Path).toBe('/students');
    expect(ById.charges.Path).toBe('/charges');
    // Sin permiso de grupos ni trámites el indicador se ve pero no navega
    expect(ById.groups.Path).toBeNull();
    expect(ById.procedures.Path).toBeNull();

    expect(Result.current.Term?.Code).toBe('2026-3');
    expect(Result.current.Term?.Progress?.DaysLeft).toBe(33);
  });
});
