// Contratos de seguimiento académico (kardex y avance)
export interface TranscriptEntry {
  AcademicTermId: number;
  ClassGroupId: number;
  ClassGroupCode: string;
  SubjectCode: string;
  SubjectName: string;
  FinalScore: number | null;
  Status: string;
  AttemptNumber: number;
}

export interface Transcript {
  StudentId: number;
  StudentNumber: string;
  FullName: string;
  Entries: TranscriptEntry[];
}

export interface AcademicProgress {
  StudentId: number;
  StudentNumber: string;
  FullName: string;
  StudyPlanId: number;
  PlanTotalCredits: number;
  ApprovedCredits: number;
  ProgressPercent: number;
  PassedSubjects: number;
  FailedSubjects: number;
}
