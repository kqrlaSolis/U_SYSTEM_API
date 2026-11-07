import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { Enrollment, EnrollmentRequest } from './Enrollment';
import type {
  EnrollmentChecklist,
  RegisterDocumentRequest,
} from './EnrollmentChecklist';

const EnrollmentsPath = '/api/v1/enrollments';

// Servicio del módulo: sin estado de UI y sin conocer React
export const EnrollmentsService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<Enrollment>> {
    return HttpClient.Get<PagedResult<Enrollment>>(EnrollmentsPath, { Page, PageSize, Search });
  },

  async GetChecklist(StudentId: number, AcademicTermId: number): Promise<EnrollmentChecklist> {
    return HttpClient.Get<EnrollmentChecklist>(`/api/v1/students/${StudentId}/enrollment-checklist`, {
      AcademicTermId,
    });
  },

  async RegisterDocument(
    StudentId: number,
    Request: RegisterDocumentRequest,
  ): Promise<void> {
    await HttpClient.Post<unknown>(`/api/v1/students/${StudentId}/documents`, Request);
  },

  async ValidateDocument(DocumentId: number): Promise<void> {
    await HttpClient.Post<unknown>(`/api/v1/student-documents/${DocumentId}/validate`, {});
  },

  async CreateReEnrollment(
    StudentId: number,
    AcademicTermId: number,
    CareerId: number,
    ClassGroupIds: number[],
  ): Promise<Enrollment> {
    return HttpClient.Post<Enrollment>(`/api/v1/students/${StudentId}/re-enrollment-proposal`, {
      StudentId,
      AcademicTermId,
      CareerId,
      Notes: null,
      ClassGroupIds,
    });
  },

  async Confirm(EnrollmentId: number): Promise<void> {
    await HttpClient.Post<unknown>(`/api/v1/enrollments/${EnrollmentId}/confirm`, {});
  },

  async GetById(Id: number): Promise<Enrollment> {
    return HttpClient.Get<Enrollment>(`${EnrollmentsPath}/${Id}`);
  },

  async Create(Request: EnrollmentRequest): Promise<Enrollment> {
    return HttpClient.Post<Enrollment>(EnrollmentsPath, Request);
  },

  async Cancel(Id: number, Reason: string): Promise<Enrollment> {
    return HttpClient.Post<Enrollment>(`${EnrollmentsPath}/${Id}/cancel`, { Reason });
  },
};
