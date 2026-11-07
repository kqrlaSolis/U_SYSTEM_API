// Indicadores del dashboard (GET /api/v1/reports/dashboard)
export interface Dashboard {
  CurrentTermId: number | null;
  CurrentTermCode: string | null;
  CurrentTermName: string | null;
  CurrentTermStatus: string | null;
  CurrentTermStartDate: string | null;
  CurrentTermEndDate: string | null;
  ActiveStudents: number;
  ActiveTeachers: number;
  OpenClassGroups: number;
  TermEnrollments: number;
  PendingCharges: number;
  PendingAmount: number;
  RequestedProcedures: number;
  IssuedProcedures: number;
  TotalEnrollments: number;
}
