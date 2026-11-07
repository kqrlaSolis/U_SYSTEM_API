// Contrato de alumno (GET /api/v1/students)
export interface Student {
  Id: number;
  StudentNumber: string;
  CareerId: number;
  CareerName: string;
  StudyPlanId: number;
  EnrollmentTermId: number;
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string | null;
  FullName: string;
  Curp: string;
  BirthDate: string;
  Gender: string;
  Email: string | null;
  Phone: string | null;
  AddressLine: string | null;
  City: string | null;
  State: string | null;
  PostalCode: string | null;
  Status: string;
}

export interface StudentRequest {
  CareerId: number;
  StudyPlanId: number;
  EnrollmentTermId: number;
  FirstName: string;
  PaternalLastName: string;
  MaternalLastName: string | null;
  Curp: string;
  BirthDate: string;
  Gender: string;
  Email: string | null;
  Phone: string | null;
  AddressLine: string | null;
  City: string | null;
  State: string | null;
  PostalCode: string | null;
  Status: string;
}
