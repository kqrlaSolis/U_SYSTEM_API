// Contratos de finanzas (conceptos y cargos)
export interface PaymentConcept {
  Id: number;
  Code: string;
  Name: string;
  BaseAmount: number;
  IsRecurring: boolean;
  IsActive: boolean;
}

export interface PaymentConceptRequest {
  Code: string;
  Name: string;
  BaseAmount: number;
  IsRecurring: boolean;
  IsActive: boolean;
}

export interface Charge {
  Id: number;
  StudentId: number;
  AcademicTermId: number;
  PaymentConceptId: number;
  Amount: number;
  Discount: number;
  Surcharge: number;
  DueDate: string;
  Status: string;
  Total: number;
}

export interface ChargeRequest {
  StudentId: number;
  AcademicTermId: number;
  PaymentConceptId: number;
  Amount: number;
  Discount: number;
  Surcharge: number;
  DueDate: string;
}
