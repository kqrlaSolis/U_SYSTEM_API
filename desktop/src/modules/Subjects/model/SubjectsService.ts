import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { Subject, SubjectRequest } from './Subject';

const SubjectsPath = '/api/v1/subjects';

// Servicio del módulo: sin estado de UI y sin conocer React
export const SubjectsService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<Subject>> {
    return HttpClient.Get<PagedResult<Subject>>(SubjectsPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<Subject> {
    return HttpClient.Get<Subject>(`${SubjectsPath}/${Id}`);
  },

  async Create(Request: SubjectRequest): Promise<Subject> {
    return HttpClient.Post<Subject>(SubjectsPath, Request);
  },

  async Update(Id: number, Request: SubjectRequest): Promise<Subject> {
    return HttpClient.Put<Subject>(`${SubjectsPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${SubjectsPath}/${Id}`);
  },
};
