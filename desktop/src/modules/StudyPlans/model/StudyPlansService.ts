import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type {
  StudyPlan,
  StudyPlanPrerequisitesRequest,
  StudyPlanPrerequisiteItem,
  StudyPlanRequest,
  StudyPlanSubjectItem,
} from './StudyPlan';

const PlansPath = '/api/v1/study-plans';

// Servicio del módulo: sin estado de UI y sin conocer React
export const StudyPlansService = {
  async GetPaged(
    Page: number,
    PageSize: number,
    Search: string,
    Filters: Record<string, string> = {},
  ): Promise<PagedResult<StudyPlan>> {
    return HttpClient.Get<PagedResult<StudyPlan>>(PlansPath, { Page, PageSize, Search, ...Filters });
  },

  async GetById(Id: number): Promise<StudyPlan> {
    return HttpClient.Get<StudyPlan>(`${PlansPath}/${Id}`);
  },

  async Create(Request: StudyPlanRequest): Promise<StudyPlan> {
    return HttpClient.Post<StudyPlan>(PlansPath, Request);
  },

  async Update(Id: number, Request: StudyPlanRequest): Promise<StudyPlan> {
    return HttpClient.Put<StudyPlan>(`${PlansPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${PlansPath}/${Id}`);
  },

  async ReplaceSubjects(Id: number, Subjects: StudyPlanSubjectItem[]): Promise<StudyPlan> {
    return HttpClient.Put<StudyPlan>(`${PlansPath}/${Id}/subjects`, { Subjects });
  },

  async ReplacePrerequisites(
    Id: number,
    Prerequisites: StudyPlanPrerequisiteItem[],
  ): Promise<StudyPlan> {
    const Request: StudyPlanPrerequisitesRequest = { Prerequisites };
    return HttpClient.Put<StudyPlan>(`${PlansPath}/${Id}/prerequisites`, Request);
  },

  async SetCurrent(Id: number): Promise<StudyPlan> {
    return HttpClient.Post<StudyPlan>(`${PlansPath}/${Id}/set-current`, {});
  },
};
