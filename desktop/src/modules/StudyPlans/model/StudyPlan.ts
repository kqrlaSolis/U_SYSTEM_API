// Contratos de /api/v1/study-plans
export interface StudyPlanSubject {
  SubjectId: number;
  SubjectCode: string;
  SubjectName: string;
  Credits: number;
  TermNumber: number;
  Type: string;
}

export interface StudyPlanPrerequisite {
  SubjectId: number;
  SubjectCode: string;
  PrerequisiteSubjectId: number;
  PrerequisiteSubjectCode: string;
}

export interface StudyPlan {
  Id: number;
  CareerId: number;
  CareerName: string;
  Code: string;
  Version: string;
  StartDate: string;
  EndDate: string | null;
  TotalCredits: number;
  AssignedCredits: number;
  IsCurrent: boolean;
  Subjects: StudyPlanSubject[];
  Prerequisites: StudyPlanPrerequisite[];
}

export interface StudyPlanRequest {
  CareerId: number;
  Code: string;
  Version: string;
  StartDate: string;
  EndDate: string | null;
  TotalCredits: number;
}

export interface StudyPlanSubjectItem {
  SubjectId: number;
  TermNumber: number;
  Type: string;
}

export interface StudyPlanPrerequisiteItem {
  SubjectId: number;
  PrerequisiteSubjectId: number;
}

export interface StudyPlanSubjectsRequest {
  Subjects: StudyPlanSubjectItem[];
}

export interface StudyPlanPrerequisitesRequest {
  Prerequisites: StudyPlanPrerequisiteItem[];
}
