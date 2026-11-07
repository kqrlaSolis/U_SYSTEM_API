// Contrato de docente (GET /api/v1/teachers)
export interface Teacher {
  Id: number;
  EmployeeNumber: string;
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string | null;
  FullName: string;
  Email: string;
  Phone: string | null;
  Specialty: string | null;
  UserId: number | null;
  IsActive: boolean;
}

export interface TeacherRequest {
  EmployeeNumber: string;
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string | null;
  Email: string;
  Phone: string | null;
  Specialty: string | null;
  UserId: number | null;
  IsActive: boolean;
}
