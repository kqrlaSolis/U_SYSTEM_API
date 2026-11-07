// Expediente de inscripción (checklist) para el asistente por pasos.
export interface ChecklistDocument {
  DocumentTypeId: number;
  Name: string;
  IsMandatory: boolean;
  StudentDocumentId: number | null;
  Status: string;
  FilePath: string | null;
  Notes: string | null;
}

export interface EnrollmentChecklist {
  StudentId: number;
  StudentNumber: string;
  FullName: string;
  AcademicTermId: number;
  Documents: ChecklistDocument[];
  PendingCharges: number;
  PendingAmount: number;
  DocumentsComplete: boolean;
  PaymentClear: boolean;
  CanConfirm: boolean;
  Warnings: string[];
  BlockingReasons: string[];
}

export interface RegisterDocumentRequest {
  DocumentTypeId: number;
  FilePath: string | null;
  Notes: string | null;
}
