// Contratos de calificaciones por grupo
export interface Grade {
  Id: number;
  EnrollmentId: number;
  ClassGroupId: number;
  Partial1: number | null;
  Partial2: number | null;
  Partial3: number | null;
  Ordinary: number | null;
  FinalScore: number | null;
  AttemptNumber: number;
  EvaluationType: string;
  Status: string;
  IsReportClosed: boolean;
}

export interface GradeItemRequest {
  EnrollmentId: number;
  Partial1: number | null;
  Partial2: number | null;
  Partial3: number | null;
  Ordinary: number | null;
  FinalScore: number | null;
  Status: string;
}

export interface SaveGradesRequest {
  Grades: GradeItemRequest[];
}
