// Contrato de materia (GET /api/v1/subjects)
export interface Subject {
  Id: number;
  Code: string;
  Name: string;
  Credits: number;
  TheoryHours: number;
  PracticeHours: number;
  IsActive: boolean;
}

export interface SubjectRequest {
  Code: string;
  Name: string;
  Credits: number;
  TheoryHours: number;
  PracticeHours: number;
  IsActive: boolean;
}
