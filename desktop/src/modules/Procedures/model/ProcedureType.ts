// Contratos de trámites (tipos y solicitudes)
export interface ProcedureType {
  Id: number;
  Code: string;
  Name: string;
  PaymentConceptId: number | null;
  IsActive: boolean;
}

export interface ProcedureTypeRequest {
  Code: string;
  Name: string;
  PaymentConceptId: number | null;
  IsActive: boolean;
}

export interface Procedure {
  Id: number;
  ProcedureTypeId: number;
  ProcedureTypeCode: string;
  StudentId: number;
  FolioNumber: string;
  Status: string;
  RequestedAt: string;
  IssuedAt: string | null;
  FilePath: string | null;
  UserId: number;
}
