// Contrato de inscripción (GET /api/v1/enrollments)
export interface Enrollment {
  Id: number;
  StudentId: number;
  AcademicTermId: number;
  CareerId: number;
  Type: string;
  Status: string;
  Notes: string | null;
  ClassGroupIds: number[];
}

export interface EnrollmentRequest {
  StudentId: number;
  AcademicTermId: number;
  CareerId: number;
  Notes: string | null;
  ClassGroupIds: number[];
}
