// Contrato de material didáctico (GET /api/v1/course-materials)
export interface CourseMaterial {
  Id: number;
  SubjectId: number;
  AcademicTermId: number | null;
  Type: string;
  Title: string;
  Description: string | null;
  Url: string | null;
  FilePath: string | null;
  IsActive: boolean;
}

export interface CourseMaterialRequest {
  SubjectId: number;
  AcademicTermId: number | null;
  Type: string;
  Title: string;
  Description: string | null;
  Url: string | null;
  FilePath: string | null;
  IsActive: boolean;
}
