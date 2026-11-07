// Contrato de grupo de clase (GET /api/v1/class-groups)
export interface ClassGroupSchedule {
  DayOfWeek: number;
  StartTime: string;
  EndTime: string;
}

export interface ClassGroup {
  Id: number;
  AcademicTermId: number;
  SubjectId: number;
  TeacherId: number;
  Code: string;
  Classroom: string | null;
  Capacity: number;
  EnrolledCount: number;
  Status: string;
  Schedules: ClassGroupSchedule[];
}

export interface ClassGroupRequest {
  AcademicTermId: number;
  SubjectId: number;
  TeacherId: number;
  Code: string;
  Classroom: string | null;
  Capacity: number;
  Status: string;
  Schedules: ClassGroupSchedule[];
}
