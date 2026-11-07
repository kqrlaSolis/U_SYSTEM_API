import type { PagedResult } from '../../../shared/http/ApiResponse';
import { HttpClient } from '../../../shared/http/HttpClient';
import type { Teacher, TeacherRequest } from './Teacher';

const TeachersPath = '/api/v1/teachers';

// Servicio del módulo: sin estado de UI y sin conocer React
export const TeachersService = {
  async GetPaged(Page: number, PageSize: number, Search: string): Promise<PagedResult<Teacher>> {
    return HttpClient.Get<PagedResult<Teacher>>(TeachersPath, { Page, PageSize, Search });
  },

  async GetById(Id: number): Promise<Teacher> {
    return HttpClient.Get<Teacher>(`${TeachersPath}/${Id}`);
  },

  async Create(Request: TeacherRequest): Promise<Teacher> {
    return HttpClient.Post<Teacher>(TeachersPath, Request);
  },

  async Update(Id: number, Request: TeacherRequest): Promise<Teacher> {
    return HttpClient.Put<Teacher>(`${TeachersPath}/${Id}`, Request);
  },

  async Delete(Id: number): Promise<void> {
    await HttpClient.Delete<boolean>(`${TeachersPath}/${Id}`);
  },
};
