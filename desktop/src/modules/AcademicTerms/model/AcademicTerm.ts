// Contrato de periodo cuatrimestral (GET /api/v1/academic-terms)
export interface AcademicTerm {
  Id: number;
  Code: string;
  Name: string;
  AcademicYear: number;
  TermNumber: number;
  StartDate: string;
  EndDate: string;
  EnrollmentStartDate: string;
  EnrollmentEndDate: string;
  ReEnrollmentStartDate: string;
  ReEnrollmentEndDate: string;
  ExamStartDate: string;
  ExamEndDate: string;
  GradeReportCloseDate: string;
  Status: string;
}

// Fechas en formato ISO (yyyy-mm-dd), que es lo que espera DateOnly en la API
export interface AcademicTermRequest {
  Code: string;
  Name: string;
  AcademicYear: number;
  TermNumber: number;
  StartDate: string;
  EndDate: string;
  EnrollmentStartDate: string;
  EnrollmentEndDate: string;
  ReEnrollmentStartDate: string;
  ReEnrollmentEndDate: string;
  ExamStartDate: string;
  ExamEndDate: string;
  GradeReportCloseDate: string;
}
